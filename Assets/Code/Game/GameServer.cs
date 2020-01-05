using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace PongGame
{
    public sealed class GameServer : IInitializable, ITickable, IDisposable, INetEventListener
    {
        [Inject] private GameSettings gameSettings;
        [Inject] private Ball.Pool ballPool;

        public event Action OnPeersDisconected;
        public event Action<int> OnPlayerConnected;
        public event Action<bool> OnClientPausedEvent;

        public const int MAX_CONNECTIONS = 2;
        public const string NET_KEY = "zenpong_game";

        private readonly List<NetPeer> connectedPlayers = new List<NetPeer>();
        private readonly NetPacketProcessor netPacketProcessor = new NetPacketProcessor();
        private readonly RestartGamePacket restartGamePacket = new RestartGamePacket();
        private readonly BallPositionPacket ballPositionPacket = new BallPositionPacket();
        private readonly GameScorePacket gameScorePacket = new GameScorePacket();

        private NetManager server;
        private NetPeer hoster;

        Ball ball;

        public void Initialize()
        {
            //register auto serializable vector2
            netPacketProcessor.RegisterNestedType((w, v) => w.Put(v), r => r.GetVector2());

            // subscribe packets
            netPacketProcessor.SubscribeReusable<JoinPacket, NetPeer>(OnPlayerJoin);
            netPacketProcessor.SubscribeReusable<PaddlePositionPacket, NetPeer>(OnPaddlePosition);
            netPacketProcessor.SubscribeReusable<PasueGamePacket>(OnClientPaused);

            // start server
            server = new NetManager(this)
            {
                AutoRecycle = true
            };
            server.Start(gameSettings.port);
        }

        private void OnPlayerJoin(JoinPacket packet, NetPeer peer)
        {
            if (!ball)
                SpawnBall();

            if (peer.Id == 0)
                hoster = peer;

            //Send join accept
            var ja = new JoinAcceptPacket { id = peer.Id };
            peer.Send(WritePacket(ja), DeliveryMethod.ReliableOrdered);
            connectedPlayers.Add(peer);

            //Send to old players info about new player
            var pj = new PlayerJoinedPacket();
            pj.isNewPlayer = true;
            server.SendToAll(WritePacket(pj), DeliveryMethod.ReliableOrdered, peer);

            //Send to new player info about old players
            pj.isNewPlayer = false;
            foreach (NetPeer otherPlayer in connectedPlayers)
            {
                if (otherPlayer == peer)
                    continue;
                peer.Send(WritePacket(pj), DeliveryMethod.ReliableOrdered);
            }

            if (peer != hoster)
            {
                // Send spawned ball
                var spawnBallPacket = new SpawnBallPacket
                {
                position = ball.transform.position,
                scale = ball.transform.localScale.x
                };
                peer.Send(WritePacket(spawnBallPacket), DeliveryMethod.ReliableOrdered);
            }

            if (OnPlayerConnected != null)
                OnPlayerConnected(server.PeersCount);
        }

        private void SpawnBall()
        {
            var randomDir = Random.insideUnitCircle;
            randomDir.x = Mathf.Clamp(randomDir.x, 0.5f, 0.8f) * (Random.value > 0.5f ? 1 : -1);
            randomDir.y = Mathf.Clamp(randomDir.x, 0.2f, 0.5f) * (Random.value > 0.5f ? 1 : -1);
            var scale = Random.Range(gameSettings.ballScaleRange.x, gameSettings.ballScaleRange.y);
            var speed = Random.Range(gameSettings.ballSpeedRange.x, gameSettings.ballSpeedRange.y);
            ball = ballPool.Spawn(randomDir, speed, scale);
        }

        public void PushBall()
        {
            ball.Push();
        }

        public void StartGame()
        {
            server.SendToAll(WritePacket(new StarGamePacket()), DeliveryMethod.ReliableOrdered, hoster);
        }

        public void RestartGame()
        {
            ballPool.Despawn(ball);
            SpawnBall();
            var spawnBallPacket = new SpawnBallPacket
            {
                position = ball.transform.position,
                scale = ball.transform.localScale.x
            };
            server.SendToAll(WritePacket(restartGamePacket), DeliveryMethod.ReliableOrdered, hoster);
            server.SendToAll(WritePacket(spawnBallPacket), DeliveryMethod.ReliableOrdered, hoster);
        }

        private void OnPaddlePosition(PaddlePositionPacket packet, NetPeer peer)
        {
            server.SendToAll(WritePacket(packet), DeliveryMethod.ReliableOrdered, peer);
        }

        public void Tick()
        {
            server.PollEvents();
            if (ball)
            {
                ballPositionPacket.position = ball.transform.position;
                server.SendToAll(WritePacket(ballPositionPacket), DeliveryMethod.ReliableOrdered, hoster);
            }
        }

        public void SendGameScore(int player1Score, int player2Score)
        {
            gameScorePacket.player1Score = player1Score;
            gameScorePacket.player2Score = player2Score;
            server.SendToAll(WritePacket(gameScorePacket), DeliveryMethod.ReliableOrdered, hoster);
        }

        public void PauseGame(bool isPaused)
        {
            server.SendToAll(WritePacket(new PasueGamePacket { isPaused = isPaused }), DeliveryMethod.ReliableOrdered, hoster);
        }

        private void OnClientPaused(PasueGamePacket packet)
        {
            if (OnClientPausedEvent != null)
                OnClientPausedEvent(packet.isPaused);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[SERVER] Player connected: " + peer.EndPoint);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Debug.Log("[SERVER] Player disconnected: " + disconnectInfo.Reason);

            if (peer.Tag != null)
            {
                var plp = new PlayerLeavedPacket { id = peer.Id };
                server.SendToAll(WritePacket(plp), DeliveryMethod.ReliableOrdered);
            }
            if (server.PeersCount == 1)
            {
                OnPeersDisconected();
            }
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
        {
            Debug.Log($"[SERVER] NetworkError: {socketError}");
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            netPacketProcessor.ReadAllPackets(reader, peer);
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            if (server.PeersCount < MAX_CONNECTIONS /* max connections */ )
                request.AcceptIfKey(NET_KEY);
            else
                request.Reject();
        }

        private readonly NetDataWriter cachedDataWriter = new NetDataWriter();

        private NetDataWriter WritePacket<T>(T packet) where T : class, new()
        {
            cachedDataWriter.Reset();
            netPacketProcessor.Write(cachedDataWriter, packet);
            return cachedDataWriter;
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void Dispose()
        {
            NetDebug.Logger = null;
            if (server != null)
                server.Stop();
        }

        [DllImport("__Internal")]
        private static extern string getLocalWifiIpAddress();

        public static string GetLocalIPAddress()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return getLocalWifiIpAddress();
#else
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "0.0.0.0";
#endif
        }
    }
}
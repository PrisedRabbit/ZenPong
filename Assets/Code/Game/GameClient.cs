using System;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public sealed class GameClient : IInitializable, ITickable, IDisposable, INetEventListener
    {
        [Inject] private GameSettings gameSettings;
        [Inject] private InputHandler inputHandler;
        [Inject] private SceneReferences sceneReferences;
        [Inject] private AtomsReferences atoms;
        [Inject] private Ball.Pool ballPool;

        public event Action OnClientConnectedEvent;
        public event Action OnClientDisConnectedEvent;
        public event Action OnGameRestartEvent;
        public event Action<bool> OnHostPausedEvent;

        private NetManager netManager;
        private NetPeer server;
        private NetPeer thisPeer;

        private readonly NetPacketProcessor netPacketProcessor = new NetPacketProcessor();
        private readonly PaddlePositionPacket paddlePositionPacket = new PaddlePositionPacket();

        Transform localPaddle, remotePaddle, ballTransform;
        Ball ball;

        public void Initialize()
        {
            //register auto serializable vector2
            netPacketProcessor.RegisterNestedType((w, v) => w.Put(v), reader => reader.GetVector2());

            // subscribe packets
            netPacketProcessor.SubscribeReusable<PlayerJoinedPacket>(OnPlayerJoined);
            netPacketProcessor.SubscribeReusable<JoinAcceptPacket>(OnJoinAccept);
            netPacketProcessor.SubscribeReusable<PlayerLeavedPacket>(OnPlayerLeaved);
            netPacketProcessor.SubscribeReusable<SpawnBallPacket>(OnBallSpawn);
            netPacketProcessor.SubscribeReusable<BallPositionPacket>(OnBallPositionUpdate);
            netPacketProcessor.SubscribeReusable<PaddlePositionPacket>(OnRemotePaddlePositionUpdate);
            netPacketProcessor.SubscribeReusable<RestartGamePacket>(OnGameRestart);
            netPacketProcessor.SubscribeReusable<PasueGamePacket>(OnGamePaused);
            netPacketProcessor.SubscribeReusable<GameScorePacket>(OnGameScore);

            // start client
            netManager = new NetManager(this);
            netManager.Start();
            thisPeer = netManager.Connect(atoms.NetworkAddressVariable.Value, gameSettings.port, GameServer.NET_KEY);
        }

        private void OnGameRestart(RestartGamePacket packet)
        {
            if (gameSettings.networkMode == NetworkMode.Client)
            {
                ballPool.Despawn(ball);
                ball.gameObject.SetActive(false);
                ballTransform = null;
            }
            if (OnGameRestartEvent != null)
                OnGameRestartEvent();
        }

        private void OnPlayerJoined(PlayerJoinedPacket packet)
        {
            if (packet.isNewPlayer)
            {
                Debug.Log($"[CLIENT] New Player joined");
                remotePaddle = SpawnPaddleAt(sceneReferences.SpawnPoint2, false);
            }
            else
            {
                Debug.Log($"[CLIENT] Old Player joined");
                remotePaddle = SpawnPaddleAt(sceneReferences.SpawnPoint1, false);
            }
        }

        private void OnPlayerLeaved(PlayerLeavedPacket packet)
        {
            Debug.Log($"[CLIENT] Player leaved: {packet}");
        }

        private void OnJoinAccept(JoinAcceptPacket packet)
        {
            Debug.Log("[CLIENT] Join accept. Received player id: " + packet.id);
            var spawnPoint = sceneReferences.SpawnPoint1;
            if (packet.id > 0)
                spawnPoint = sceneReferences.SpawnPoint2;
            localPaddle = SpawnPaddleAt(spawnPoint, true);
            if (OnClientConnectedEvent != null)
                OnClientConnectedEvent();
        }

        private void OnBallSpawn(SpawnBallPacket packet)
        {
            if (gameSettings.networkMode == NetworkMode.Client)
            {
                ball = ballPool.Spawn(Vector2.zero, 0, packet.scale);
                ballTransform = ball.transform;
                ballTransform.position = packet.position;
                lastBallPosition = packet.position;
                ball.gameObject.SetActive(true);
            }
        }

        Transform SpawnPaddleAt(Transform spawnPoint, bool controllable)
        {
            var paddle = GameObject.Instantiate(sceneReferences.PaddlePrefab, spawnPoint.position, Quaternion.identity);
            paddle.tag = "Player";
            if (controllable)
            {
                inputHandler.AddMovable(paddle.GetComponent<IHorizontalMoveable>());
            }
            return paddle.transform;
        }

        private void OnGameScore(GameScorePacket packet)
        {
            atoms.ScoreVariable.Value = packet.player1Score;
            atoms.ScorePlayer2Variable.Value = packet.player2Score;
        }

        Vector2 lastBallPosition;
        Vector2 lastRemotePaddlePosition;

        private void OnBallPositionUpdate(BallPositionPacket packet)
        {
            lastBallPosition = packet.position;
        }

        private void OnRemotePaddlePositionUpdate(PaddlePositionPacket packet)
        {
            lastRemotePaddlePosition = packet.position;
        }

        public void Tick()
        {
            netManager.PollEvents();

            if (ballTransform)
            {
                ballTransform.position = Vector2.Lerp(ballTransform.position, lastBallPosition, Time.deltaTime * 50);
            }
            if (localPaddle)
            {
                paddlePositionPacket.position = localPaddle.position;
                SendPacket<PaddlePositionPacket>(paddlePositionPacket, DeliveryMethod.ReliableOrdered);
            }
            if (remotePaddle)
            {
                remotePaddle.position = Vector2.Lerp(remotePaddle.position, lastRemotePaddlePosition, Time.deltaTime * 50);
            }
        }

        public void PauseGame(bool isPaused)
        {
            SendPacket<PasueGamePacket>(new PasueGamePacket { isPaused = isPaused }, DeliveryMethod.ReliableOrdered);
        }

        private void OnGamePaused(PasueGamePacket packet)
        {
            if (OnHostPausedEvent != null)
                OnHostPausedEvent(packet.isPaused);
        }

        private readonly NetDataWriter cachedDataWriter = new NetDataWriter();

        public void SendPacket<T>(T packet, DeliveryMethod deliveryMethod) where T : class, new()
        {
            cachedDataWriter.Reset();
            netPacketProcessor.Write(cachedDataWriter, packet);
            server.Send(cachedDataWriter, deliveryMethod);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            Debug.Log("[CLIENT] Connected to server: " + peer.EndPoint);
            server = peer;
            SendPacket(new JoinPacket(), DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (OnClientConnectedEvent != null)
                OnClientDisConnectedEvent();
        }

        public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) { }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            netPacketProcessor.ReadAllPackets(reader);
        }

        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) { }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) { }

        public void OnConnectionRequest(ConnectionRequest request) { }

        public void Dispose()
        {
            if (netManager != null)
                netManager.Stop();
        }
    }
}
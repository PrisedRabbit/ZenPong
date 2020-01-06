using System;
using DG.Tweening;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace PongGame
{
    public class NetworkGameManager : IInitializable, IDisposable
    {
        #pragma warning disable 0649

        [Inject] private GameSettings gameSettings;
        [Inject] private InputHandler inputHandler;
        [Inject] private SceneReferences sceneReferences;
        [Inject] private AtomsReferences atoms;
        [Inject] private Ball.Pool ballPool;

        [InjectOptional] private GameServer server;
        [Inject] private GameClient client;

        #pragma warning restore 0649

        private const string ON_ALL_PLAYERS_READY_MSG = "Let's get rolling the ball!";
        private const string GAME_PAUSED_MSG = "GAME IS PAUSED";
        private const string PLAYERS_DISCONNECTED_MSG = "PLAYER DISCONNECTED";
        private const string HOST_DISCONNECTED_MSG = "HOST DISCONNECTED";

        public void Initialize()
        {
            if (gameSettings.networkMode == NetworkMode.Host)
            {
                atoms.NetworkGameStatusVariable.Value = "Waiting for Player 2 \n Host address: " + GameServer.GetLocalIPAddress();
                atoms.CollisionBallEvent.OnEvent += OnBallCollision;
                atoms.CountDownEndEvent.OnEvent += OnServerCountDownEnd;
                atoms.PauseGameEvent.OnEvent += OnServerGamePaused;
                server.OnPlayerConnected += OnServerPlayerConnected;
                server.OnClientPausedEvent += OnSomeoneGamePaused;
                server.OnPeersDisconected += OnServerPlayersDisconected;
            }
            else
            {
                atoms.NetworkGameStatusVariable.Value = "Connecting to " + atoms.NetworkAddressVariable.Value;
                atoms.PauseGameEvent.OnEvent += OnClientGamePaused;
                client.OnClientConnectedEvent += OnClientConnected;
                client.OnGameRestartEvent += OnClientGameRestart;
                client.OnHostPausedEvent += OnSomeoneGamePaused;
                client.OnClientDisConnectedEvent += OnClientDisconnected;
            }
        }

        #region Host Behaviour

        private int connectedPlayers;

        private void OnServerPlayerConnected(int totalPlayers)
        {
            Debug.Log("total players " + totalPlayers);
            connectedPlayers = totalPlayers;
            if (totalPlayers == GameServer.MAX_CONNECTIONS)
            {
                atoms.NetworkGameStatusVariable.Value = ON_ALL_PLAYERS_READY_MSG;
                var _ = 0;
                DOTween.To(() => _, t => _ = t, 0, 3).OnComplete(OnServerStartGame);
            }
        }

        private void OnServerPlayersDisconected()
        {
            atoms.NetworkGameStatusVariable.Value = PLAYERS_DISCONNECTED_MSG;
            var delay = 0;
            DOTween.To(() => delay, t => delay = t, 0, 2).SetUpdate(true).OnComplete(atoms.QuitToMainMenuAction.Do);
        }

        private void OnServerCountDownEnd(UnityAtoms.Void _)
        {
            server.PushBall();
        }

        private void OnServerStartGame()
        {
            atoms.NetworkGameStatusVariable.Value = string.Empty;
            server.StartGame();
            atoms.RestartLevelEvent.Raise();
        }

        private void OnServerRestartGame()
        {
            atoms.NetworkGameStatusVariable.Value = string.Empty;
            server.RestartGame();
            atoms.RestartLevelEvent.Raise();
        }

        private void OnServerGamePaused(bool isPaused)
        {
            server.PauseGame(isPaused);
        }

        private void OnBallCollision(Collider2D collider)
        {
            var collidedGO = collider.gameObject;
            if (collidedGO.CompareTag("Player"))
            {
                // nothing yet
            }
            else if (collidedGO.tag.Contains("Goal"))
            {
                if (collidedGO.CompareTag("Goal1"))
                {
                    atoms.ScorePlayer2Variable.Value += 1;
                }
                else
                {
                    atoms.ScoreVariable.Value += 1;
                }
                server.SendGameScore(atoms.ScoreVariable.Value, atoms.ScorePlayer2Variable.Value);
                // just adds delay between restart
                var _ = 0;
                DOTween.To(() => _, t => _ = t, 0, 1).OnComplete(OnServerRestartGame);
            }
            //TODO: fix ball's vertical velocity, to prevent infitiy bounce between obstacles
        }

        #endregion // Server

        #region Client Behaviour

        private void OnClientConnected()
        {
            atoms.NetworkGameStatusVariable.Value = ON_ALL_PLAYERS_READY_MSG;
        }

        private void OnClientDisconnected()
        {
            atoms.NetworkGameStatusVariable.Value = HOST_DISCONNECTED_MSG;
            var _ = 0;
            DOTween.To(() => _, t => _ = t, 0, 2).SetUpdate(true).OnComplete(atoms.QuitToMainMenuAction.Do);
        }

        private void OnClientGameRestart()
        {
            atoms.NetworkGameStatusVariable.Value = string.Empty;
            atoms.RestartLevelEvent.Raise();
        }

        private void OnClientGamePaused(bool isPaused)
        {
            client.PauseGame(isPaused);
        }

        #endregion // Client

        #region Shared Behaviour

        private void OnSomeoneGamePaused(bool isPaused)
        {
            if (isPaused)
            {
                Time.timeScale = 0f;
                atoms.NetworkGameStatusVariable.Value = GAME_PAUSED_MSG;
            }
            else
            {
                Time.timeScale = 1f;
                atoms.NetworkGameStatusVariable.Value = string.Empty;
            }
        }

        #endregion // Shared

        public void Dispose()
        {
            atoms.CollisionBallEvent.OnEvent -= OnBallCollision;
            atoms.CountDownEndEvent.OnEvent -= OnServerCountDownEnd;
            atoms.PauseGameEvent.OnEvent -= OnServerGamePaused;
            atoms.PauseGameEvent.OnEvent -= OnClientGamePaused;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;
using Zenject;

namespace PongGame
{
    public sealed class LocalGameManager : IInitializable, System.IDisposable
    {
        [Inject] private GameSettings gameSettings;
        [Inject] private InputHandler inputHandler;
        [Inject] private SceneReferences sceneReferences;
        [Inject] private AtomsReferences atomsReferences;

        [Inject] Ball.Pool ballPool;

        Ball ball;

        public void Initialize()
        {
            Application.targetFrameRate = gameSettings.targetFrameRate;

            atomsReferences.CollisionBallEvent.OnEvent += OnBallCollision;
            atomsReferences.CountDownEndEvent.OnEvent += OnCountDownEnd;

            SetupPaddles();
            SpawnBall();

            atomsReferences.RestartLevelEvent.Raise();
        }

        private void SpawnBall()
        {
            var randomDir = Random.insideUnitCircle;
            randomDir.x = Mathf.Clamp(randomDir.x, 0.5f, 0.8f) * (Random.value > 0.5f ? 1 : -1);
            randomDir.y = Mathf.Clamp(randomDir.x, 0.2f, 0.5f) * (Random.value > 0.5f ? 1 : -1);
            ball = ballPool.Spawn(randomDir, 300f);
            ball.transform.position = Vector2.zero;
        }

        private void SetupPaddles()
        {
            var paddle1 = GameObject.Instantiate(sceneReferences.PaddlePrefab, sceneReferences.SpawnPoint1.position, Quaternion.identity);
            var paddle2 = GameObject.Instantiate(sceneReferences.PaddlePrefab, sceneReferences.SpawnPoint2.position, Quaternion.identity);
            paddle1.tag = "Player";
            paddle2.tag = "Player";
            inputHandler.AddMovable(paddle1.GetComponent<IHorizontalMoveable>());
            inputHandler.AddMovable(paddle2.GetComponent<IHorizontalMoveable>());
        }

        void OnCountDownEnd(UnityAtoms.Void nothing)
        {
            ball.Push();
        }

        void OnBallCollision(Collider2D collider)
        {
            var collidedGO = collider.gameObject;
            if (collidedGO.CompareTag("Player"))
            {
                atomsReferences.ScoreVariable.Value += 1;
            }
            else if (collidedGO.tag.Contains("Goal"))
            {
                Restart();
            }
        }

        void Restart()
        {
            atomsReferences.ScoreVariable.Value = 0;
            ballPool.Despawn(ball);
            SpawnBall();
            atomsReferences.RestartLevelEvent.Raise();
        }

        public void Dispose()
        {
            atomsReferences.CollisionBallEvent.OnEvent -= OnBallCollision;
            atomsReferences.CountDownEndEvent.OnEvent -= OnCountDownEnd;
        }
    }
}
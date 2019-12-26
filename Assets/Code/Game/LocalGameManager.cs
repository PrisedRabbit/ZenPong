using System.Collections;
using System.Collections.Generic;
using UnityAtoms;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace PongGame
{
    public sealed class LocalGameManager : IInitializable, System.IDisposable
    {
        private readonly GameSettings gameSettings;
        private readonly InputHandler inputHandler;
        private readonly SceneReferences sceneReferences;
        private readonly AtomsReferences atoms;
        private readonly Ball.Pool ballPool;

        public LocalGameManager(GameSettings gameSettings, InputHandler inputHandler, SceneReferences sceneReferences, AtomsReferences atomsReferences, Ball.Pool ballPool)
        {
            this.gameSettings = gameSettings;
            this.inputHandler = inputHandler;
            this.sceneReferences = sceneReferences;
            this.atoms = atomsReferences;
            this.ballPool = ballPool;
        }

        public void Initialize()
        {
            Application.targetFrameRate = gameSettings.targetFrameRate;

            atoms.CollisionBallEvent.OnEvent += OnBallCollision;
            atoms.CountDownEndEvent.OnEvent += OnCountDownEnd;

            SetupPaddles();
            SpawnBall();

            // lets rolling the ball!
            atoms.RestartLevelEvent.Raise();
        }

        #region create entities
        //TODO: move code to Factories?

        Ball ball;

        private void SpawnBall()
        {
            var randomDir = Random.insideUnitCircle;
            randomDir.x = Mathf.Clamp(randomDir.x, 0.5f, 0.8f) * (Random.value > 0.5f ? 1 : -1);
            randomDir.y = Mathf.Clamp(randomDir.x, 0.2f, 0.5f) * (Random.value > 0.5f ? 1 : -1);
            var scale = Random.Range(gameSettings.ballScaleRange.x, gameSettings.ballScaleRange.y);
            var speed = Random.Range(gameSettings.ballSpeedRange.x, gameSettings.ballSpeedRange.y);
            ball = ballPool.Spawn(randomDir, speed, scale);
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
        #endregion

        #region game logic

        void OnCountDownEnd(UnityAtoms.Void nothing)
        {
            ball.Push();
        }

        void OnBallCollision(Collider2D collider)
        {
            var collidedGO = collider.gameObject;
            if (collidedGO.CompareTag("Player"))
            {
                atoms.ScoreVariable.Value += 1;
            }
            else if (collidedGO.tag.Contains("Goal"))
            {
                // just adds delay between restart
                var delay = 0;
                DOTween.To(() => delay, t => delay = t, 0, 1).OnComplete(Restart);
            }
            //TODO: fix ball's vertical velocity, to prevent infitiy bounce between obstacles
        }

        void Restart()
        {
            Time.timeScale = 1f;
            ballPool.Despawn(ball);
            atoms.ScoreVariable.Value = 0;
            SpawnBall();
            atoms.RestartLevelEvent.Raise();
        }
        
        #endregion

        public void Dispose()
        {
            atoms.CollisionBallEvent.OnEvent -= OnBallCollision;
            atoms.CountDownEndEvent.OnEvent -= OnCountDownEnd;
        }
    }
}
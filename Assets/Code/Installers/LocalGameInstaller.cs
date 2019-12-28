using UnityAtoms;
using UnityEngine;
using Zenject;

namespace PongGame
{
    [RequireComponent(typeof(AtomsReferences))]
    [RequireComponent(typeof(SceneReferences))]
    public sealed class LocalGameInstaller : MonoInstaller
    {
        [Inject] private GameSettings gameSettings;

        private InputHandler inputHandler;

        public override void InstallBindings()
        {
            //bind asset references
            var sceneReferences = GetComponent<SceneReferences>();
            Container.BindInstance(sceneReferences).AsSingle();
            var atoms = GetComponent<AtomsReferences>();
            Container.BindInstance(atoms).AsSingle();

            //bind input
            inputHandler = new InputHandler();
            Container.BindInterfacesAndSelfTo<InputHandler>().FromInstance(inputHandler).AsSingle();

            //bind pools
            Container.BindMemoryPool<Ball, Ball.Pool>()
                .WithInitialSize(1)
                .FromComponentInNewPrefab(sceneReferences.BallPrefab);

            // score manager
            Container.BindInterfacesAndSelfTo<HighScoreManager>().AsSingle();

            //bind logic
            if (gameSettings.gameMode == GameMode.Multiplayer)
            {

            }
            else
            {
                Container.BindInterfacesAndSelfTo<LocalGameManager>().AsSingle();
            }
        }
    }

}
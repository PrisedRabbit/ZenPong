using UnityAtoms;
using UnityEngine;
using Zenject;

namespace PongGame
{
    [RequireComponent(typeof(AtomsReferences))]
    [RequireComponent(typeof(SceneReferences))]
    public sealed class GameInstaller : MonoInstaller
    {
        [Inject] private GameSettings gameSettings;

        private InputHandler inputHandler;

        public override void InstallBindings()
        {
            Container.BindInitializableExecutionOrder<StringToTextSetter>(-100);

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

            // #if UNITY_EDITOR
            //             gameSettings.gameMode = GameMode.NetworkGame;
            // #endif

            //bind managers
            if (gameSettings.gameMode == GameMode.NetworkGame)
            {
                if (gameSettings.networkMode == NetworkMode.Host)
                {
                    Container.BindInterfacesAndSelfTo<GameServer>().AsSingle();
                    atoms.NetworkAddressVariable.Value = "localhost";
                }
                Container.BindInterfacesAndSelfTo<GameClient>().AsSingle();
                Container.BindInterfacesAndSelfTo<NetworkGameManager>().AsSingle();
            }
            else
            {
                Container.BindInterfacesAndSelfTo<HighScoreManager>().AsSingle();
                Container.BindInterfacesAndSelfTo<LocalGameManager>().AsSingle();
            }
        }
    }

}
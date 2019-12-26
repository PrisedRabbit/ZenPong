using UnityEngine;
using Zenject;

namespace PongGame
{
    [RequireComponent(typeof(AtomsReferences))]
    public class MainMenuInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            var atoms = GetComponent<AtomsReferences>();
            Container.BindInstance(atoms).AsSingle();

            Container.BindInterfacesAndSelfTo<HighScoreManager>().AsSingle();
        }
    }
}
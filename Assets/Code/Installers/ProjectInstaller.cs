using UnityEngine;
using Zenject;
using PongGame;

[CreateAssetMenu(fileName = "ProjectInstaller", menuName = "Installers/ProjectInstaller")]
public sealed class ProjectInstaller : ScriptableObjectInstaller<ProjectInstaller>
{
    public GameSettings gameSettings;

    public override void InstallBindings()
    {
            Container.BindInstance(gameSettings).AsSingle();
    }
}
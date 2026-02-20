using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{

    public override void InstallBindings()
    {
        Container.Bind<DicesController>().AsSingle();
        Container.Bind<WaveController>().AsSingle();
        Container.Bind<GameController>().AsSingle();
    }

}

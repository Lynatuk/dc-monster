using Zenject;

public class DiceUpgradeInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<DicesController>().AsSingle();
        Container.Bind<UpgradeWindowController>().AsSingle();
    }
}

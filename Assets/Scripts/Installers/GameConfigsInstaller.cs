using Dice.Configs;
using Dice.Services;
using Zenject;
using UnityEngine;

public class GameConfigsInstaller : MonoInstaller
{

    [SerializeField] private EconomyConfig economyConfig;
    [SerializeField] private FaceBaseConfig faceBaseConfig;
    [SerializeField] private FacePerkTable facePerkTable;

    public override void InstallBindings()
    {
        Container.Bind<EconomyConfig>().FromInstance(economyConfig);
        Container.Bind<FaceBaseConfig>().FromInstance(faceBaseConfig);
        Container.Bind<FacePerkTable>().FromInstance(facePerkTable);

        BindDiceConfig();
    }

    private void BindDiceConfig()
    {
        Container.Bind<DamageService>().AsSingle().NonLazy();
        Container.Bind<DiceUpgrade>().AsSingle().NonLazy();
    }
}

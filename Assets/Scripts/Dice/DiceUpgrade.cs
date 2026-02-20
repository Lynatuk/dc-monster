using Dice.Configs;
using System;
using Zenject;

public class DiceUpgrade
{
    private EconomyConfig _economy;

    [Inject]
    private void Construct(EconomyConfig economy)
    {
        _economy = economy;
    }

    public int GetCurrentCountUpgrades(DiceType diceType)
    {
        var diceProgress = Data.instance.localData.GetOrCreateProgress(diceType);
        int faceCount = (int)diceType;
        int totalLevel = 0;

        for (int i = 0; i < faceCount; i++)
        {
            int level = (int)diceProgress.GetFaceProgress(i).diceFaceLevel;
            totalLevel += level;
        }

        return totalLevel;
    }

    public bool TryUpgrade(DiceType type, int faceIndex, DiceData diceData)
    {
        DiceProgress progress = Data.instance.localData.GetOrCreateProgress(type);
        FaceProgress face = progress.GetFaceProgress(faceIndex);
        int countFaces = (int)type;
        int price = GetUpgradeCost(faceIndex, (int)face.diceFaceLevel, type);

        if ((int)face.diceFaceLevel >= countFaces)
            return false;

        if (Data.instance.localData.coins < price)
            return false;

        face.diceFaceLevel += 1;
        face.damageMultiplier = 1 + 0.1f * (int)face.diceFaceLevel;

        Data.instance.localData.coins -= price;
        CoinsController.updateCoins?.Invoke();
        DiceController.UpdateDice?.Invoke(diceData);

        return true;
    }

    public int GetUpgradeCost(int faceIndex, int currentLevel, DiceType diceType)
    {
        float B = _economy.GetBaseCost(diceType);
        double faceMult = 1.0 + 0.06 * faceIndex;
        double exp = Math.Pow(_economy.upgradeCostExpC, currentLevel);
        double nonlinear = 1.0 + _economy.upgradeNonlinearK * Math.Pow(currentLevel, 1.2);
        return (int)Math.Max(1.0, B * faceMult * exp * nonlinear);
    }
}

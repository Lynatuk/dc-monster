using System;
using System.Collections.Generic;

[Serializable]
public class LocalData
{

    public Settings settings;
    [Serializable]
    public class Settings
    {
        public bool soundState = true;
        public bool musicState = true;
    }

    public int coins;
    public int gems;

    public int currentWave = 0;
    public int currentStage = 0;

    public DiceType currentNumericalDice = DiceType.Numerical6;

    public Dictionary<DiceType, DiceProgress> dicesProgress = new();

    public DiceProgress GetOrCreateProgress(DiceType type)
    {
        if (!dicesProgress.TryGetValue(type, out var progress))
        {
            progress = new DiceProgress();
            for (int i = 0; i < (int)type; i++)
            {
                progress.faceProgresses[i] = new FaceProgress
                {
                    diceFaceLevel = DiceFaceRarity.Wooden,
                    damageMultiplier = 1.0f
                };
            }
            dicesProgress[type] = progress;
        }

        return progress;
    }

}
using Dice.Configs;
using System;
using UnityEngine;

public class EnemyManager
{
    public int BaseCoinsReward { get; private set; }

    public double MaxHealth { get; private set; }
    public double CurrentHealth { get; private set; }

    private EconomyConfig _economy;

    public EnemyManager(int waveIndex, int stageIndex, EconomyConfig economyConfig)
    {
        _economy = economyConfig;

        BaseCoinsReward = GetRewardCoins(waveIndex, stageIndex);
        MaxHealth = GetEnemyHP(waveIndex, stageIndex);
        CurrentHealth = MaxHealth;
    }

    public double GetEnemyHP(int wave, int indexInWave)
    {
        wave = Mathf.Max(1, wave);
        indexInWave = Mathf.Clamp(indexInWave, 1, 10);

        double hpBase = _economy.Hp0 * Math.Pow(1.0 + _economy.growthG, wave - 1);

        if (indexInWave == 10) // босс
            return hpBase * _economy.bossMult * (1.0 + 0.02 * wave);

        double inside = Math.Pow(1.0 + _economy.intraWaveR, indexInWave);
        return hpBase * inside;
    }

    public double GetCoinsByHP(double hp) => _economy.coinsK * Math.Pow(hp, _economy.coinsExp);

    public static int GetRewardCoins(int waveIndex, int enemyIndex)
    {
        int bossBonus = enemyIndex == 9 ? 1 : 3;

        double baseCoins = 2.0;
        double waveGrowth = Mathf.Pow(waveIndex + 1, 1.15f);
        double enemyGrowth = 1.0 + (enemyIndex * 0.2f);

        return Mathf.CeilToInt((float)(baseCoins * waveGrowth * enemyGrowth * bossBonus));
    }

    public void TakeDamage(double amount)
    {
        CurrentHealth = Math.Max(0, CurrentHealth - amount);
    }

    public bool IsDead => CurrentHealth < 0.5f;
}

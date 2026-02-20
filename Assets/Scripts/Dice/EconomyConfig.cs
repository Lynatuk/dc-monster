using System.Collections.Generic;
using UnityEngine;

namespace Dice.Configs
{
    [CreateAssetMenu(fileName = "EconomyConfig", menuName = "Dice/EconomyConfig")]
    public class EconomyConfig : ScriptableObject
    {
        [Header("Damage Growth")]
        [Tooltip("Базовый прирост урона на КАЖДОМ уровне грани, напр. 0.08 = +8%")]
        [Range(0f, 0.5f)] public float baseDmgPerLevel = 0.08f;

        [Tooltip("Параметр 'a' в (1 + a * L^p)")]
        [Range(0f, 1f)] public float damageLevelA = 0.12f;

        [Tooltip("Параметр 'p' в (1 + a * L^p)")]
        [Range(0.5f, 2.0f)] public float damageLevelP = 1.15f;

        [Header("Upgrade Cost")]
        [Tooltip("Коэффициент экспоненты c в стоимости улучшения")]
        [Range(1.0f, 2.0f)] public float upgradeCostExpC = 1.22f;

        [Tooltip("Добавка нелинейности (1 + 0.02 * L^1.2) — множитель")]
        public float upgradeNonlinearK = 0.02f;

        [System.Serializable]
        public struct BaseCostBySides
        {
            public DiceType diceType;
            public float baseCost; // B(S)
        }

        [SerializeField] private List<BaseCostBySides> baseCosts = new();

        public float GetBaseCost(DiceType diceType)
        {
            foreach (var b in baseCosts)
                if (b.diceType == diceType)
                    return Mathf.Max(1f, b.baseCost);
            Debug.LogWarning($"EconomyConfig: no base cost for {diceType}, fallback to 10");
            return 10f;
        }

        [Header("Enemy/Reward (если считаешь тут)")]
        public double Hp0 = 100;      // старт HP первой волны
        public double growthG = 0.25; // рост HP по волне
        public double intraWaveR = 0.08; // рост HP внутри волны (i-й моб)
        public double bossMult = 12.0;  // множитель босса
        [Tooltip("Множитель для награды (монеты)")]
        public double coinsK = 1.0;
        [Tooltip("Степенная зависимость монет от HP: HP^coinsExp")]
        public double coinsExp = 0.90;
    }
}

using System;
using UnityEngine;

namespace Dice.Services
{
    using Dice.Configs;
    using Zenject;

    /// Сервис расчёта урона: применяет базу, рост от уровня, перки, руны/сеты/комбо.
    public class DamageService
    {
        private EconomyConfig _economy;
        private FaceBaseConfig _baseCfg;
        private FacePerkTable _perkTable;

        public System.Random rng;

        [Inject]
        private void Construct(EconomyConfig economy, FaceBaseConfig baseCfg, FacePerkTable perkTable)
        {
            _economy = economy;
            _baseCfg = baseCfg;
            _perkTable = perkTable;

            rng = new System.Random();
        }

        public struct CalcRequest
        {
            public DiceType diceType;
            public DiceFaceRarity faceLevel;
            public int faceIndex;

            public bool rollCrit;

            public float externalStreakBonus;
            public float elementalBonus;

            public IModifierProvider[] providers;
        }

        public struct CalcResult
        {
            public DiceFaceRarity faceLevel;

            public double damage;

            public bool isCrit;
            public float appliedCritChance;
            public float targetCritChance;
            public float appliedCritMultiplier;

            public bool isBoost;
            public double appliedBoostChance;

            public float goldOnHitBonus;

            public float boostChance;
            public float boostBonus;
        }

        public double GetDamageWithoutStats(DiceType diceType, int faceIndex)
        {
            DiceProgress diceProgress = Data.instance.localData.GetOrCreateProgress(diceType);
            int faceLevel = (int)diceProgress.GetFaceProgress(faceIndex).diceFaceLevel;

            double baseDmg = _baseCfg.GetBaseDamage(diceType, faceIndex);

            float l = Mathf.Max(0, faceLevel);
            double perLevelMul = Math.Pow(1.0 + _economy.baseDmgPerLevel, l);
            double nonlinearMul = 1.0 + (_economy.damageLevelA * Math.Pow(l, _economy.damageLevelP));

            double dmg = baseDmg
                       * perLevelMul
                       * nonlinearMul;

            return dmg;
        }

        public double GetDamageWithoutStats(DiceType diceType, int faceIndex, int faceLevel)
        {
            double baseDmg = _baseCfg.GetBaseDamage(diceType, faceIndex);

            float l = Mathf.Max(0, faceLevel);
            double perLevelMul = Math.Pow(1.0 + _economy.baseDmgPerLevel, l);
            double nonlinearMul = 1.0 + (_economy.damageLevelA * Math.Pow(l, _economy.damageLevelP));

            double dmg = baseDmg
                       * perLevelMul
                       * nonlinearMul;

            return dmg;
        }

        public CalcResult Evaluate(in CalcRequest req)
        {
            double baseDmg = _baseCfg.GetBaseDamage(req.diceType, req.faceIndex);

            float l = Mathf.Max(0, (int)req.faceLevel);
            double perLevelMul = Math.Pow(1.0 + _economy.baseDmgPerLevel, l);
            double nonlinearMul = 1.0 + (_economy.damageLevelA * Math.Pow(l, _economy.damageLevelP));

            var acc = _perkTable.GetAccumulated(req.diceType, Mathf.Max(0, (int)req.faceLevel));
            ModifierUtils.Accumulate(req.providers, ref acc);

            float critChance = Mathf.Clamp01(acc.critChance);
            float critMult = acc.critDamageAdd;
            double randCrit = 0;
            bool isCrit = false;
            if (req.rollCrit && rng != null)
            {
                randCrit = rng.NextDouble();
                isCrit = randCrit < critChance;
            }

            float boostChance = Mathf.Clamp01(acc.boostChance);
            double randBoost = 0;
            float boostValue = 1;
            bool isBoost = false;
            if (rng != null)
            {
                randBoost = rng.NextDouble();
                isBoost = randBoost < boostChance;
            }
            if (isBoost)
            {
                boostValue += req.externalStreakBonus;
            }

            double dmg = baseDmg
                       * perLevelMul
                       * nonlinearMul
                       * acc.damageExtraMultiplier
                       * (1.0 + req.elementalBonus);

            if (isCrit)
            {
                dmg *= critMult;
            }

            boostValue += req.externalStreakBonus;
            dmg *= boostValue;

            return new CalcResult
            {
                faceLevel = req.faceLevel,
                damage = dmg,
                isCrit = isCrit,
                appliedCritMultiplier = isCrit ? critMult * 100 : 1f,
                appliedCritChance = MathF.Round((float)randCrit * 100, 1),
                targetCritChance = MathF.Round(critChance * 100, 1),
                goldOnHitBonus = Mathf.Max(0, acc.goldOnHit),
                isBoost = isBoost,
                appliedBoostChance = randBoost,
                boostChance = Mathf.Clamp01(acc.boostChance),
                boostBonus = boostValue
            };
        }
    }
}

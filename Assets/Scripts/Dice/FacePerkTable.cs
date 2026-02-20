using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dice.Configs
{
    [CreateAssetMenu(fileName = "FacePerkTable", menuName = "Dice/FacePerkTable")]
    public class FacePerkTable : ScriptableObject
    {
        [Serializable]
        public struct StatMod
        {
            public FaceStatType stat;
            [Tooltip("В процентах. Например 10 = +10%")]
            public float valuePercent;
        }

        [Serializable]
        public class PerkAtLevel
        {
            public DiceFaceRarity level;
            public List<StatMod> mods = new();
        }

        [Serializable]
        public class Schedule
        {
            [Tooltip("Для какой размерности действует сетка. Если isDefault=true — для всех S, где нет явного оверрайда.")]
            public DiceType diceType;
            public bool isOverride = false;
            public List<PerkAtLevel> entries = new();
        }

        [Header("Дефолтная сетка уровней для S>=6")]
        public List<PerkAtLevel> defaultEntries = new();

        [Header("Оверрайды для конкретных размерностей (например D4)")]
        public List<Schedule> overrides = new();

        /// <summary>
        /// Собирает суммарные модификаторы для данной размерности и уровня L (все пороги <= L).
        /// Возвращает значения в ДОЛЯХ (0.10 = +10%), уже приведённые из процентов.
        /// </summary>
        public AccumulatedPerks GetAccumulated(DiceType diceType, int level)
        {
            var acc = new AccumulatedPerks();
            level = Math.Max(0, level - 1);

            foreach (var e in defaultEntries)
            {
                if ((int)e.level <= level)
                    Apply(acc, e.mods);
            }

            var ov = overrides.Find(o => o.diceType == diceType && o.isOverride);
            if (ov != null)
            {
                acc = new AccumulatedPerks();
                foreach (var e in ov.entries)
                    if ((int)e.level <= level)
                        Apply(acc, e.mods);
                return acc;
            }

            var add = overrides.Find(o => o.diceType == diceType && !o.isOverride);
            if (add != null)
            {
                foreach (var e in add.entries)
                    if ((int)e.level <= level)
                        Apply(acc, e.mods);
            }

            return acc;
        }

        public AccumulatedPerks GetAccumulatedByLevel(DiceType diceType, int level)
        {
            var acc = new AccumulatedPerks();
            level = Math.Max(0, level - 1);

            foreach (var e in defaultEntries)
            {
                if ((int)e.level == level)
                    Apply(acc, e.mods);
            }

            var ov = overrides.Find(o => o.diceType == diceType && o.isOverride);
            if (ov != null)
            {
                acc = new AccumulatedPerks();
                foreach (var e in ov.entries)
                    if ((int)e.level == level)
                        Apply(acc, e.mods);
                return acc;
            }

            var add = overrides.Find(o => o.diceType == diceType && !o.isOverride);
            if (add != null)
            {
                foreach (var e in add.entries)
                    if ((int)e.level == level)
                        Apply(acc, e.mods);
            }

            return acc;
        }

        private void Apply(AccumulatedPerks acc, List<StatMod> mods)
        {
            foreach (var m in mods)
            {
                float v = m.valuePercent / 100f;
                switch (m.stat)
                {
                    case FaceStatType.DamageExtraMultiplierPercent:
                        acc.damageExtraMultiplier += v;
                        break;
                    case FaceStatType.CritChancePercent:
                        acc.critChance += v;
                        break;
                    case FaceStatType.CritDamageAddPercent:
                        acc.critDamageAdd += v;
                        break;
                    case FaceStatType.BoostChancePercent:
                        acc.boostChance += v;
                        break;
                    case FaceStatType.GoldOnHitPercent:
                        acc.goldOnHit += v;
                        break;
                }
            }
        }

        /// <summary>
        /// Возвращает минимальный уровень, на котором ВПЕРВЫЕ появляется заданный стат.
        /// Если стата нет до капа уровня — вернёт -1.
        /// levelCap <= 0 => считается равным числу сторон (int)sides.
        /// </summary>
        public int GetFirstLevelForStat(DiceType sides, FaceStatType stat)
        {
            int cap = (int)sides;
            var entries = GetEffectiveEntriesSorted(sides);

            int first = int.MaxValue;
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if ((int)e.level + 1 > cap)
                    break;

                if (ContainsStat(e.mods, stat))
                    if ((int)e.level < first)
                        first = (int)e.level;
            }
            return first == int.MaxValue ? -1 : first;
        }

        /// <summary>
        /// true, если БОЛЬШЕ не будет улучшений для стата на уровнях (currentLevel; levelCap].
        /// Например: sides=D6, currentLevel=6, след. улучшение на 7 => вернёт true (за капом).
        /// Если таких улучшений вообще нет — тоже вернёт true.
        /// </summary>
        public bool IsStatMaxedAtLevel(DiceType sides, FaceStatType stat, int currentLevel)
        {
            int cap = (int)sides;
            if (currentLevel >= cap)
                return true;

            var entries = GetEffectiveEntriesSorted(sides);
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                if ((int)e.level <= currentLevel)
                    continue;
                if ((int)e.level > cap)
                    break;
                if (ContainsStat(e.mods, stat))
                    return false;
            }
            return true;
        }

        // Собирает «эффективную» сетку перков для данной размерности и сортирует по уровню.
        // Логика: если есть override (isOverride=true) — полностью заменяет defaultEntries;
        // иначе берём defaultEntries и добавляем все не-override для этой размерности.
        private List<PerkAtLevel> GetEffectiveEntriesSorted(DiceType sides)
        {
            var list = new List<PerkAtLevel>();

            var ov = overrides.Find(o => o.diceType == sides && o.isOverride);
            if (ov != null)
            {
                list.AddRange(ov.entries);
            }
            else
            {
                if (defaultEntries != null)
                    list.AddRange(defaultEntries);
                for (int i = 0; i < overrides.Count; i++)
                {
                    var add = overrides[i];
                    if (add.diceType == sides && !add.isOverride && add.entries != null)
                        list.AddRange(add.entries);
                }
            }

            list.Sort((a, b) => a.level.CompareTo(b.level));
            return list;
        }

        private static bool ContainsStat(List<StatMod> mods, FaceStatType stat)
        {
            if (mods == null)
                return false;
            for (int i = 0; i < mods.Count; i++)
                if (mods[i].stat == stat)
                    return true;
            return false;
        }

        [Serializable]
        public class AccumulatedPerks
        {
            public float damageExtraMultiplier = 1; // суммарный доп. множитель урона (как доля)
            public float critChance = 0;            // суммарный шанс крита (доля)
            public float critDamageAdd = 1.5f;      // добавка к крит-множителю (доля)
            public float boostChance = 0;           // шанс буста броска (доля)
            public float goldOnHit = 0;             // доп. % золота (доля)
        }
    }
}

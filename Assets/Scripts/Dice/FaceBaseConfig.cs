// FaceBaseConfig.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dice.Configs
{
    [CreateAssetMenu(fileName = "FaceBaseConfig", menuName = "Dice/FaceBaseConfig")]
    public class FaceBaseConfig : ScriptableObject
    {
        [Serializable]
        public class FaceBaseTable
        {
            public DiceType diceType;
            [Tooltip("БАЗОВЫЙ урон по индексам граней (0..S-1)")]
            public List<double> baseDamageByFace = new();

            public float boostBonusValue;
        }

        [Header("Таблицы базового урона по размерности")]
        public List<FaceBaseTable> tables = new();

        [Header("Джиттер (детерминированный)")]
        public int jitterSeed = 1337;
        [Tooltip("Амплитуда джиттера, например 0.05 = ±5%")]
        [Range(0f, 0.2f)] public float jitterAmplitude = 0.05f;

        public double GetBaseDamage(DiceType diceType, int faceIndex)
        {
            var t = tables.Find(x => x.diceType == diceType);
            if (t == null || t.baseDamageByFace == null || t.baseDamageByFace.Count <= faceIndex)
            {
                Debug.LogError($"FaceBaseConfig: no base damage for {diceType} face {faceIndex}");
                return 1.0;
            }

            double baseDmg = t.baseDamageByFace[faceIndex];
            float j = Jitter01(jitterSeed, (int)diceType, faceIndex);
            float signed = (j * 2f - 1f) * jitterAmplitude;
            return baseDmg * (1.0 + signed);
        }

        public float GetBoostBonus(DiceType diceType)
        {
            return tables.Find(x => x.diceType == diceType).boostBonusValue;
        }

        private static float Jitter01(int seed, int a, int b)
        {
            unchecked
            {
                int h = seed;
                h = h * 31 + a;
                h = h * 31 + b;
                uint x = (uint)h;
                x ^= x << 13;
                x ^= x >> 17;
                x ^= x << 5;
                return (x & 0x00FFFFFF) / (float)0x01000000;
            }
        }
    }
}

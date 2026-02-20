using UnityEngine;
using TMPro;
using System;
using Dice.Configs;
using Dice.Services;

public class StatsPanelController : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI statsAddText;
    public TextMeshProUGUI lockLevelText;

    private DamageService _damage;
    private FacePerkTable _perkTable;

    private DiceType _diceType;
    private int _faceIndex;

    public void SetupStats(StatsPanelData statsPanelData, FaceStatType faceStatType)
    {
        _damage = statsPanelData._damage;
        _perkTable = statsPanelData._perkTable;
        _diceType = statsPanelData.diceType;
        _faceIndex = statsPanelData.faceIndex;

        int openLevelStat = _perkTable.GetFirstLevelForStat(_diceType, faceStatType);
        bool noMoreUpgrade = _perkTable.IsStatMaxedAtLevel(_diceType, faceStatType, statsPanelData.currentLevel-1) || statsPanelData.currentLevel >= (int)_diceType;
        
        if (openLevelStat == -1)
        {
            titleText.gameObject.SetActive(false);
            statsText.SetText("");
            statsAddText.SetText("");
            lockLevelText.SetText("");
            return;
        }

        if (openLevelStat <= statsPanelData.currentLevel)
        {
            switch (faceStatType)
            {
                case FaceStatType.DamageExtraMultiplierPercent:
                    SetupDamageStat(statsPanelData.currentLevel, noMoreUpgrade);
                    break;
                case FaceStatType.CritChancePercent:
                    SetupCritChance(statsPanelData.currentLevel, noMoreUpgrade);
                    break;
                case FaceStatType.CritDamageAddPercent:
                    SetupCritDamage(statsPanelData.currentLevel, noMoreUpgrade);
                    break;
                case FaceStatType.BoostChancePercent:
                    SetupBoostChance(statsPanelData.currentLevel, noMoreUpgrade);
                    break;
                case FaceStatType.GoldOnHitPercent:
                    SetupGoldOnHit(statsPanelData.currentLevel, noMoreUpgrade);
                    break;
                default:
                    break;
            }
            titleText.gameObject.SetActive(true);
        }
        else
        {
            lockLevelText.SetText("Ур. {0}", openLevelStat + 1);
            statsAddText.SetText("");
            titleText.gameObject.SetActive(false);
        }
    }

    public void SetupDamageStat(int currentLevel, bool noMoreBoost)
    {

        double damage = _damage.GetDamageWithoutStats(_diceType, _faceIndex, currentLevel);
        float damageMultiplier = _perkTable.GetAccumulated(_diceType, currentLevel).damageExtraMultiplier;
        string currentDamage = BigNumberFormatter.Format(damage * damageMultiplier);

        statsAddText.gameObject.SetActive(true);
        if (noMoreBoost)
        {
            statsText.SetText(currentDamage);
            statsAddText.SetText("<size=33><color=#86FF88>(max)</color>");
        }
        else
        {
            double damageNextLevel = _damage.GetDamageWithoutStats(_diceType, _faceIndex, currentLevel + 1);
            float nextDamageMultiplier = _perkTable.GetAccumulated(_diceType, currentLevel + 1).damageExtraMultiplier;
            string nextDamage = BigNumberFormatter.Format(damageNextLevel * nextDamageMultiplier);

            int bonusMultiplier = (int)((_perkTable.GetAccumulatedByLevel(_diceType, currentLevel + 1).damageExtraMultiplier - 1) * 100);

            statsText.SetText($"<size=90%>{currentDamage}</size> » <color=#86FF88>{nextDamage}</color>");
            statsAddText.SetText("<size=32><color=#FFF586>(+{0}%)</color>", bonusMultiplier);
        }
    }

    public void SetupCritChance(int currentLevel, bool noMoreBoost)
    {
        float critChance = _perkTable.GetAccumulated(_diceType, currentLevel).critChance * 100;

        if (noMoreBoost)
        {
            statsText.SetText("{0}%", (int)critChance);
            statsAddText.gameObject.SetActive(true);
            statsAddText.SetText("<size=33><color=#86FF88>(max)</color>");
        }
        else
        {
            float bonusMultiplier = _perkTable.GetAccumulatedByLevel(_diceType, currentLevel + 1).critChance * 100;

            if (bonusMultiplier > 0)
            {
                float nextCritChance = critChance + bonusMultiplier;

                statsText.SetText("<size=90%>{0}%</size> » <color=#86FF88>{1}%</color>", critChance, nextCritChance);
                statsAddText.gameObject.SetActive(true);
                statsAddText.SetText("<size=32><color=#FFF586>(+{0}%)</color>", (int)bonusMultiplier);
            }
            else
            {
                statsText.SetText("{0}%", (int)critChance);
                statsAddText.gameObject.SetActive(false);
            }
        }
    }

    public void SetupCritDamage(int currentLevel, bool noMoreBoost)
    {
        float critChance = _perkTable.GetAccumulated(_diceType, currentLevel).critDamageAdd * 100;

        if (noMoreBoost)
        {
            statsText.SetText("{0}%", (int)critChance);
            statsAddText.gameObject.SetActive(true);
            statsAddText.SetText("<size=33><color=#86FF88>(max)</color>");
        }
        else
        {
            float bonusMultiplier = (_perkTable.GetAccumulatedByLevel(_diceType, currentLevel + 1).critDamageAdd - 1.5f) * 100;

            if (bonusMultiplier > 0)
            {
                float nextCritChance = critChance + bonusMultiplier;

                statsText.SetText("<size=90%>{0}%</size> » <color=#86FF88>{1}%</color>", critChance, nextCritChance);
                statsAddText.gameObject.SetActive(true);
                statsAddText.SetText("<size=32><color=#FFF586>(+{0}%)</color>", (int)bonusMultiplier);
            }
            else
            {
                statsText.SetText("{0}%", (int)critChance);
                statsAddText.gameObject.SetActive(false);
            }
        }
    }

    public void SetupBoostChance(int currentLevel, bool noMoreBoost)
    {
        float critChance = _perkTable.GetAccumulated(_diceType, currentLevel).boostChance * 100;

        if (noMoreBoost)
        {
            statsText.SetText("{0}%", (int)critChance);
            statsAddText.gameObject.SetActive(true);
            statsAddText.SetText("<size=33><color=#86FF88>(max)</color>");
        }
        else
        {
            float bonusMultiplier = _perkTable.GetAccumulatedByLevel(_diceType, currentLevel + 1).boostChance * 100;

            if (bonusMultiplier > 0)
            {
                float nextCritChance = critChance + bonusMultiplier;

                statsText.SetText("<size=90%>{0}%</size> » <color=#86FF88>{1}%</color>", critChance, nextCritChance);
                statsAddText.gameObject.SetActive(true);
                statsAddText.SetText("<size=32><color=#FFF586>(+{0}%)</color>", (int)bonusMultiplier);
            }
            else
            {
                statsText.SetText("{0}%", (int)critChance);
                statsAddText.gameObject.SetActive(false);
            }
        }
    }

    public void SetupGoldOnHit(int currentLevel, bool noMoreBoost)
    {
        float critChance = _perkTable.GetAccumulated(_diceType, currentLevel).goldOnHit * 100;

        if (noMoreBoost)
        {
            statsText.SetText("{0}%", (int)critChance);
            statsAddText.gameObject.SetActive(true);
            statsAddText.SetText("<size=33><color=#86FF88>(max)</color>");
        }
        else
        {
            float bonusMultiplier = _perkTable.GetAccumulatedByLevel(_diceType, currentLevel + 1).goldOnHit * 100;

            if (bonusMultiplier > 0)
            {
                float nextCritChance = critChance + bonusMultiplier;

                statsText.SetText("<size=90%>{0}%</size> » <color=#86FF88>{1}%</color>", critChance, nextCritChance);
                statsAddText.gameObject.SetActive(true);
                statsAddText.SetText("<size=32><color=#FFF586>(+{0}%)</color>", (int)bonusMultiplier);
            }
            else
            {
                statsText.SetText("{0}%", (int)critChance);
                statsAddText.gameObject.SetActive(false);
            }
        }
    }

}

[Serializable]
public class StatsPanelData
{
    public DamageService _damage; 
    public FacePerkTable _perkTable; 
    public DiceType diceType; 
    public int faceIndex; 
    public int currentLevel;
}
using Cysharp.Threading.Tasks;
using Dice.Configs;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using Zenject;
using static Dice.Services.DamageService;

public class AttackInfoController : MonoBehaviour
{
    public AttackInfoUI attackInfoUI;

    public List<PanelInfoStyle> panelInfoStyles;

    private List<AttackPanelData> _attackPanelDatas = new();

    private CalcResult _resultData;

    private FacePerkTable _perkTable;

    private string _titleGold;
    private string _titleDamage;
    private string _titleCritChance;
    private string _titleCritDamage;
    private string _titleBoostChance;

    [Inject]
    private void Construct(FacePerkTable perkTable)
    {
        _perkTable = perkTable;

        OnLocaleChanged(null);
    }

    public void HidePanels()
    {
        attackInfoUI.HidePanels();
        _attackPanelDatas = new List<AttackPanelData>();
    }

    public async UniTask SetDamageInfo(CalcResult damageResult)
    {
        _resultData = damageResult;

        if (damageResult.damage > 0)
        {
            SetDamage();
        }

        if (damageResult.targetCritChance > 0)
        {
            SetCritChance();
        }

        if (damageResult.isCrit && damageResult.appliedCritMultiplier > 0)
        {
            SetCritBonus();
        }

        if (damageResult.boostBonus > 1)
        {
            SetBoostChance();
        }

        await attackInfoUI.SetInfoPanels(_attackPanelDatas);
    }

    public void SetGoldEarnCount(string value)
    {
        PanelInfoStyle panelInfoStyle = panelInfoStyles.Find(info => info.attackInfoType == AttackInfoType.GoldEarn);

        AttackPanelData attackPanelData = new();
        attackPanelData.icon = panelInfoStyle.icon;
        attackPanelData.colorPanel = panelInfoStyle.baseColorPanel;

        attackPanelData.name = _titleGold;
        attackPanelData.value = $"{value}<sprite name=coin>";

        _attackPanelDatas.Add(attackPanelData);
    }

    public void SetDamage()
    {
        PanelInfoStyle panelInfoStyle = panelInfoStyles.Find(info => info.attackInfoType == AttackInfoType.Damage);

        AttackPanelData attackPanelData = new();
        attackPanelData.icon = panelInfoStyle.icon;
        attackPanelData.colorPanel = panelInfoStyle.baseColorPanel;

        attackPanelData.name = _titleDamage;
        attackPanelData.value = BigNumberFormatter.Format(_resultData.damage);

        _attackPanelDatas.Add(attackPanelData);
    }

    public void SetCritChance()
    {
        PanelInfoStyle panelInfoStyle = panelInfoStyles.Find(info => info.attackInfoType == AttackInfoType.CritChance);

        AttackPanelData attackPanelData = new();
        attackPanelData.icon = panelInfoStyle.icon;

        attackPanelData.name = _titleCritChance;

        if (!_resultData.isCrit)
        {
            attackPanelData.colorPanel = panelInfoStyle.unluckColorPanel;
            attackPanelData.value = $"<color=#FF4D4C>{_resultData.appliedCritChance}% <size=80%>></color> <color=#42FF57>{_resultData.targetCritChance}%</size></color>";
        }
        else
        {
            attackPanelData.colorPanel = panelInfoStyle.luckColorPanel;
            attackPanelData.value = $"<color=#42FF57>{_resultData.appliedCritChance}%</color> <color=#FF4D4C><size=80%>> {_resultData.targetCritChance}%</size></color>";
        }

        _attackPanelDatas.Add(attackPanelData);
    }

    public void SetCritBonus()
    {
        PanelInfoStyle panelInfoStyle = panelInfoStyles.Find(info => info.attackInfoType == AttackInfoType.CritDamage);

        AttackPanelData attackPanelData = new();
        attackPanelData.icon = panelInfoStyle.icon;
        attackPanelData.colorPanel = panelInfoStyle.baseColorPanel;

        attackPanelData.name = _titleCritDamage;
        attackPanelData.value = $"+{_resultData.appliedCritMultiplier}%";

        _attackPanelDatas.Add(attackPanelData);
    }

    public void SetBoostChance()
    {
        PanelInfoStyle panelInfoStyle = panelInfoStyles.Find(info => info.attackInfoType == AttackInfoType.BoostChance);

        AttackPanelData attackPanelData = new();
        attackPanelData.icon = panelInfoStyle.icon;

        attackPanelData.name = _titleBoostChance;

        attackPanelData.colorPanel = panelInfoStyle.luckColorPanel;
        attackPanelData.value = $"<color=#42FF57>x{_resultData.boostBonus}</color>";

        _attackPanelDatas.Add(attackPanelData);
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale _)
    {
        _titleGold = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "coins_earning");
        _titleDamage = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "damage");
        _titleCritChance = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "critical_chance");
        _titleCritDamage = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "critical_bonus");
        _titleBoostChance = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "boost_bonus");
    }

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    [Serializable]
    public class PanelInfoStyle
    {
        public AttackInfoType attackInfoType;
        public Sprite icon;

        public Color baseColorPanel;

        [Header("For Chances")]
        public Color luckColorPanel;
        public Color unluckColorPanel;
    }
}

[Serializable]
public class AttackPanelData
{
    public Sprite icon;

    public Color colorPanel;

    public string name;
    public string value;
}
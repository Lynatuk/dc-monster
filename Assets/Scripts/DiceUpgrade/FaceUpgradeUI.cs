using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Dice.Configs;
using Dice.Services;

public class FaceUpgradeView : MonoBehaviour
{

    public Image faceIcon;

    public Slider progressSlider;

    public TextMeshProUGUI diceText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI priceText;
    public TextMeshProUGUI maxLevelText;

    [Header("Stats")]
    public StatsPanelController damageStatsPanel;
    public StatsPanelController critChanceStatsPanel;
    public StatsPanelController critDamageStatsPanel;
    public StatsPanelController boostChanceStatsPanel;
    public StatsPanelController goldBonusStatsPanel;

    public Button upgradeButton;

    private int _maxLevel;
    private int _price;

    private bool _canBeImproved;

    private StatsPanelData _statsPanelData;

    private void OnEnable()
    {
        CoinsController.updateCoins += SetInteractable;
    }

    private void OnDisable()
    {
        CoinsController.updateCoins -= SetInteractable;
    }

    public void Setup(DamageService damageService, FacePerkTable perkTable, DiceType diceType, int faceIndex)
    {
        _maxLevel = (int)diceType;
        _statsPanelData = new StatsPanelData { _damage = damageService,_perkTable = perkTable, diceType = diceType, faceIndex = faceIndex };
    }

    public void SetupStats(int currentLevel)
    {
        _statsPanelData.currentLevel = currentLevel;

        damageStatsPanel.SetupStats(_statsPanelData, FaceStatType.DamageExtraMultiplierPercent);
        critChanceStatsPanel.SetupStats(_statsPanelData, FaceStatType.CritChancePercent);
        critDamageStatsPanel.SetupStats(_statsPanelData, FaceStatType.CritDamageAddPercent);
        boostChanceStatsPanel.SetupStats(_statsPanelData, FaceStatType.BoostChancePercent);
        goldBonusStatsPanel.SetupStats(_statsPanelData, FaceStatType.GoldOnHitPercent);

        SetupMaxLevelPanel();
    }

    private void SetupMaxLevelPanel()
    {
        bool isMaxLevel = _statsPanelData.currentLevel == _maxLevel;
        priceText.gameObject.SetActive(!isMaxLevel);
        maxLevelText.gameObject.SetActive(isMaxLevel);
    }

    public void SetFaceIcon(Sprite icon, int diceNumb)
    { 
        faceIcon.sprite = icon;
        diceText.SetText("{0}", diceNumb);
    }

    public void SetProgress(int current, int max)
    {
        progressText.SetText("{0}/{1}", current, max);
        progressSlider.value = (float)current / max;

        _canBeImproved = current < max;
    }

    public void SetPrice(int price) 
    {
        this._price = price;

        priceText.SetText("{0}", price);
    }

    public void SetInteractable()
    {
        bool isEnoughCoins = _price <= Data.instance.localData.coins;

        upgradeButton.interactable = _canBeImproved && isEnoughCoins;
    }

    public void SetOnUpgradeClick(Action callback)
    {
        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(() => callback.Invoke());
    }
}
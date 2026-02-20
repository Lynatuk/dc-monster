using Dice.Configs;
using Dice.Services;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UpgradeWindowController : MonoBehaviour
{
    [SerializeField] private UpgradeWindowUI upgradeWindowUI;
    [SerializeField] private DiceData diceData;

    private DiceType currentType;
    private List<FaceUpgradeView> facePanels = new();

    private DiceProgress diceProgress;

    private DamageService _damage;
    private DiceUpgrade _diceUpgrade;
    private FacePerkTable _perkTable;

    [Inject]
    private void Construct(DamageService damageService, DiceUpgrade diceUpgrade, FacePerkTable perkTable)
    {
        _damage = damageService;
        _diceUpgrade = diceUpgrade;
        _perkTable = perkTable;
    }

    private void Start()
    {
        currentType = Data.Instance.localData.currentNumericalDice;

        upgradeWindowUI.SetOnPrevClick(OnPrevClicked);
        upgradeWindowUI.SetOnNextClick(OnNextClicked);

        SetupWindow();

        DiceController.UpdateDice?.Invoke(diceData);
    }

    private void SetupWindow()
    {
        upgradeWindowUI.ClearFaces();
        facePanels.Clear();

        int faceCount = (int)currentType;
        int totalLevel = _diceUpgrade.GetCurrentCountUpgrades(currentType);

        for (int i = 0; i < faceCount; i++)
        {
            var facePanel = CreateFacePanel(i);
            facePanels.Add(facePanel);
        }

        upgradeWindowUI.progressSliderController.SetProgress(totalLevel, faceCount * faceCount);
        upgradeWindowUI.SetNavigationInteractable(currentType > DiceType.Numerical4, currentType < DiceType.Numerical20);
    }

    private FaceUpgradeView CreateFacePanel(int index)
    {
        diceProgress = Data.Instance.localData.GetOrCreateProgress(currentType);
        FaceProgress faceProgress = diceProgress.GetFaceProgress(index);
        DiceFaceRarity rarity = faceProgress.diceFaceLevel;
        double damage = _damage.GetDamageWithoutStats(currentType, index);
        int level = (int)rarity;
        int faceCount = (int)currentType;
        int price = _diceUpgrade.GetUpgradeCost(index, level, currentType);

        FaceUpgradeView panel = upgradeWindowUI.CreateFacePanel();
        panel.Setup(_damage, _perkTable, currentType, index);
        panel.SetFaceIcon(diceData.GetNumericalDiceInfo(rarity).faceSprite, index + 1);
        panel.SetupStats(level);
        panel.SetProgress(level, faceCount);
        panel.SetPrice(price);
        panel.SetInteractable();
        panel.SetOnUpgradeClick(() => OnUpgradeClicked(index));
        return panel;
    }

    private void UpdateFacePanel(int index)
    {
        var panel = facePanels[index];
        diceProgress = Data.Instance.localData.GetOrCreateProgress(currentType);
        var progress = diceProgress.GetFaceProgress(index);
        int level = (int)progress.diceFaceLevel;
        int faceCount = (int)currentType;

        panel.SetFaceIcon(diceData.GetNumericalDiceInfo(progress.diceFaceLevel).faceSprite, index + 1);
        panel.SetupStats(level);
        panel.SetProgress(level, faceCount);
        panel.SetPrice(_diceUpgrade.GetUpgradeCost(index, level, currentType));
        panel.SetInteractable();
    }

    private void RecalculateOverallProgress()
    {
        var faceCount = (int)currentType;
        int totalLevel = _diceUpgrade.GetCurrentCountUpgrades(currentType);

        upgradeWindowUI.progressSliderController.UpdateProgress(totalLevel, faceCount * faceCount);
    }

    private void OnPrevClicked()
    {
        if (currentType > DiceType.Numerical4)
        {
            currentType--;
            SetupWindow();
        }
    }

    private void OnNextClicked()
    {
        if (currentType < DiceType.Numerical20)
        {
            currentType++;
            SetupWindow();
        }
    }

    private void OnUpgradeClicked(int faceIndex)
    {
        if (_diceUpgrade.TryUpgrade(currentType, faceIndex, diceData))
        {
            UpdateFacePanel(faceIndex);
            RecalculateOverallProgress();
        }
    }
}
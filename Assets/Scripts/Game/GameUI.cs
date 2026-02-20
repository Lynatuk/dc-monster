using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    public TextMeshProUGUI waveNumberText;

    public ProgressSliderController progressSliderController;

    [Header("UpgradeShopButton")]
    public Button upgradeShopButton;

    [Header("AttackButton")]
    public Button attackButton;
    public Image delayFillButton;

    [Header("AutoAttack")]
    public Button autoAttackButton;
    public SpriteFontsIndexSwaps autoAttackSpSwap;

    [Header("AttackSpeed")]
    public Button attackSpeedButton;
    public SpriteFontsIndexSwaps attackSpeedSpSwap;

    public void Setup()
    {
        autoAttackSpSwap.SetState(0);
        attackSpeedSpSwap.SetState(0);
    }

    public void SetupSliderProgress(int currentProgress, int targetProgress)
    {
        progressSliderController.SetProgress(currentProgress, targetProgress);
    }

    public void UpdateSliderProgress(int currentProgress, int targetProgress)
    {
        progressSliderController.SetProgress(currentProgress, targetProgress);
    }

    public void UpdateDelayFillButton(float value)
    {
        delayFillButton.fillAmount = value;
    }

    public void SetAutoAttackState(bool isActive)
    {
        if (isActive)
        {
            autoAttackSpSwap.SetState(1);
        }
        else
        {
            autoAttackSpSwap.SetState(0);
        }
    }

    public void SetSpeedAttackState(bool isActive)
    {
        if (isActive)
        {
            attackSpeedSpSwap.SetState(1);
        }
        else
        {
            attackSpeedSpSwap.SetState(0);
        }
    }

}

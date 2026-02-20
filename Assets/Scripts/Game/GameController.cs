using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using Zenject;
using static Dice.Services.DamageService;
using UnityEditor.Localization.Editor;
using UnityEngine.Localization.Settings;

public class GameController : MonoBehaviour
{

    public DicesController dicesController;
    public BoostAttackController boostAttackController;
    public ComboController comboController;
    public WaveController waveController;
    public AttackInfoController attackInfoController;

    public GameUI gameUI;

    [SerializeField] private float attackSpeed = 1f;

    private bool isAttackProcces = false;
    private bool isAutoAttackEnabled = false;
    private bool isSpeedAttackEnabled = false;

    private double lastAttack = 0;

    private CancellationTokenSource ctsAutoAtackProcess;

    private DiceUpgrade _diceUpgrade;

    [Inject]
    private void Construct(DiceUpgrade diceUpgrade)
    {
        _diceUpgrade = diceUpgrade;
    }

    private void OnEnable()
    {
        DiceController.UpdateDice += (s)=> UpdateProgressDiceSlider(false);
    }

    private void Start()
    {
        dicesController.Init();
        waveController.Init();
        comboController.Init();
        
        gameUI.Setup();

        SetupButtonEvents();

        UpdateProgressDiceSlider(true);
    }

    private void SetupButtonEvents()
    {
        gameUI.attackButton.onClick.AddListener(() => DiceAttack().Forget());
        gameUI.autoAttackButton.onClick.AddListener(() => StartStopAutoAttack());
        gameUI.attackSpeedButton.onClick.AddListener(() => StartStopSpeedAttack());
        gameUI.upgradeShopButton.onClick.AddListener(() => OpenUpgradeShop());
    }

    private void OpenUpgradeShop()
    {
        Navigation.ShowWindow(ScenesData.DiceUpgrade);
    }

    private void StartStopSpeedAttack() // todo вынести в отдельный скрипт
    {
        if (!isSpeedAttackEnabled)
        {
            isSpeedAttackEnabled = true;
            attackSpeed = 0.5f;
        }
        else
        {
            isSpeedAttackEnabled = false;
            attackSpeed = 1f;
        }

        gameUI.SetSpeedAttackState(isSpeedAttackEnabled);
    }

    private void StartStopAutoAttack() // todo вынести в отдельный скрипт
    {
        if (!isAutoAttackEnabled && !isAttackProcces)
        {
            isAutoAttackEnabled = true;
            AutoAtackProcess().Forget();
        }
        else
        {
            isAutoAttackEnabled = false;
            ctsAutoAtackProcess?.Cancel();
            ctsAutoAtackProcess?.Dispose();
        }

        gameUI.SetAutoAttackState(isAutoAttackEnabled);
    }

    private async UniTaskVoid AutoAtackProcess()
    {
        ctsAutoAtackProcess = new CancellationTokenSource();

        while (isAutoAttackEnabled && !ctsAutoAtackProcess.IsCancellationRequested)
        {
            await DiceAttack();
        }
    }

    public async UniTask DiceAttack()
    {
        int numericalDiceFace;

        if (!isAttackProcces)
        {
            isAttackProcces = true;

            AnimateAttackFill().Forget();
            gameUI.attackButton.interactable = false;

            numericalDiceFace = await dicesController.RollNumerical(attackSpeed);

            if (numericalDiceFace >= 0)
            {
                attackInfoController.HidePanels();

                CalcResult damageResult = dicesController.GetNumericalDamageAttack(numericalDiceFace);

                if (damageResult.isCrit)
                {
                    FxGameController.spawnPopup(AttackInfoType.CritDamage, LocalizationSettings.StringDatabase.GetLocalizedString("UI", "crit"));
                }
                FxGameController.spawnPopup(AttackInfoType.Damage, BigNumberFormatter.Format(damageResult.damage));
                HitEnemyDamage(damageResult.damage);

                boostAttackController.SetBoost(damageResult.boostBonus);
                comboController.SetupFace(numericalDiceFace, dicesController.diceData.GetNumericalDiceInfo(damageResult.faceLevel));

                await attackInfoController.SetDamageInfo(damageResult);
            }

            gameUI.attackButton.interactable = true;
            isAttackProcces = false;
        }
    }

    private void HitEnemyDamage(double damage)
    {
        IAttackable target = waveController.CurrentEnemy;
        target?.TakeDamage(damage, deathEnemyData => DeathEnemyEvent(deathEnemyData));
    }

    private void DeathEnemyEvent(DeathEnemyData deathEnemyData)
    {
        attackInfoController.SetGoldEarnCount(deathEnemyData.GoldReward.ToString());
    }

    private void UpdateProgressDiceSlider(bool isSetup)
    {
        DiceType diceType = dicesController.GetCurrentDiceType();
        var faceCount = (int)diceType;
        int currentLevel = _diceUpgrade.GetCurrentCountUpgrades(diceType);

        if (isSetup)
        {
            gameUI.SetupSliderProgress(currentLevel, faceCount * faceCount);
        }
        else
        {
            gameUI.UpdateSliderProgress(currentLevel, faceCount * faceCount);
        }
    }

    public async UniTask AnimateAttackFill()
    {
        float currentValue;
        float elapsed = 0f;

        while (elapsed < 0.15f)
        {
            elapsed += Time.deltaTime;
            currentValue = Mathf.Lerp(0, 1, elapsed / 0.1f);
            gameUI.UpdateDelayFillButton(currentValue);
            await UniTask.Yield();
        }

        elapsed = 0f;
        while (elapsed < attackSpeed)
        {
            elapsed += Time.deltaTime;
            currentValue = Mathf.Lerp(1f, 0, elapsed / attackSpeed);
            gameUI.UpdateDelayFillButton(currentValue);
            await UniTask.Yield();
        }

        gameUI.UpdateDelayFillButton(0);
    }

    private void OnDisable()
    {
        DiceController.UpdateDice -= (s) => UpdateProgressDiceSlider(false);
    }

}

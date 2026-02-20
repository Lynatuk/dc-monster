using Cysharp.Threading.Tasks;
using Dice.Configs;
using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour, IAttackable
{
    public event Action<EnemyController> OnEnemyDied;

    [SerializeField] private HealthBarController healthBarController;

    [SerializeField] private EnemyUI enemyUI;

    private EnemyManager _enemyManager;
    private EnemyData _enemyData;

    private DeathEnemyData _deathEnemyData;

    private double _maxHealth;

    public void Setup(EnemyData data, EconomyConfig economyConfig)
    {
        _enemyData = data;
        _enemyManager = new EnemyManager(Data.instance.localData.currentWave, Data.instance.localData.currentStage, economyConfig);

        _maxHealth = _enemyManager.MaxHealth;
        healthBarController.Setup(_maxHealth);

        enemyUI.Setup();
    }

    public void TakeDamage(double amount, UnityAction<DeathEnemyData> dieEvents)
    {
        if (IsDead)
            return;

        _enemyManager.TakeDamage(amount);
        UpdateHealth(_enemyManager.CurrentHealth);

        if (_enemyManager.IsDead)
        {
            Die().Forget();

            _deathEnemyData.GoldReward = GetBaseCoinsReward();
            dieEvents?.Invoke(_deathEnemyData);
        }
        else
        {
            enemyUI.PlayHit();
        }
    }

    public void UpdateHealth(double currentHealth)
    {
        double progress = currentHealth / _maxHealth;
        float targetValue = Mathf.Clamp01((float)progress);

        healthBarController.UpdateHealth(targetValue);

        healthBarController.UpdateHealthCount(currentHealth);
    }

    private async UniTaskVoid Die()
    {
        DeathReward();

        await enemyUI.PlayDeath();

        OnEnemyDied?.Invoke(this);
        Destroy(gameObject);
    }

    private void DeathReward()
    {
        int coinsReward = GetBaseCoinsReward();
        FxGameController.spawnPopup(AttackInfoType.GoldEarn, BigNumberFormatter.Format(coinsReward));
        Data.Instance.localData.coins += coinsReward;
    }

    public int GetBaseCoinsReward() => _enemyManager.BaseCoinsReward;

    public bool IsDead => _enemyManager?.IsDead ?? true;
}

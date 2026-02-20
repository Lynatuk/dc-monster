using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using Dice.Configs;
using Zenject;

public class WaveController : MonoBehaviour
{
    [SerializeField] private WavePanelController wavePanelController;

    [SerializeField] private CoinRewardManager coinRewardManager;

    [SerializeField] private List<WaveData> waveConfigs;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private WaveUI waveUI;

    private WaveManager _waveManager;

    private readonly EnemyTracker _tracker = new();

    private CancellationTokenSource _bossTimerCts;
    private float _bossTimerRemaining;
    private bool _isBossTimerActive;

    private EconomyConfig _economy;

    private int _waveIndex;
    private int CurrentWaveIndex
    {
        get => _waveIndex;
        set
        {
            _waveIndex = value;
            Data.Instance.localData.currentWave = _waveIndex;
        }
    }

    private int _stageIndex;
    private int CurrentStageIndex
    {
        get => _stageIndex;
        set {
            _stageIndex = value;
            Data.Instance.localData.currentStage = _stageIndex;
        }
    }

    [Inject]
    private void Construct(EconomyConfig economy)
    {
        _economy = economy;
    }

    public void Init()
    {
        CurrentWaveIndex = Data.Instance.localData.currentWave;
        CurrentStageIndex = Data.Instance.localData.currentStage;

        _waveManager = new WaveManager(waveConfigs, CurrentWaveIndex);

        wavePanelController.Init(CurrentWaveIndex, CurrentStageIndex);
        waveUI.Setup(CurrentWaveIndex, CurrentStageIndex);

        SpawnNextEnemy().Forget();
    }

    private async UniTaskVoid SpawnNextEnemy()
    {
        if (!_waveManager.HasNextWave)
            return;

        WaveData currentWave = _waveManager.GetCurrentWave();

        if (CurrentStageIndex >= currentWave.StageCount)
        {
            _waveManager.MoveToNextWave();
            CurrentStageIndex = 0;
            CurrentWaveIndex++;

            wavePanelController.SetWaveNumber(CurrentWaveIndex, CurrentStageIndex);

            if (!_waveManager.HasNextWave)
            {
                Debug.Log("Все волны завершены");
                return;
            }

            currentWave = _waveManager.GetCurrentWave();
        }

        EnemyData enemyData = currentWave.GetEnemyForStage(CurrentStageIndex);
        EnemyController enemy = SpawnEnemy(enemyData);

        _tracker.Register(enemy);
        wavePanelController.UpdateStage(CurrentStageIndex);

        if (currentWave.IsBossIndex(CurrentStageIndex))
        {
            StartBossTimer(enemy).Forget();
            waveUI.ShowBossPanel();
            waveUI.ShowBossTimer();
        }

        enemy.OnEnemyDied += (e) =>
        {
            _tracker.Clear();
            StopBossTimer();

            CoinRewardManager.Instance.SpawnCoins(transform.position, enemy.GetBaseCoinsReward());

            CurrentStageIndex++;
            SpawnNextEnemy().Forget();
        };
    }

    private EnemyController SpawnEnemy(EnemyData data)
    {
        var instance = Instantiate(data.monsterPrefab, Vector3.zero, Quaternion.identity, spawnPoint);
        instance.transform.localPosition = Vector3.zero;
        instance.Setup(data, _economy);
        return instance;
    }

    private async UniTaskVoid StartBossTimer(EnemyController boss)
    {
        _bossTimerCts = new CancellationTokenSource();
        _isBossTimerActive = true;
        _bossTimerRemaining = waveUI.GetBossTimerDuration();

        try
        {
            while (_bossTimerRemaining > 0 && !boss.IsDead)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: _bossTimerCts.Token);
                
                if (_isBossTimerActive)
                {
                    _bossTimerRemaining -= 0.1f;
                    waveUI.UpdateTimer(_bossTimerRemaining);
                }
            }

            if (!boss.IsDead && _bossTimerRemaining <= 0)
            {
                Debug.Log("Босс не убит — рестарт волны");

                Destroy(boss.gameObject);
                _tracker.Clear();

                CurrentStageIndex = 0;
                wavePanelController.Init(CurrentWaveIndex, CurrentStageIndex);

                await UniTask.Delay(500);

                SpawnNextEnemy().Forget();
            }
        }
        catch (OperationCanceledException) { /* Убит вовремя */ }
        finally
        {
            _isBossTimerActive = false;
            waveUI.HideBossTimer();
        }
    }

    private void StopBossTimer()
    {
        _isBossTimerActive = false;
        _bossTimerCts?.Cancel();
        waveUI.HideBossTimer();
    }

    public EnemyController CurrentEnemy => _tracker.CurrentEnemy;
}

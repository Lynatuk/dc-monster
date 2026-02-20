using System.Collections.Generic;

public class WaveManager
{
    private readonly List<WaveData> _waves;

    private int _currentWaveIndex;

    public WaveManager(List<WaveData> waves, int waveIndex)
    {
        _waves = waves;
        _currentWaveIndex = waveIndex;
    }

    public void MoveToNextWave() => _currentWaveIndex++;

    public bool HasNextWave => _currentWaveIndex < _waves.Count;

    public WaveData GetCurrentWave() => _waves[_currentWaveIndex];
}

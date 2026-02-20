using UnityEngine;

[CreateAssetMenu(menuName = "Wave/StageBasedWave")]
public class WaveData : ScriptableObject
{
    [Tooltip("Всего должно быть 10 врагов, последний — босс")]
    public EnemyData[] stageEnemies = new EnemyData[10];

    public EnemyData GetEnemyForStage(int index)
    {
        if (index < 0 || index >= stageEnemies.Length)
        {
            Debug.LogError($"Неверный индекс стадии врага: {index}");
            return null;
        }

        return stageEnemies[index];
    }

    public int StageCount => stageEnemies.Length;

    public bool IsBossIndex(int index) => index == stageEnemies.Length - 1;
}

using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/NewEnemy")]
public class EnemyData : ScriptableObject
{
    public EnemyController monsterPrefab;

    public int monsterId;
}

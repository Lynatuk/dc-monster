public class EnemyTracker
{
    private EnemyController _currentEnemy;

    public EnemyController CurrentEnemy => _currentEnemy;

    public bool IsEnemyAlive => _currentEnemy != null && !_currentEnemy.IsDead;

    public void Register(EnemyController enemy) => _currentEnemy = enemy;

    public void Clear() => _currentEnemy = null;
}

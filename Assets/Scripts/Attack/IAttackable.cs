using UnityEngine.Events;

public interface IAttackable
{
    void TakeDamage(double amount, UnityAction<DeathEnemyData> dieEvents);
}

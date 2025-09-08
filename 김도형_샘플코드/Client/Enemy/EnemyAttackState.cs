using Enemy;
using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    public void Enter(EnemyController enemy)
    {
        enemy.AnimationPlay("attack");
    }

    public void Update(EnemyController enemy)
    {
    }

    public void Exit(EnemyController enemy) { }
}
using System.Collections;
using UnityEngine;
using Enemy;

public class EnemyDeadState : IEnemyState
{
    public void Enter(EnemyController enemy)
    {
        enemy.AnimationPlay("dead");
        enemy.StartKnockBack();
    }

    public void Update(EnemyController enemy)
    {
        
    }
    public void Exit(EnemyController enemy) { }
}
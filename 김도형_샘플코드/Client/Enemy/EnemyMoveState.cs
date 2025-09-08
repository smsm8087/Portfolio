using Enemy;
using UnityEngine;

public class EnemyMoveState : IEnemyState
{
    public void Enter(EnemyController enemy)
    {
        enemy.AnimationPlay("idle");
    }

    public void Update(EnemyController enemy)
    {
        Vector3 dir = enemy.serverPosition - enemy.transform.position;
        if (dir.sqrMagnitude > 0.01f)
        {
            enemy.transform.position = Vector3.Lerp(enemy.transform.position, enemy.serverPosition, Time.deltaTime * 5f);

            if (Mathf.Abs(dir.x) > 0.01f)
            {
                enemy.spriteRenderer.flipX = dir.x < 0;
            }
        }
    }

    public void Exit(EnemyController enemy) { }
}
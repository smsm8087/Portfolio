using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class EnemyDieHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> Enemies = new ();

    public string Type => "enemy_die";

    public EnemyDieHandler(Dictionary<string, GameObject> Enemies)
    {
        this.Enemies = Enemies;
    }

    public void Handle(NetMsg msg)
    {
        List<string> deadEnemyIds = msg.deadEnemyIds;
        foreach (var deadEnemyId in deadEnemyIds)
        {
            var pid = deadEnemyId;
            if (!Enemies.ContainsKey(pid)) return;
            if (!Enemies[pid]) return;
            var enemyController = Enemies[pid].GetComponent<EnemyController>();
            if(!enemyController) return;
            enemyController.setKilledPlayerId(msg.playerId);
            enemyController.ChangeStateByEnum(EnemyState.Dead);
            SoundManager.Instance.PlaySFX("ememy_dead", 0.7f);
        }
    }
}
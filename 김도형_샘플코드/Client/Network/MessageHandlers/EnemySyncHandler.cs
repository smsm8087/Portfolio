using UnityEngine;
using System.Collections.Generic;
public class EnemySyncHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> Enemies = new ();

    public string Type => "enemy_sync";

    public EnemySyncHandler(Dictionary<string, GameObject> Enemies)
    {
        this.Enemies = Enemies;
    }

    public void Handle(NetMsg msg)
    {
        List<EnemySyncPacket> enemies = msg.enemies;
        foreach (var enemySyncPacket in enemies)
        {
            var pid = enemySyncPacket.enemyId;
            if (!Enemies.ContainsKey(pid)) continue;
            var enemyObj =  Enemies[pid];
            if (!enemyObj) continue;
            EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
            if (!enemyController) continue;
            enemyController.SyncFromServer(enemySyncPacket.x, enemySyncPacket.y,enemySyncPacket.enemyType);
        }
    }
}
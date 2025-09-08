using System.Collections.Generic;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class EnemyChangeStateHandler : INetworkMessageHandler
    {
        private readonly Dictionary<string, GameObject> Enemies = new ();

        public string Type => "enemy_change_state";

        public EnemyChangeStateHandler(Dictionary<string, GameObject> Enemies)
        {
            this.Enemies = Enemies;
        }

        public void Handle(NetMsg msg)
        {
            var pid = msg.enemyId;
            var animName = msg.animName;
            if (!Enemies.ContainsKey(pid)) return;
            EnemyController enemyController = Enemies[pid].GetComponent<EnemyController>();
            switch (animName)
            {
                case "attack" :
                    if (msg.playerId != string.Empty)
                    {
                        enemyController.setTarget(msg.playerId);
                    }
                    enemyController.ChangeStateByEnum(EnemyState.Attack);
                    break;
                case "idle" :
                    enemyController.ChangeStateByEnum(EnemyState.Move);
                    break;
                case "die" :
                    enemyController.ChangeStateByEnum(EnemyState.Dead);
                    break;
            }
        }
    }
}
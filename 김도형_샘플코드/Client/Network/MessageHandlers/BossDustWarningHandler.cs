using System.Collections.Generic;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class BossDustWarningHandler : INetworkMessageHandler
    {
        private readonly Dictionary<string, GameObject> bossDict = new ();
        public string Type => "boss_dust_warning";
        public BossDustWarningHandler(Dictionary<string, GameObject> bossDict)
        {
            this.bossDict = bossDict;
        }
        public void Handle(NetMsg msg)
        {
            var bossObj = bossDict["boss"];
            if (bossObj == null) return;
            BossController bossController = bossObj.GetComponent<BossController>();
            if (bossController == null) return;
            bossController.PlayDustSummon();
            
            var spawnPositions = msg.spawnPositions;
            //TODO:마법진 생성 애니메이션 재생
            foreach (var spawnPosition in spawnPositions)
            { 
                Debug.Log($"곧 먼지가 소환될 예정 x좌표 : {spawnPosition.Item1} , y좌표 : {spawnPosition.Item2} ");
                bossController.PlayDustSummonEffect(spawnPosition.Item1, spawnPosition.Item2 - 0.5f);
            }
        }
    }
}
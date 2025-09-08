using System.Collections.Generic;
using DataModels;
using UI;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class SettlementStartHandler : INetworkMessageHandler
    {
        public string Type => "settlement_start";

        public void Handle(NetMsg msg)
        {
            if (NetworkManager.Instance.MyUserId != msg.playerId) return;

            // 내 플레이어가 죽었는지 확인
            var players = NetworkManager.Instance.GetPlayers();
            if (players != null && players.TryGetValue(NetworkManager.Instance.MyUserId, out GameObject myPlayerObj))
            {
                if (myPlayerObj != null)
                {
                    BasePlayer myPlayer = myPlayerObj.GetComponent<BasePlayer>();
                    if (myPlayer != null && myPlayer.isDead)
                    {
                        Debug.Log("[SettlementStartHandler] 사망 상태로 카드 선택 불가");
                        return;
                    }
                }
            }

            List<CardData> cards = msg.cards;
            float duration = msg.duration;
            int alivePlayerCount = msg.alivePlayerCount;
        
            UIManager.Instance.ShowCardSelectPopup(cards, duration, alivePlayerCount);
        }
    }
}

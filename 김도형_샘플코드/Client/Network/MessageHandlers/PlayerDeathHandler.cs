using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    
    public string Type => "player_dead";
    
    public PlayerDeathHandler(Dictionary<string, GameObject> players)
    {
        this.players = players;
    }
    
    public void Handle(NetMsg msg)
    {
        if (players.TryGetValue(msg.playerId, out GameObject playerObj))
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                // 죽은 위치 설정 추가
                if (msg.deathX != 0 || msg.deathY != 0)
                {
                    player.deathPosition = new Vector3(msg.deathX, msg.deathY, 0);
                }
                else
                {
                    // 메시지에 위치 정보가 없으면 현재 위치 사용
                    player.deathPosition = playerObj.transform.position;
                }
                player.Die();
                Debug.Log($"[PlayerDeathHandler] {msg.playerId} 사망 처리");
            }
        }
    }
}
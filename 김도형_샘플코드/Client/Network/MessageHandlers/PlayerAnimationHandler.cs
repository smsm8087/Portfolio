using UnityEngine;
using System.Collections.Generic;

public class PlayerAnimationHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    public string Type => "player_animation";

    public PlayerAnimationHandler(Dictionary<string, GameObject> players)
    {
        this.players = players;
    }

    public void Handle(NetMsg msg)
    {
        //플레이어 애니메이션 브로드캐스팅
        var pid = msg.playerId;
        //내 플레이어는 이걸 할 필요가 없음.
        if (pid == NetworkManager.Instance.MyUserId) return;
        
        var playerObj = players[pid];
        var animator =  playerObj.GetComponent<Animator>();
        if (!animator) return;
        animator.Play(msg.animation);
    }
}
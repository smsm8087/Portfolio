using UnityEngine;
using System.Collections.Generic;

public class PlayerMoveHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    public string Type => "move";

    public PlayerMoveHandler(Dictionary<string, GameObject> players)
    {
        this.players = players;
    }

    public void Handle(NetMsg msg)
    {
        string pid = msg.playerId;
        //내 플레이어는 이걸 할 필요가 없음.
        if (pid == NetworkManager.Instance.MyUserId) return;
        
        if (!players.ContainsKey(pid)) return;
        
        var playerObj = players[pid];
        var follower = playerObj.GetComponent<NetworkCharacterFollower>();
        if (follower != null)
        {
            follower.SetTargetPosition(new Vector3(msg.x, msg.y, 0));
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class PlayerUpdateHpHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    
    public string Type => "player_update_hp";
    
    public PlayerUpdateHpHandler(Dictionary<string, GameObject> players)
    {
        this.players = players;
    }
    
    public void Handle(NetMsg msg)
    {
        if (msg.playerId == NetworkManager.Instance.MyUserId)
        {
            GameObject profileObj = GameObject.Find("ProfileUI");
            if (!profileObj) return;
            ProfileUI profileUI = profileObj.GetComponent<ProfileUI>();
            if(!profileUI) return;
            profileUI.UpdateHp(msg.playerInfo.currentHp, msg.playerInfo.currentMaxHp);
        }
    }
}
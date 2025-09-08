using System;
using UnityEngine;
using System.Collections.Generic;


public class UpdatePlayerDataHandler : INetworkMessageHandler
{
    public string Type => "update_player_data";
    private readonly Dictionary<string, GameObject> players;
    public UpdatePlayerDataHandler(Dictionary<string, GameObject> players)
    {
        this.players =  players;
    }
    public void Handle(NetMsg msg)
    {
        //프로필 최신화
        string pid = msg.playerInfo.id;
        var myPlayerObj = players[pid];
        if (myPlayerObj == null)
        {
            Debug.LogError("Player " + pid + " doesn't exist");
            return;
        }
        BasePlayer playerController = myPlayerObj.GetComponent<BasePlayer>();
        playerController.ApplyPlayerInfo(msg.playerInfo);
        GameObject profileObj = GameObject.Find("ProfileUI");
        if (!profileObj) return;
        ProfileUI profileUI = profileObj.GetComponent<ProfileUI>();
        if(!profileUI) return;
        profileUI.UpdatePlayerInfo(msg.playerInfo);
    }
}
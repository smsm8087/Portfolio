using System;
using UnityEngine;
using System.Collections.Generic;

public class PlayerLeaveHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    public string Type => "player_leave";

    public void Handle(NetMsg msg)
    {
        string pid = msg.playerId;
        NetworkManager.Instance.RemovePlayer(pid);
    }
}
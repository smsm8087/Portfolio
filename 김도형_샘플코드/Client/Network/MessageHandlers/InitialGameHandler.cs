using System;
using UnityEngine;
using System.Collections.Generic;
using UI;
using UnityEngine.SceneManagement;

public class InitialGameHandler : INetworkMessageHandler
{
    public string Type => "initial_game";
    public void Handle(NetMsg msg)
    {
        GameManager.Instance.InitializeGame(msg.wave_id);
    }
}
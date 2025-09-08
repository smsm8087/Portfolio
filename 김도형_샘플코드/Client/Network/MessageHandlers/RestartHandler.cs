using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class RestartHandler : INetworkMessageHandler
{
    public string Type => "restart";
    
    public void Handle(NetMsg msg)
    {
        //치트썼다는 로그용.
        Debug.Log(String.Format("restart from user : {0}", msg.playerId));
        NetworkManager.Instance.TriggerGameOver();
        GameManager.Instance.ResumeGame();
    }
}
using UnityEngine;
using System.Collections.Generic;

public class SharedHpUpdateHandler : INetworkMessageHandler
{
    public string Type => "shared_hp_update";

    public void Handle(NetMsg msg)
    {
        GameManager.Instance.UpdateHPBar(msg.currentHp,msg.maxHp);
    }
}
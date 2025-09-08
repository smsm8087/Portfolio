using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class SettlementTimerUpdateHandler : INetworkMessageHandler
{
    public string Type => "settlement_timer_update";
    public void Handle(NetMsg msg)
    {
        float duration = msg.duration;
        UIManager.Instance.UpdateSettlementTimer(duration);
        UIManager.Instance.UpdateSettlementReadyCount(msg.readyCount);
    }
}
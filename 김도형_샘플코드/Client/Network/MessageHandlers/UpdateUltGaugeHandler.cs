using UnityEngine;
using System.Collections.Generic;
public class UpdateUltGaugeHandler : INetworkMessageHandler
{
    public string Type => "update_ult_gauge";
    public void Handle(NetMsg msg)
    {
        ProfileUI profileUI = GameObject.Find("ProfileUI").GetComponent<ProfileUI>();
        profileUI.UpdateUltGauge(msg.currentUlt,msg.maxUlt);
    }
}
using UnityEngine;
using System.Collections.Generic;
public class BossSyncHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> bossDict = new ();

    public string Type => "boss_sync";

    public BossSyncHandler(Dictionary<string, GameObject> bossDict)
    {
        this.bossDict = bossDict;
    }

    public void Handle(NetMsg msg)
    {
        var boss =  bossDict["boss"];
        if (!boss) return;
        BossController bossController = boss.GetComponent<BossController>();
        if (!bossController) return;
        bossController.SyncFromServer(msg.x);
    }
}
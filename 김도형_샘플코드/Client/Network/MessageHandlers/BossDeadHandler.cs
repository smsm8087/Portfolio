using UnityEngine;
using System.Collections.Generic;
using UI;

public class BossDeadHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> bossDict = new ();

    public string Type => "boss_dead";

    public BossDeadHandler(Dictionary<string, GameObject> bossDict)
    {
        this.bossDict = bossDict;
    }

    public void Handle(NetMsg msg)
    {
        GameObject boss = bossDict["boss"];
        if (boss == null)
        {
            Debug.LogError("Boss not found");
            return;
        }
        GameObject.Destroy(boss.gameObject);
        bossDict.Remove("boss");
        SoundManager.Instance.PlaySFX("ingame");
    }
}
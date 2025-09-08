using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class BossStartHandler : INetworkMessageHandler
{
    public string Type => "boss_start";
    private readonly GameObject bossPrefab;
    private readonly Dictionary<string, GameObject> bossDict;
    public BossStartHandler(GameObject prefab, Dictionary<string, GameObject> bossDict)
    {
        this.bossPrefab = prefab;
        this.bossDict = bossDict;
    }
    public void Handle(NetMsg msg)
    {
        if (bossDict.ContainsKey("boss")) return;
        SoundManager.Instance.PlayBGM("boss");
        var bossObj = GameObject.Instantiate(bossPrefab, new Vector3(msg.x, msg.y, 0), Quaternion.identity);
        bossDict["boss"] = bossObj;
        bossObj.GetComponent<BossController>().PlayIntroCoroutine(msg.maxHp);
    }
}
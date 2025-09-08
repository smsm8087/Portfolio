using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class BulletDestroyHandler : INetworkMessageHandler
{
    public string Type => "bullet_destroy";
    private readonly Dictionary<string, GameObject> bullets;
    public BulletDestroyHandler(Dictionary<string, GameObject> bullets)
    {
        this.bullets = bullets;
    }
    public void Handle(NetMsg msg)
    {
        List<BulletInfo> destoryBullets = msg.bullets;
        foreach (BulletInfo bullet in destoryBullets)
        {
            string pid = bullet.bulletId;
            if (!bullets.ContainsKey(pid))
            {
                Debug.LogWarning($"[BulletSpawnHandler] 존재하지 않는 bulletId: {pid}");
                return;
            }

            BulletController bulletController = bullets[pid].GetComponent<BulletController>();
            if (bulletController != null)
            {
                bulletController.PlayDeadAnimation();
            }
        }
    }
}
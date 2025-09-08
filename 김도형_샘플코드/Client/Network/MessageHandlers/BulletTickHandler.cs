using System.Collections.Generic;
using UnityEngine;

public class BulletTickHandler : INetworkMessageHandler
{
    public string Type => "bullet_tick";

    private readonly Dictionary<string, GameObject> bullets;

    public BulletTickHandler(Dictionary<string, GameObject> bullets)
    {
        this.bullets = bullets;
    }

    public void Handle(NetMsg msg)
    {
        foreach (var bulletInfo in msg.bullets)
        {
            string bulletId = bulletInfo.bulletId;
            if (bullets.TryGetValue(bulletId, out var bulletObj))
            {
                var controller = bulletObj.GetComponent<BulletController>();
                if (controller != null)
                {
                    controller.SyncFromServer(bulletInfo.x, bulletInfo.y);
                }
            }
        }
    }
}
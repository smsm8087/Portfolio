using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class BulletSpawnHandler : INetworkMessageHandler
{
    public string Type => "bullet_spawn";
    private readonly Dictionary<string, GameObject> bullets;
    private readonly GameObject bulletPrefab;
    public BulletSpawnHandler(Dictionary<string, GameObject> bullets, GameObject bulletPrefab )
    {
        this.bullets = bullets;
        this.bulletPrefab = bulletPrefab;
    }
    public void Handle(NetMsg msg)
    {
        string pid = msg.bulletId;
        if (bullets.ContainsKey(pid))
        {
            Debug.LogWarning($"[BulletSpawnHandler] 이미 존재하는 bulletId: {pid}");
            return;
        }

        Vector3 pos = new Vector3(msg.x, msg.y, 0f);

        GameObject bullet = GameObject.Instantiate(bulletPrefab, pos, Quaternion.identity);
        var bulletController = bullet.GetComponent<BulletController>();
        if (bulletController)
        {
            bulletController.Init(msg.x,msg.y, pid);
            bullet.GetComponent<SpriteRenderer>().sortingOrder = 500 + bullets.Count * 2;
            Debug.Log($"[BulletSpawnHandler] 총알 생성됨: {pid}"); 
            bullets.Add(pid, bullet);
        }
    }
}

using System.Collections;
using DataModels;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    private int spawnOrderOffset = 0;
    public GameObject SpawnEnemy(string guid, float spawnPosX, float spawnPosY, int enemyDataId)
    {
        spawnOrderOffset = NetworkManager.Instance.GetEnemies().Count;
        Vector3 spawnPos = new Vector3(spawnPosX, spawnPosY, 0);
        var enemy = CreateEnemyFromId(enemyDataId, spawnPos);
        var movement = enemy.GetComponent<EnemyController>();
        if (movement)
        {
            movement.spriteRenderer.sortingOrder = spawnOrderOffset * 2;
            movement?.SetGuid(guid);
        }
        
        return enemy;
    }
    GameObject CreateEnemyFromId(int enemyId, Vector3 spawnPos)
    {
        var enemyData = GameDataManager.Instance.GetData<EnemyData>("enemy_data", enemyId);
        var prefab = Resources.Load<GameObject>(enemyData.prefab_path);
        return prefab ? Instantiate(prefab, spawnPos , Quaternion.identity) : null;
    }
}

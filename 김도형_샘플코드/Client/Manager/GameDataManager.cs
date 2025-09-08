using System;
using System.Collections.Generic;
using DataModels;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllData();
    }

    private Dictionary<string, object> _tableDict = new();

    public void LoadAllData()
    {
        //table 추가시에 여기다가 작업
        _tableDict["card_data"] = CsvLoader.Load<CardData>("DataExcels/card_data");
        _tableDict["player_data"] = CsvLoader.Load<PlayerData>("DataExcels/player_data");
        _tableDict["skill_data"]  = CsvLoader.Load<SkillData>("DataExcels/skill_data");
        _tableDict["enemy_data"] = CsvLoader.Load<EnemyData>("DataExcels/enemy_data");
        _tableDict["wave_data"] = CsvLoader.Load<WaveData>("DataExcels/wave_data");
        _tableDict["shared_data"] = CsvLoader.Load<SharedData>("DataExcels/shared_data");
        _tableDict["bullet_data"] = CsvLoader.Load<BulletData>("DataExcels/bullet_data");
    }

    public Dictionary<int, T> GetTable<T>(string tableName)
    {
        if (!_tableDict.TryGetValue(tableName, out var tableObj))
        {
            Debug.LogError($"[GameDataManager] 테이블 {tableName} 없음");
            return null;
        }

        var table = tableObj as Dictionary<int, T>;

        if (table == null)
        {
            Debug.LogError($"[GameDataManager] 테이블 {tableName} 타입 오류");
            return null;
        }

        return table;
    }

    public T GetData<T>(string tableName, int id) where T : class
    {
        var table = GetTable<T>(tableName);

        if (table == null)
            return null;

        if (table.TryGetValue(id, out var data))
            return data;

        Debug.LogWarning($"[GameDataManager] 테이블 {tableName} → ID {id} 없음");
        return null;
    }
}

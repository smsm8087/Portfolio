
using System;
using System.Collections.Generic;
using DataModels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsPaused = false;

    [SerializeField] private WaveManager waveManager;
    [SerializeField] private Transform crystalRoot;
    [SerializeField] private List<SpriteRenderer> backGroundImages;
    [SerializeField] private HPBar hpBar;
    [SerializeField] private HPText hpText;
    [SerializeField] private Image hpImg;
    [SerializeField] private HPBar bossHpBar;
    [SerializeField] private HPText bossHpText;
    [SerializeField] private Image bossHpImg;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
        NetworkManager.Instance.SetOnGamveOverAction( ()=> TriggerGameOver());
    }
    public void TriggerGameOver()
    {
        NetworkManager.Instance.RemoveAllEnemies();
    }
    public void UpdateHPBar(float currentHp, float maxHp)
    {
        hpBar?.UpdateHP(currentHp, maxHp);
        hpText?.UpdateHP(currentHp, maxHp);
    }
    public void UpdateBossHPBar(float currentHp, float maxHp)
    {
        bossHpBar?.UpdateHP(currentHp, maxHp);
        bossHpText?.UpdateHP(currentHp, maxHp);
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
    public void InitializeGame(int wave_id)
    {
        var waveData = GameDataManager.Instance.GetData<WaveData>("wave_data", wave_id);
        if (waveData == null)
        {
            Debug.LogError("wave data not found");
            return;
        }

        var sharedHpData = GameDataManager.Instance.GetData<SharedData>("shared_data", waveData.shared_hp_id);
        if (sharedHpData == null)
        {
            Debug.LogError("sharedHpData not found");
            return;
        }
        
        GameObject crystalPrefab = Resources.Load<GameObject>(sharedHpData.prefab_path);
        if (crystalPrefab != null)
        {
            GameObject go = Instantiate(crystalPrefab, crystalRoot);
        }
        else
        {
            Debug.LogError($"프리팹 로드 실패: {sharedHpData.prefab_path}");
        }
        Sprite backGroundSprite = Resources.Load<Sprite>(waveData.background);
        if (backGroundSprite != null)
        {
            foreach (SpriteRenderer sprite in backGroundImages)
            {
                sprite.sprite = backGroundSprite;
            }
        }

        hpImg.sprite = Resources.Load<Sprite>(waveData.hp_icon);
        bossHpImg.sprite = Resources.Load<Sprite>(waveData.boss_icon);
        SoundManager.Instance.PlayBGM("ingame");
    }
}

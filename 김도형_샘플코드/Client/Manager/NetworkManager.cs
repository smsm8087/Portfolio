using NativeWebSocket.MessageHandlers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    [Header("직업별 프리팹")]
    public GameObject tankPrefab;
    public GameObject sniperPrefab;
    public GameObject programmerPrefab;
    public GameObject maidPrefab;
    
    [Header("총알")]
    [SerializeField] private GameObject bulletPrefab;
    
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private GameObject DamageTextPrefab;
    
    [Header("보스")]
    [SerializeField] private GameObject bossPrefab;
    
    private Dictionary<string, GameObject> players = new();
    private Dictionary<string, GameObject> enemies = new();
    private Dictionary<string, INetworkMessageHandler> handlers = new();
    private Dictionary<string, GameObject> prefabMap = new();
    private Dictionary<string, GameObject> bullets = new();
    private Dictionary<string, GameObject> bossDict = new();
    private event Action onGameOver;
    public string MyUserId => UserSession.UserId;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        prefabMap = new Dictionary<string, GameObject>()
        {
            { "tank", tankPrefab },
            { "sniper", sniperPrefab },
            { "programmer", programmerPrefab },
            { "maid", maidPrefab }
        };
        
        RegisterHandlers();
    }
    void OnDisable()
    {
        if (WebSocketClient.Instance != null)
            WebSocketClient.Instance.OnMessageReceived -= HandleMessage;
    }

    public void Reset()
    {
        Time.timeScale = 1;
        onGameOver = null;
        RemoveAllEnemies();
        RemoveAllBullets();
        RemoveAllPlayers();
        RemoveAllBoss();
        PartyMemberUI.Instance.ClearAllMembers();
        WebSocketClient.Instance.OnMessageReceived -= HandleMessage;
    }
    
    private void Start()
    {
        WebSocketClient.Instance.OnMessageReceived += HandleMessage;
    }

    public Dictionary<string, GameObject> GetEnemies()
    {
        return  enemies;
    }
    public Dictionary<string, GameObject> GetPlayers()
    {
        return  players;
    }
    

    public void SetOnGamveOverAction(Action onGameOver)
    {
        this.onGameOver += onGameOver;
    }

    public void TriggerGameOver()
    {
        this.onGameOver?.Invoke();
    }
    public void RemovePlayer(string guid)
    {
        if (!players.ContainsKey(guid)) return;
        Destroy(players[guid]);
        players.Remove(guid);
    }

    public void RemoveEnemy(string guid)
    {
        if (!enemies.ContainsKey(guid)) return;
        Destroy(enemies[guid]);
        enemies.Remove(guid);
    }
    public void RemoveBullet(string guid)
    {
        if (!bullets.ContainsKey(guid)) return;
        Destroy(bullets[guid]);
        bullets.Remove(guid);
    }

    public void RemoveAllEnemies()
    {
        foreach (var enemyData in enemies)
        {
            GameObject enemyObj = enemyData.Value;
            Destroy(enemyObj);
        }
        enemies.Clear();
    }
    public void RemoveAllBoss()
    {
        foreach (var boss in bossDict)
        {
            GameObject bossObj = boss.Value;
            Destroy(bossObj);
        }
        bossDict.Clear();
    }
    public void RemoveAllPlayers()
    {
        foreach (var player in players)
        {
            GameObject playerObj = player.Value;
            Destroy(playerObj);
        }
        players.Clear();
    }
    public void RemoveAllBullets()
    {
        foreach (var bullet in bullets)
        {
            GameObject bulletObj = bullet.Value;
            Destroy(bulletObj);
        }
        bullets.Clear();
    }

    public void ResetHp()
    {
        //임시 초기화
        GameManager.Instance.UpdateHPBar(100,100);
    }
    private Dictionary<string, INetworkMessageHandler> _handlers;

    private void RegisterHandlers()
    {
        //서버 리시브 처리 부분.
        AddHandler(new PlayerJoinHandler(prefabMap,players, this));
        AddHandler(new PlayerMoveHandler(players));
        AddHandler(new PlayerListHandler(prefabMap, players));
        AddHandler(new PlayerLeaveHandler());
        AddHandler(new SpawnEnemyHandler(enemies,waveManager));
        AddHandler(new EnemySyncHandler(enemies));
        AddHandler(new EnemyDieHandler(enemies));
        AddHandler(new SharedHpUpdateHandler());
        AddHandler(new CountDownHandler());
        AddHandler(new WaveStartHandler());
        AddHandler(new GameResultHandler());
        AddHandler(new RestartHandler());
        AddHandler(new PlayerAnimationHandler(players));
        AddHandler(new EnemyDamagedHandler(enemies,DamageTextPrefab));
        AddHandler(new EnemyChangeStateHandler(enemies));
        AddHandler(new SettlementStartHandler());
        AddHandler(new SettlementTimerUpdateHandler());
        AddHandler(new UpdateUltGaugeHandler());
        AddHandler(new UpdatePlayerDataHandler(players));
        AddHandler(new PartyMemberHealthHandler());
        AddHandler(new PartyMemberUltHandler());
        AddHandler(new PartyMemberStatusHandler());
        AddHandler(new PartyInfoHandler());
        AddHandler(new PartyMemberLeftHandler());
        AddHandler(new InitialGameHandler());
        AddHandler(new PlayerUpdateHpHandler(players));
        AddHandler(new BulletSpawnHandler(bullets, bulletPrefab));
        AddHandler(new BulletTickHandler(bullets));
        AddHandler(new BulletDestroyHandler(bullets));
        AddHandler(new PlayerDeathHandler(players));
        AddHandler(new BossStartHandler(bossPrefab, bossDict));
        AddHandler(new BossDamagedHandler(bossDict,DamageTextPrefab));
        AddHandler(new BossSyncHandler(bossDict));
        AddHandler(new BossDustWarningHandler(bossDict));
        AddHandler(new BossDeadHandler(bossDict));
        AddHandler(new RevivalStartedHandler());
        AddHandler(new RevivalProgressHandler());
        AddHandler(new RevivalCompletedHandler(players));
        AddHandler(new RevivalCancelledHandler(players));
        AddHandler(new InvulnerabilityEndedHandler(players));
        AddHandler(new SkillUsedHandler());
    }

    private void AddHandler(INetworkMessageHandler handler)
    {
        handlers[handler.Type] = handler;
    }

    public void HandleMessage(string msg)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(msg);
        if (handlers.TryGetValue(netMsg.type, out var handler))
        {
            handler.Handle(netMsg);
        }
        else
        {
            Debug.LogWarning($"Unhandled message type: {netMsg.type}");
        }
    }
    
    public void SendMsg(NetMsg msg)
    {
        msg.playerId ??=  UserSession.UserId;
        msg.roomCode ??= RoomSession.RoomCode;
        
        string json = JsonConvert.SerializeObject(msg);
        WebSocketClient.Instance.Send(json);
    }
}

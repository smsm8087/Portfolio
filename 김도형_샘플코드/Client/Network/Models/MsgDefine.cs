using System;
using System.Collections.Generic;
using DataModels;

[System.Serializable]
public class NetMsg
{
    public string type;
    public string roomCode;
    public List<RoomInfo> RoomInfos;
    public string hostId;
    public int wave;
    public int wave_id;
    public string requestId;
    public bool approved;
    
    public string playerId;
    public string nickName;
    public string enemyId;
    public int enemyDataId;
    public List<PlayerInfo> players; // List<string> -> List<PlayerInfo>로 변경
    public string jobType;
    
    public string animation;
    //player
    public float x;
    public float y;
    public float currentUlt;
    public float maxUlt;
    public PlayerInfo playerInfo { get; set; }
    
    //partymember
    public string player_id;         
    public float current_health;      
    public float max_health;        
    public float current_ult;      
    public float max_ult;          
    public string status;            
    public List<PartyMemberInfo> members;
    
    //히트박스 영역
    public float attackBoxCenterX;
    public float attackBoxCenterY;
    public float attackBoxWidth;
    public float attackBoxHeight;
        
    //enemy
    public float spawnPosX;
    public float spawnPosY;
    public List<EnemySyncPacket> enemies;
    public List<string> deadEnemyIds;
    public List<EnemyDamageInfo> damagedEnemies;
    public string animName;
    
    //ui
    public float currentHp;
    public float maxHp;
    public int countDown;
    public string message;
    public string result_type;
    
    //settlementPhase
    public float duration;
    public List<CardData> cards;
    public int selectedCardId;
    public int readyCount;
    public int alivePlayerCount;
    
    //bullet
    public string bulletId;
    public List<BulletInfo> bullets;

    //boss
    public BossDamageInfo damagedBoss;

    public List<(float, float)> spawnPositions;
    //cheat
    public bool isCheat;
    
    //revival
    public string targetId;
    public string reviverId;
    public float progress;
    public string reason;
    public float reviveX;
    public float reviveY;
    public float deathX;
    public float deathY;
    public float invulnerabilityDuration;
    public bool isDead;
    public bool isBeingRevived;
    public bool isInvulnerable;
    public string revivedBy;
    
    //skill
    public int skillId;
    public float dirX;
    public float dirY;
}

[System.Serializable]
public class PlayerInfo
{
    public string id { get; set; }
    public string nickName { get; set; }
    public string job_type { get; set; }
    public int currentHp { get; set; }
    public float currentUlt { get; set; }
    public int currentMaxHp { get; set; }
    public float currentUltGauge { get; set; }
    public float currentMoveSpeed { get; set; }
    public float currentAttackSpeed { get; set; }
    public int currentCriPct { get; set; }
    public int currentCriDmg { get; set; }
    public float currentAttack { get; set; }
    public List<int> cardIds { get; set; } = new List<int>();
    public PlayerData playerBaseData { get; set; }
}
public class RoomInfo
{
    public string playerId;
    public string nickName;
    public string jobType;
    public bool isReady = false;
}

public class ChatInfo
{
    public string playerId;
    public string message;
}
public class PartyMemberInfo
{
    public string id;
    public string job_type;
    public float current_health;
    public float max_health;
    public float current_ult;
    public float max_ult;
    public bool is_dead;
    public bool is_being_revived;
    public bool is_invulnerable;
    public string revived_by;
    public float? death_position_x;
    public float? death_position_y;
}

public class EnemySyncPacket
{
    public string enemyId { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public string enemyType { get; set; }
}

public class EnemyDamageInfo
{
    public string enemyId { get; set; }
    public int currentHp { get; set; }
    public int maxHp { get; set; }
    public int damage { get; set; }
    public bool isCritical { get; set; }
}
public class BossDamageInfo
{
    public string playerId { get; set; }
    public int currentHp { get; set; }
    public int maxHp { get; set; }
    public int damage { get; set; }
    public bool isCritical { get; set; }
}
public class BulletInfo
{
    public string bulletId;
    public float x;
    public float y;
}
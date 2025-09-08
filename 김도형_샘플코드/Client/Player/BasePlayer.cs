using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 모든 플레이어 직업의 공통 기능을 담당하는 기본 클래스.
/// 서버에서 받은 PlayerInfo 데이터를 기반으로 능력치를 설정하며,
/// FSM 상태 관리, 애니메이션 전송, 공격 메시지 처리 등 공통 동작을 포함.
/// </summary>
public abstract class BasePlayer : MonoBehaviour
{
    [Header("Player Info")]
    public string playerGUID;
    public string job_type;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] public float jumpForce = 10f;

    [Header("Battle Stats")]
    public float attackPower;
    public float attackSpeed;
    public int critChance;
    public int critDamage;
    public int currentHp;
    public int maxHp;
    public float currentUlt;
    public float currentUltGauge;
    public List<int> cardIds = new();

    [Header("Unity Components")]
    public Rigidbody2D _rb;
    public SpriteRenderer _sr;
    public Animator _animator;

    [Header("Ground Check")]
    public bool _isGrounded = true;

    [Header("Attack Range")]
    public Transform attackRangeTransform;
    public BoxCollider2D attackRangeCollider;
    
    [Header("Player Status")]
    public bool isDead = false;
    
    [Header("Revival System")]
    public bool isBeingRevived = false;
    public bool isInvulnerable = false;
    public string revivedBy = "";
    public Vector3 deathPosition;
    
    public bool ActionLocked { get; set; } = false;

    private float revivalStartTime = 0f;
    private bool isCurrentlyReviving = false;
    private string currentRevivalTarget = "";
    
    private Coroutine invulnerabilityCoroutine;
    private SpriteRenderer spriteRenderer;  
    

    // FSM
    protected PlayerState currentState;
    protected PlayerState prevState;
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public JumpState jumpState { get; private set; }
    public AttackState attackState { get; private set; }
    public DeathState deathState { get; private set; }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        var attackRangeObj = transform.Find("AttackRanageCollider");
        if (attackRangeObj)
        {
            attackRangeTransform = attackRangeObj.transform;
            attackRangeCollider = attackRangeObj.GetComponent<BoxCollider2D>();    
        }
    }

    protected virtual void Start()
    {
        idleState = new IdleState(this);
        moveState = new MoveState(this);
        jumpState = new JumpState(this);
        attackState = new AttackState(this);
        deathState = new DeathState(this);

        ChangeState(idleState);
    }

    protected virtual void Update()
    {
        // 사망 상태일 때는 입력 처리 안함
        if (isDead) return;
        
        // 본인 캐릭터일 때만 상태 업데이트 및 서버로 위치 전송
        if (!IsMyPlayer) return;

        SendMoveToServer();
        
        if (ActionLocked) return;
        
        currentState?.Update();
        
        // 부활 입력 체크
        CheckRevivalInput();
    }
    
    protected void SendUseSkill(int skillId, Vector2 dir)
    {
        var msg = new NetMsg
        {
            type = "use_skill",
            playerId = NetworkManager.Instance.MyUserId,
            skillId = skillId,
            dirX = dir.x,
            dirY = dir.y,
            x = transform.position.x,
            y = transform.position.y
        };
        NetworkManager.Instance.SendMsg(msg);
    }

    public virtual void ChangeState(PlayerState newState)
    {
        if (newState == null)
        {
            Debug.LogError("새로운 상태가 null입니다!");
            return;
        }
    
        currentState?.Exit();
        prevState = currentState;
        currentState = newState;
        currentState.Enter();
    }

    public PlayerState GetCurrentState() => currentState;
    public PlayerState GetPrevState() => prevState;

    /// <summary>
    /// 서버에서 받은 PlayerInfo를 바탕으로 능력치 적용
    /// </summary>
    public virtual void ApplyPlayerInfo(PlayerInfo info)
    {
        playerGUID = info.id;
        job_type = info.job_type;
        moveSpeed = info.currentMoveSpeed;
        jumpForce = 10f; // 필요 시 직업별 기본값 설정 가능

        attackPower = info.currentAttack;
        attackSpeed = info.currentAttackSpeed;
        critChance = info.currentCriPct;
        critDamage = info.currentCriDmg;

        currentHp = info.currentHp;
        maxHp = info.currentMaxHp;
        currentUlt = info.currentUlt;
        currentUltGauge = info.currentUltGauge;

        cardIds = new List<int>(info.cardIds);
    }

    public float GetMoveSpeed() => moveSpeed;

    public virtual void SendAnimationMessage(string animation)
    {
        var animationMsg = new NetMsg
        {
            type = "player_animation",
            playerId = NetworkManager.Instance.MyUserId,
            animation = animation
        };
        NetworkManager.Instance.SendMsg(animationMsg);
    }

    private void SendMoveToServer()
    {
        var pos = transform.position;

        var moveMsg = new NetMsg
        {
            type = "move",
            playerId = NetworkManager.Instance.MyUserId,
            x = pos.x,
            y = pos.y,
        };

        NetworkManager.Instance.SendMsg(moveMsg);
    }

    public virtual void OnSendAttackRequest()
    {
        if (!IsMyPlayer) return;
        NetMsg attackMsg = AttackBoxHelper.BuildAttackMessage(this);
        NetworkManager.Instance.SendMsg(attackMsg);
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
        {
            _isGrounded = true;
        }
    }

    /// <summary>
    /// 점프 중 공격이 가능한 직업인지 여부
    /// </summary>
    public virtual bool CanAttackWhileJumping => true;
    
    public virtual bool IsMyPlayer
    {
        get => playerGUID == NetworkManager.Instance.MyUserId;
    }
    // 사망 처리 메서드
    public virtual void Die()
    {
        if (isDead) return;
        
        isDead = true;
        deathPosition = transform.position;
        // 무적 상태 해제
        StopInvulnerability();
        // 부활 이펙트 중단 (혹시 진행 중이었다면)
        if (RevivalEffectManager.Instance != null)
        {
            RevivalEffectManager.Instance.StopRevivalEffect(playerGUID);
        }
        
        SpectatorManager.Instance.OnPlayerDied(playerGUID);

        if (IsMyPlayer)
        {
            ChangeState(deathState);
            SpectatorManager.Instance.StartSpectating();
        }
    }
    
    // 부활 처리 메서드
    public virtual void Revive()
    {
        if (!isDead) return;
        
        isDead = false;
        isBeingRevived = false;
        revivedBy = "";
        
        // 부활 완료 이펙트 재생
        if (RevivalEffectManager.Instance != null)
        {
            RevivalEffectManager.Instance.PlayRevivalCompleteEffect(transform.position);
        }

        if (IsMyPlayer)
        {
            ChangeState(idleState);
            SpectatorManager.Instance.StopSpectating();
        }
    }
    
    /// <summary>
    /// 서버에서 부활 취소 메시지를 받았을 때 로컬 부활 상태 초기화
    /// </summary>
    public void CancelLocalRevival(string targetId)
    {
        if (!IsMyPlayer) return;
        if (!isCurrentlyReviving) return;
        if (currentRevivalTarget != targetId) return;

        Debug.Log($"[BasePlayer] 서버 취소 신호로 로컬 부활 상태 초기화: {targetId}");
        isCurrentlyReviving = false;
        currentRevivalTarget = "";
    }
    
    /// <summary>
    /// 부활 입력 체크 (F키)
    /// </summary>
    private void CheckRevivalInput()
    {
        if (isDead || !IsMyPlayer) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("[부활] F키 눌림 - 부활 시도");
            string targetId = FindNearbyDeadPlayer();
            if (!string.IsNullOrEmpty(targetId))
            {
                TryStartRevival();
                // 부활 시작 시간 기록
                revivalStartTime = Time.time;
                isCurrentlyReviving = true;
                currentRevivalTarget = targetId;
                Debug.Log($"[부활] 부활 시작 - 대상: {targetId}");
            }
            else
            {
                Debug.Log("[부활] 근처에 부활 가능한 플레이어 없음");
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            Debug.Log("[부활] F키 뗌 - 부활 중단");
            if (isCurrentlyReviving)
            {
                TryStopRevival();
                isCurrentlyReviving = false;
                currentRevivalTarget = "";
            }
        }

        // F키를 누르고 있는 동안 진행률 업데이트
        if (Input.GetKey(KeyCode.F) && isCurrentlyReviving && !string.IsNullOrEmpty(currentRevivalTarget))
        {
            UpdateRevivalProgress();
        }
    }

    /// <summary>
    /// 부활 시작 시도
    /// </summary>
    private void TryStartRevival()
    {
        string targetPlayerId = FindNearbyDeadPlayer();
        if (string.IsNullOrEmpty(targetPlayerId)) 
        {
            Debug.Log("[부활] 근처에 부활 가능한 플레이어 없음");
            return;
        }

        Debug.Log($"[부활] 부활 시작 요청 전송: targetId={targetPlayerId}");
    
        // 중복 요청 방지
        if (isBeingRevived)
        {
            Debug.Log("[부활] 이미 부활 진행 중");
            return;
        }
    
        // 서버에 부활 시작 요청
        var revivalMsg = new NetMsg
        {
            type = "start_revival",
            playerId = NetworkManager.Instance.MyUserId,
            targetId = targetPlayerId
        };
        NetworkManager.Instance.SendMsg(revivalMsg);
    
        Debug.Log("[부활] 부활 메시지 전송 완료");
    }

    /// <summary>
    /// 부활 중단
    /// </summary>
    private void TryStopRevival()
    {
        if (string.IsNullOrEmpty(currentRevivalTarget)) return;

        // 서버에 부활 취소 요청
        var cancelMsg = new NetMsg
        {
            type = "cancel_revival",
            playerId = NetworkManager.Instance.MyUserId,
            targetId = currentRevivalTarget
        };
        NetworkManager.Instance.SendMsg(cancelMsg);
        
        Debug.Log($"[부활] 부활 취소 요청 전송: {currentRevivalTarget}");
    }

    /// <summary>
    /// 부활 진행률 업데이트
    /// </summary>
    private void UpdateRevivalProgress()
    {
        if (!isCurrentlyReviving || string.IsNullOrEmpty(currentRevivalTarget)) return;
        
        // 거리 체크
        const float maxDistance = 2.0f;
        var playersDict = NetworkManager.Instance.GetPlayers();
        if (playersDict.TryGetValue(currentRevivalTarget, out GameObject targetObj))
        {
            var targetPlayer = targetObj.GetComponent<BasePlayer>();
            float distance = Vector3.Distance(transform.position, targetPlayer.deathPosition);
            Debug.Log($"[부활] 대상과 거리: {distance:F2} (최대: {maxDistance})");
            if (distance > maxDistance)
            {
                Debug.Log("[부활] 거리 초과로 부활 취소");
                TryStopRevival();
                isCurrentlyReviving = false;
                currentRevivalTarget = "";
                return;
            }
        }
        
        // 실제 경과 시간 기반 진행률 계산
        float elapsedTime = Time.time - revivalStartTime;
        float progress = (elapsedTime / 3.0f) * 100f; // 3초로 완료
        progress = Mathf.Clamp(progress, 0f, 100f);
        
        var progressMsg = new NetMsg
        {
            type = "update_revival",
            playerId = NetworkManager.Instance.MyUserId,
            targetId = currentRevivalTarget,
            progress = progress
        };
        NetworkManager.Instance.SendMsg(progressMsg);
        
        Debug.Log($"[부활] 진행률 업데이트: {progress:F1}% (경과시간: {elapsedTime:F1}초)");
    }

    /// <summary>
    /// 근처 죽은 플레이어 찾기
    /// </summary>
    private string FindNearbyDeadPlayer()
    {
        var players = NetworkManager.Instance.GetPlayers();
        float maxDistance = 2.0f;
    
        Debug.Log($"근처 죽은 플레이어 찾기 시작. 총 플레이어 수: {players.Count}");
    
        foreach (var kvp in players)
        {
            if (kvp.Key == playerGUID) continue;
        
            GameObject playerObj = kvp.Value;
            if (playerObj == null) 
            {
                Debug.Log($"플레이어 {kvp.Key} 오브젝트가 null");
                continue;
            }
        
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player == null) 
            {
                Debug.Log($"플레이어 {kvp.Key}에 BasePlayer 컴포넌트 없음");
                continue;
            }
        
            Debug.Log($"플레이어 {kvp.Key}: isDead={player.isDead}, isBeingRevived={player.isBeingRevived}");
        
            if (!player.isDead || player.isBeingRevived) continue;
        
            float distance = Vector3.Distance(transform.position, player.deathPosition);
            Debug.Log($"플레이어 {kvp.Key}와의 거리: {distance}, 최대거리: {maxDistance}");
        
            if (distance <= maxDistance)
            {
                Debug.Log($"부활 가능한 플레이어 발견: {kvp.Key}");
                return kvp.Key;
            }
        }
    
        Debug.Log("근처에 부활 가능한 플레이어 없음");
        return null;
    }

    /// <summary>
    /// 현재 부활시키고 있는 대상 ID 가져오기
    /// </summary>
    private string GetRevivalTargetId()
    {
        // 현재 부활 중인 대상을 추적하는 로직 필요
        // 임시로 빈 문자열 반환
        return "";
    }

    /// <summary>
    /// 무적 상태 시작
    /// </summary>
    public void StartInvulnerability(float duration)
    {
        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
        }
        
        isInvulnerable = true;
        invulnerabilityCoroutine = StartCoroutine(InvulnerabilityCoroutine(duration));
    }

    /// <summary>
    /// 무적 상태 해제
    /// </summary>
    public void StopInvulnerability()
    {
        if (invulnerabilityCoroutine != null)
        {
            StopCoroutine(invulnerabilityCoroutine);
            invulnerabilityCoroutine = null;
        }
        
        isInvulnerable = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    /// <summary>
    /// 무적 상태 코루틴
    /// </summary>
    private IEnumerator InvulnerabilityCoroutine(float duration)
    {
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            if (spriteRenderer != null)
            {
                float alpha = Mathf.PingPong(Time.time * 10f, 1f);
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 무적 해제
        isInvulnerable = false;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
        
        invulnerabilityCoroutine = null;
    }

    /// <summary>
    /// 부활 상태 업데이트 (서버에서 호출)
    /// </summary>
    public void SetRevivalState(bool beingRevived, string reviverPlayerId = "")
    {
        bool wasBeingRevived = isBeingRevived;
        isBeingRevived = beingRevived;
        revivedBy = reviverPlayerId;
        
        // 부활 중단 시 이펙트 정지
        if (!beingRevived && wasBeingRevived)
        {
            if (RevivalEffectManager.Instance != null)
            {
                RevivalEffectManager.Instance.StopRevivalEffect(playerGUID);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
public enum EnemyState
{
    None,
    Move,
    Attack,
    Dead
}
public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public string guid;
    public Vector3 serverPosition;
    public SpriteRenderer spriteRenderer;
    public GameObject outlineObj;
    
    private string killedPlayerId;
    private string targetPlayerId;
    
    private IEnemyState currentState;
    public EnemyMoveState moveState = new ();
    public EnemyAttackState attackState = new ();
    public EnemyDeadState deadState = new ();
    private bool alreadyHit = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        serverPosition = transform.position;
        currentState = null;
    }

    void Start()
    {
        StartCoroutine(FadeInCoroutine());
        ChangeState(moveState);
    }

    void Update()
    {
        currentState?.Update(this);
    }
    public void setTarget(string playerId)
    {
        targetPlayerId = playerId;
    }
    public void OnCheckFlip()
    {
        alreadyHit = false;
        var players = NetworkManager.Instance.GetPlayers();
        if (!players.ContainsKey(targetPlayerId)) return;
        
        var targetPos = players[targetPlayerId].transform.position;
        if (targetPos.x != 0)
        {
            Vector3 dir = targetPos - transform.position;
            if (Mathf.Abs(dir.x) > 0.01f)
            {
                spriteRenderer.flipX = dir.x < 0;
            }
        }
    }
    public void ShowOutline(float duration = 3f)
    {
        if (outlineObj == null) return;

        outlineObj.SetActive(true);
        CancelInvoke(nameof(HideOutline));
        Invoke(nameof(HideOutline), duration);
    }

    private void HideOutline()
    {
        outlineObj.SetActive(false);
    }
    public void ChangeStateByEnum(EnemyState stateEnum)
    {
        switch (stateEnum)
        {
            case EnemyState.Move:
                ChangeState(moveState);
                break;
            case EnemyState.Attack:
                ChangeState(attackState);
                break;
            case EnemyState.Dead:
                ChangeState(deadState);
                break;
        }
    }
    public void ChangeState(IEnemyState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void SyncFromServer(float posX, float posY, string enemyType)
    {
        if (currentState == null)
        {
            ChangeState(moveState);
        }

        if (enemyType == "player")
        {
            serverPosition = new Vector3(posX, posY, transform.position.z);
        }
        else
        {
            serverPosition = new Vector3(posX, transform.position.y, transform.position.z);
        }
        
    }

    public void SetGuid(string guid)
    {
        this.guid = guid;
    }
    public void OnAttackHit()
    {
        if (!string.IsNullOrEmpty(targetPlayerId))
        {
            if (alreadyHit) return;
            alreadyHit = true;    
        }
        Debug.Log($"Enemy {guid} 공격 HIT!");

        // 서버에 공격 명중 메시지 전송
        var enemy_hit_msg = new NetMsg
        {
            type = "enemy_attack_hit",
            playerId = NetworkManager.Instance.MyUserId,
            enemyId = guid,
        };
        NetworkManager.Instance.SendMsg(enemy_hit_msg);
    }
    public void OnDeadAction()
    {
        Debug.Log($"Enemy {guid} Dead");
        StartFadeOut();
    }
    public void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }
    private IEnumerator FadeOutCoroutine()
    {
        CancelInvoke(nameof(HideOutline));
        outlineObj.SetActive(false);
        float elapsed = 0f;
        Color color = spriteRenderer.color;
        float duration = 0.5f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        gameObject.SetActive(false); // 완전히 사라지면 비활성화
        NetworkManager.Instance.RemoveEnemy(guid);
    }
    private IEnumerator FadeInCoroutine()
    {
        float elapsed = 0f;
        Color color = spriteRenderer.color;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    public void setKilledPlayerId(string playerId)
    {
        this.killedPlayerId = playerId;
    }
    public void StartKnockBack()
    {
        StartCoroutine(KnockBackCoroutine());
    }
    private IEnumerator KnockBackCoroutine()
    {
        var players = NetworkManager.Instance.GetPlayers();
        if (players.ContainsKey(killedPlayerId))
        {
            GameObject player = players[killedPlayerId];
            Vector3 attackPosition = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
            
            float elapsed = 0f;
            float duration = 0.5f;
            Vector3 startPos = transform.position;
            //오른쪽으로 볼때 flipx == false
            Vector3 hitDir = (transform.position - attackPosition).normalized;
            Vector3 endPos = startPos + hitDir * 0.3f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = Utils.EaseOutCubic(t); 
                transform.position = Vector3.Lerp(startPos, endPos, eased);
                yield return null;
            }
        }
    }

    public void AnimationPlay(string name)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(name)) return;
        animator.Play(name);
    }
}
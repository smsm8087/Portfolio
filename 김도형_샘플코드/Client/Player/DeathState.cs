using UnityEngine;

public class DeathState : PlayerState
{
    public DeathState(BasePlayer player) : base(player) { }

    public override void Enter()
    {
        player._animator.Play(AnimationNames.Death);  // "DEAD_Clip" 재생
        player.SendAnimationMessage(AnimationNames.Death);
        
        // 사망 시 물리 비활성화
        player._rb.linearVelocity = Vector2.zero;
        player._rb.bodyType = RigidbodyType2D.Kinematic;
        
        // 공격 범위 비활성화
        if (player.attackRangeCollider != null)
            player.attackRangeCollider.enabled = false;
            
        Debug.Log($"[DeathState] {player.playerGUID} 사망 상태 진입");
    }

    public override void Update()
    {
        // Animator가 자동으로 루프 처리하므로 별도 코드 불필요
    }

    public override void Exit()
    {
        // 부활 시 물리 활성화
        player._rb.bodyType = RigidbodyType2D.Dynamic;
        
        // 공격 범위 활성화
        if (player.attackRangeCollider != null)
            player.attackRangeCollider.enabled = true;
            
        Debug.Log($"[DeathState] {player.playerGUID} 사망 상태 종료");
    }
}
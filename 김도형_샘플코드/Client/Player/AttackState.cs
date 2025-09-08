using UnityEngine;

public class AttackState : PlayerState
{
    private float attackDuration = 0.5f;
    private float elapsedTime = 0f;

    public AttackState(BasePlayer player) : base(player) { }

    public override void Enter()
    {
        //sound
        SoundManager.Instance.PlaySFX($"{player.job_type}_hit");
        
        // 공격속도에 따라 애니메이션 속도 조절 (이제 모든 직업 기본 1.0)
        player._animator.speed = player.attackSpeed;
      
        player._animator.Play(AnimationNames.Attack);
        player.SendAnimationMessage(AnimationNames.Attack);
      
        // 원본 애니메이션 길이를 공격속도로 나누어 실제 재생 시간 계산
        float originalDuration = GetAnimationClipLength(AnimationNames.Attack);
        attackDuration = originalDuration / player.attackSpeed;
    }

    private float GetAnimationClipLength(string clipName)
    {
        var clips = player._animator.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"[AttackState] 애니메이션 클립 '{clipName}' 을 찾을 수 없습니다. 기본 0.5초 사용.");
        return 0.5f; // fallback
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        // 애니메이션 끝나면 이전 상태로 복귀
        if (elapsedTime >= attackDuration)
        {
            // 이전 상태가 null이면 Idle로
            PlayerState targetState = player.GetPrevState() ?? player.idleState;
            player.ChangeState(targetState);
            elapsedTime = 0f;
        }
    }

    public override void Exit()
    {
        // 애니메이션 속도를 기본값으로 복원
        player._animator.speed = 1.0f;
    }
}
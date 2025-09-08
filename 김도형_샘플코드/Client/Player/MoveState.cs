using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(BasePlayer player) : base(player) { }

    public override void Enter()
    {
        player._animator.Play(AnimationNames.Run); 
        player.SendAnimationMessage(AnimationNames.Run); 
    }

    public override void Update()
    {
        // 이동
        float moveInput = InputManager.GetMoveInput();
        MovementHelper.Move(player._rb, moveInput, player.GetMoveSpeed());

        // 방향 전환
        if (moveInput > 0)
        {
            player._sr.flipX = true;
        }
        else if (moveInput < 0)
        {
            player._sr.flipX = false;
        }

        // 입력 체크 → 상태 전환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ChangeState(player.jumpState);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            player.ChangeState(player.attackState);
        }
        else if (Mathf.Abs(moveInput) < 0.01f)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        // 필요시 Exit 처리 (비워도 됨)
    }
}
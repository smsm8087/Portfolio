using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(BasePlayer player) : base(player) { }

    public override void Enter()
    {
        player._animator.Play(AnimationNames.Jump); 
        player.SendAnimationMessage(AnimationNames.Jump);

        // 점프 처리
        if (player.GetPrevState() != player.attackState)
        {
            MovementHelper.Jump(player._rb, player.jumpForce);
            player._isGrounded = false; // 점프 중 상태
        }
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
        if (Input.GetKeyDown(KeyCode.Z) && player.CanAttackWhileJumping)
        {
            player.ChangeState(player.attackState);
        }
        // 착지 체크
        if (player._isGrounded)
        {
            player.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        // 필요 시 Exit 처리
    }
}
using UnityEngine;

public class JumpState : State
{
    public JumpState(Player player, StateMachine stateMachine) : base(player, stateMachine, "isJumping")
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpForce);
        player.anim.SetTrigger("jump");
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (player.isGrounded && player.rb.linearVelocity.y < 0.1f)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}

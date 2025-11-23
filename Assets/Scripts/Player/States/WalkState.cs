using UnityEngine;

public class WalkState : State
{
    public WalkState(Player player, StateMachine stateMachine) : base(player, stateMachine, "isWalking")
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("isWalking", true);
        player.anim.SetBool("isRunning", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Mathf.Abs(player.moveInput) < Mathf.Epsilon)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            stateMachine.ChangeState(player.runState);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && player.isGrounded)
        {
            stateMachine.ChangeState(player.jumpState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        float targetVelocityX = player.moveInput * player.moveSpeed;
        player.rb.linearVelocity = new Vector2(targetVelocityX, player.rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("isWalking", false);
    }
}

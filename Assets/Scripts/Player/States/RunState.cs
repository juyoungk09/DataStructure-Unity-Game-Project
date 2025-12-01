using UnityEngine;

public class RunState : State
{
    public RunState(Player player, StateMachine stateMachine) : base(player, stateMachine, "isRunning")
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("isRunning", true);
        player.anim.SetBool("isWalking", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Mathf.Abs(player.moveInput) < Mathf.Epsilon)
        {
            stateMachine.ChangeState(player.idleState);
        }
        else if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            stateMachine.ChangeState(player.walkState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        float targetVelocityX = player.moveInput * (player.moveSpeed * player.runMultiplier);
        player.rb.linearVelocity = new Vector2(targetVelocityX, player.rb.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("isRunning", false);
    }
}
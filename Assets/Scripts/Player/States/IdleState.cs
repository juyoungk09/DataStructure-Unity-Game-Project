using UnityEngine;

public class IdleState : State
{
    public IdleState(Player player, StateMachine stateMachine) : base(player, stateMachine, "isIdle")
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.rb.linearVelocity = new Vector2(0, player.rb.linearVelocity.y);
        player.anim.SetBool("isWalking", false);
        player.anim.SetBool("isRunning", false);
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
            
        if (Mathf.Abs(player.moveInput) > Mathf.Epsilon)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                stateMachine.ChangeState(player.runState);
            }
            else
            {
                stateMachine.ChangeState(player.walkState);
            }
        }
    }
}


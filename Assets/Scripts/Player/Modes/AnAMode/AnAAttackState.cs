

public class AnAAttackState : State
{
    public AnAAttackState(Player player, StateMachine stateMachine) : base(player, stateMachine, "isAttacking")
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("isAttacking", true);
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("isAttacking", false);
    }
}


public class AnASkill2State : State
{
    public AnASkill2State(Player player, StateMachine stateMachine) : base(player, stateMachine, "isSkill2")
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("isAnASkill2", true);
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("isAnASkill2", false);
    }
}
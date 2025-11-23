

public class AnASkill1State : State
{
    public AnASkill1State(Player player, StateMachine stateMachine) : base(player, stateMachine, "isSkill1")
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.anim.SetBool("isAnASkill1", true);
    }

    public override void Exit()
    {
        base.Exit();
        player.anim.SetBool("isAnASkill1", false);
    }
}
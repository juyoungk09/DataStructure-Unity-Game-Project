using UnityEngine;

public abstract class State
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected float startTime;
    protected bool isAnimationFinished;
    protected bool isExitingState;

    public State(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        startTime = Time.time;
        isAnimationFinished = false;
        isExitingState = false;
        player.anim.SetBool(animBoolName, true);
        // Debug.Log($"Entering {this.GetType().Name}");
    }

    public virtual void Exit()
    {
        isExitingState = true;
        player.anim.SetBool(animBoolName, false);
    }

    public virtual void LogicUpdate()
    {
        // Handle input and state transitions here
    }

    public virtual void PhysicsUpdate()
    {
        // Handle physics updates here
    }

    public virtual void AnimationTrigger()
    {
        // Handle animation events
    }

    public virtual void AnimationFinishTrigger()
    {
        isAnimationFinished = true;
    }
}

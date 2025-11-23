using UnityEngine;

public class StateMachine
{
    public State CurrentState { get; private set; }
    public State PreviousState { get; private set; }

    public void Initialize(State startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(State newState)
    {
        if (CurrentState == newState) return;
        Debug.Log($"{CurrentState.GetType().Name}에서 {newState.GetType().Name}으로 State 전환");
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update()
    {
        CurrentState?.LogicUpdate();
    }

    public void FixedUpdate()
    {
        CurrentState?.PhysicsUpdate();
    }
}

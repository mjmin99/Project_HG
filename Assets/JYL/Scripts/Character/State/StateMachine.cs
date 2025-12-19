using UnityEngine;

public class StateMachine
{
    public BaseState CurrentState { get; private set; }

    public void Initialize(BaseState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    public void ChangeState(BaseState newState)
    {
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
    public void Update() => CurrentState.Update();

    public void FixedUpdate()
    {
        if(CurrentState.RunFixedUpdate) CurrentState.FixedUpdate();
    }
        
    public void LateUpdate()
    {
        if(CurrentState.RunLateUpdate) CurrentState.LateUpdate();
    }
}

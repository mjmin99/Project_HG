using UnityEngine;

public class StateMachine : MonoBehaviour
{
    public BaseState CurrentState { get; private set; }

    public void Initialize(BaseState InitialState)
    {
        CurrentState = InitialState;
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

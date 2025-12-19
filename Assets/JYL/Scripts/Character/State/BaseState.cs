using UnityEngine;

public abstract class BaseState
{
    public bool RunFixedUpdate { get; protected set; } = false;
    public bool RunLateUpdate { get; protected set; } = false;

    public abstract void Enter();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void LateUpdate();
    public abstract void Exit();
}

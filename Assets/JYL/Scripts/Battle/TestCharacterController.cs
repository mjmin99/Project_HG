using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TestCharacterController : MonoBehaviour
{
    private StateMachine stateMachine;
    private Dictionary<CharStateType, BaseState> states = new(); 

    public void Init()
    {
        stateMachine = new StateMachine();
        states.Add(CharStateType.Idle, new CharacterIdle(this) );
        states.Add(CharStateType.Run, new CharacterRun(this));
        states.Add(CharStateType.Attack, new CharacterAttack(this));
        states.Add(CharStateType.Skill, new CharacterSkill(this));
        states.Add(CharStateType.Hit, new CharacterHit(this));
        states.Add(CharStateType.Dead, new CharacterDead(this));
        stateMachine.Initialize(states[CharStateType.Idle]);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

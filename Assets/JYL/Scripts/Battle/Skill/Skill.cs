using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    StrongAttack, Parrying, AllHeal 
}

public abstract class Skill : MonoBehaviour
{
    [SerializeField] public SkillType skillType;
    [SerializeField] protected float returnTime = 2f;
    [SerializeField] protected float coolDown = 3f;

    private Stack<Skill> skillPool;

    protected float timer;
    
    protected Animator animator;
    private const string FIRE = "Fire";
    private const string AFTER = "After";
    private float afterTime;


    protected virtual void Update()
    {
        if (timer <= 0) return;
        
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ReturnToPool();
        }
    }

    public virtual void Init(Stack<Skill> skillPool)
    {
        animator = gameObject.GetOrAddComponent<Animator>();
        this.skillPool =  skillPool;
    }

    public virtual void SkillEffect()
    {
        timer = returnTime;
        gameObject.SetActive(true);
        animator.Play(FIRE);
        animator.Update(0f);
    }

    protected void ReturnToPool()
    {
        gameObject.SetActive(false);
        skillPool.Push(this);
    }
    public void AfterEffect()
    {
        animator.Play(AFTER);
        animator.Update(0f);
        if (afterTime <= 0f)
        {
            afterTime = animator.GetCurrentAnimatorStateInfo(0).length;
        }
        timer =  afterTime;
    }
}
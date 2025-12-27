using System;
using UnityEngine;
public class Skill : MonoBehaviour
{
    [SerializeField] public SkillType skillType;
    [SerializeField] public CharacterRole role;
    [SerializeField] private float parryTime = 0.5f;
    [SerializeField] private float destroyTime = 3f;

    public bool isParrying;
    private float timer;
    private Animator animator;

    private void Update()
    {
        if (!isParrying) return;
        
        parryTime -= Time.deltaTime;
        if (!(parryTime <= 0)) return;
        isParrying = false;
        Destroy(gameObject);
    }
    public void Init()
    {
        animator = gameObject.GetOrAddComponent<Animator>();
        Destroy(gameObject, destroyTime);
    }

    public void SkillEffect()
    {
        animator.Play("Fire");
        
        if (skillType != SkillType.Parrying) return;
        
        isParrying = true;
        timer = parryTime;
    }

    public void AfterEffect()
    {
        animator.Play("After");
    }
}

public enum SkillType
{
    StrongAttack, Parrying, AllHeal 
}

using System.Collections.Generic;
using UnityEngine;

public class Parrying : Skill
{
    [SerializeField] private float parryTime = 2f;

    public float stunTime = 5f;
    public bool isParrying;
    
    protected override void Update()
    {
        if (timer <= 0) return;
        timer -= Time.deltaTime;
        if (timer > 0) return;
        isParrying = false;
        ReturnToPool();
    }
    
    public override void Init(Stack<Skill> skillPool)
    {
        base.Init(skillPool);
        skillType = SkillType.Parrying;
    }

    public override void SkillEffect()
    {
        timer = parryTime;
        gameObject.SetActive(true);
        animator.Play("Fire");
        isParrying = true;
    }

    public void SuccessParry()
    {
        Debug.Log("패리 성공함");
        isParrying = false;
        AfterEffect();
    }
    
}

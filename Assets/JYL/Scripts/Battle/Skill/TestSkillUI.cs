using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TestSkillUI : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Button[] skillButton;
    [SerializeField] private TMP_Text[] btnText;
    
    
    public void Init(List<SkillCounter> skills)
    {
        btnText[0].SetText($"{skills[0].type} : {skills[0].skillCount}");
        btnText[1].SetText($"{skills[1].type} : {skills[1].skillCount}");
        btnText[2].SetText($"{skills[2].type} : {skills[2].skillCount}");
        skillButton[0].OnClickAsObservable().Subscribe(x => battleManager.OnClickSkills(skills[0].charId, 0)).AddTo(this);
        skillButton[1].OnClickAsObservable().Subscribe(_ => battleManager.OnClickSkills(skills[1].charId, 1)).AddTo(this);
        skillButton[2].OnClickAsObservable().Subscribe(_ => battleManager.OnClickSkills(skills[2].charId, 2)).AddTo(this);
    }

    public void SetTxt(int num, string text) => btnText[num].SetText(text);
    
}

using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SkillPresenter : MonoBehaviour
{
    [Header("Set Manager")]
    [SerializeField] private BattleManager battleManager;

    [Header("Set Skill Button")] 
    [SerializeField] public GameObject[] skillButtonPanel;
    [SerializeField] private Button[] skillButton;
    [SerializeField] private TMP_Text[] btnText;
    [SerializeField] private Image[] skillForwardImage;
    [SerializeField] private Image[] skillBackImage;
    
    [Header("Set Rewind")]
    [SerializeField] private Button rewindButton;
    [SerializeField] private TMP_Text rewindTimeText;
    [SerializeField] private Image rewindFillImage;
    [SerializeField] private GameObject rewindPanel;

    private float rewindCoolDown;
    private float rewindCoolTimer;
    private float rewindTimer;

    private float[] skillCooldown;
    private float[] skillTimer;
    private float[] skillCount;

    private void Update()
    {
        if (rewindTimer > 0f)
        {
            rewindTimer -= Time.deltaTime;
            if (rewindTimer <= 0f)
            {
                rewindPanel.SetActive(false);
            }
        }

        if (rewindCoolTimer > 0f)
        {
            rewindCoolTimer -= Time.deltaTime;
            UpdateRewindUI();
        }

        if (skillCooldown == null) return;
        
        for(int i = 0; i< skillCooldown.Length; i++)
        {
            if (!(skillTimer[i] > 0f)) continue;
            skillTimer[i] -= Time.deltaTime;
            UpdateSkillUI(i);
        }
    }
    
    public void Init(List<SkillInfo> skills)
    {
        skillCooldown = new float[skills.Count];
        skillTimer = new float[skills.Count];
        skillCount = new float[skills.Count];
        
        for (int i = 0; i < skillButtonPanel.Length; i++)
        {
            skillButtonPanel[i].SetActive(i < skills.Count);
            
            if (i >= skills.Count) continue; // 캐릭 숫자만큼만 세팅함
            
            btnText[i].SetText($"{skills[i].type} : {skills[i].skillCount}");
            skillCooldown[i] = skills[i].skillCooldown;
            
            skillForwardImage[i].sprite = skills[i].skillIcon;
            skillForwardImage[i].fillAmount = skills[i].skillCount.Value > 0 ? 1f : 0f;
            skillBackImage[i].sprite = skills[i].skillIcon;
            
            int index = i;
            skillButton[i].OnClickAsObservable().Subscribe(x => battleManager.OnClickSkills(skills[index].charId, 0)).AddTo(this);
            skillButton[i].OnClickAsObservable().Subscribe(x => OnClickSkillButton(index)).AddTo(this);
            skills[i].skillCount.Subscribe(x =>
                    SetSkillButtonInteractable(index, x, 
                        battleManager.skillDict[skills[index].charId].isDead.Value))
                .AddTo(this);

            if (skills[i].skillCount.Value <= 0)
            {
                skillButton[i].interactable = false;
            }
        }
        
        rewindButton.OnClickAsObservable().Subscribe(_ => battleManager.RewindTime()).AddTo(this);
        rewindPanel.SetActive(false);
    }

    public void SetTxt(int num, string text) => btnText[num].SetText(text);

    private void SetSkillButtonInteractable(int index, int amount, bool isDead)
    {
        skillCount[index] = amount;
        if (isDead)
        {
            skillButton[index].interactable = false;
            skillForwardImage[index].fillAmount = 0f;
            skillTimer[index] = 0f;
        }
        else if (amount > 0 && skillTimer[index] <= 0f)
        {
            skillForwardImage[index].fillAmount = 1f;
            skillButton[index].interactable = true;
        }
        else if(amount == 0 && skillTimer[index] <= 0f)
        {
            skillForwardImage[index].fillAmount = 0f;
            skillButton[index].interactable = false;
        }
    }

    public void SetRewind(float rewindTime, float amount)
    {
        if(rewindCoolDown <= 0f) rewindCoolDown = amount;
        rewindCoolTimer = rewindCoolDown;
        rewindTimer = rewindTime * 0.66f;
        rewindButton.interactable = false;
        rewindPanel.SetActive(true);
    }

    private void OnClickSkillButton(int index)
    {
        if (skillTimer[index] > 0f) return;
        skillTimer[index] = skillCooldown[index];
        skillButton[index].interactable = false;
    }
    
    private void UpdateSkillUI(int index)
    {
        float amount = (skillCooldown[index] - skillTimer[index]) / skillCooldown[index];
        
        if (Mathf.Abs(1 - amount) < 0.01f)
        {
            skillTimer[index] = 0f;
            skillForwardImage[index].fillAmount = 1f;
            if(skillCount[index] > 0) skillButton[index].interactable = true;
        }
        else
        {
            skillForwardImage[index].fillAmount = amount;
        }
    }

    private void UpdateRewindUI()
    {
        float amount = (rewindCoolDown - rewindCoolTimer) / rewindCoolDown;

        if (Mathf.Abs(amount - 1) < 0.01f)
        {
            rewindCoolTimer = 0f;
            rewindButton.interactable = true;
            rewindFillImage.fillAmount = 1f;
            rewindTimeText.SetText("");
        }
        else
        {
            rewindFillImage.fillAmount = amount;
            rewindTimeText.SetText(rewindCoolTimer.ToString("F1"));
        }
    }
}

using TMPro;
using UnityEngine;

public class BossHpPresenter : EnemyHpPresenter
{
    [SerializeField] private TMP_Text bossHpText;
    
    public override void UpdateUI(float amount)
    {
        base. UpdateUI(amount);
        if (amount <= 0.01f)
        {
            if(gameObject.activeSelf) gameObject.SetActive(false);
        }
        bossHpText.SetText($"{amount} / {maxHp}");
    }
}

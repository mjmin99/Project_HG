using TMPro;
using UnityEngine;

public class BossHpPresenter : EnemyHpPresenter
{
    [SerializeField] private TMP_Text bossHpText;
    
    public override void UpdateUI(float amount)
    {
        base. UpdateUI(amount);
        bossHpText.SetText($"{amount} / {maxHp}");
        if (amount <= 0.001f)
        {
            Destroy(gameObject);
        }
    }
}

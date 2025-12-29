using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class EnemyHpPresenter : MonoBehaviour
{
    [SerializeField] protected TMP_Text nameText;
    [SerializeField] protected Slider hpBar;

    protected float maxHp;
    
    public void Init(string enemyName, float maxHp)
    {
        nameText.SetText(enemyName);
        this.maxHp = maxHp;
        UpdateUI(maxHp);
    }

    public virtual void UpdateUI(float amount)
    {
        hpBar.value = amount / maxHp;
    }
}

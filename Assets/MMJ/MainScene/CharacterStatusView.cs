using TMPro;
using UnityEngine;

/// <summary>
/// 테스트용 View 스크립트 자세히 볼 필요 없음
/// </summary>
public class CharacterStatusView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text attackText;

    public void UpdateView(string name, int hp, int atk)
    {
        nameText.text = name;
        hpText.text = "HP : " + hp;
        attackText.text = "ATK : " + atk;
    }

    public void UpdateEmpty()
    {
        nameText.text = "-";
        hpText.text = "HP : -";
        attackText.text = "ATK : -";
    }
}

using TMPro;
using UnityEngine;

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
}

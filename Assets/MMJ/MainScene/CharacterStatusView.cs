using TMPro;
using UnityEngine;

public class CharacterStatusView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text atkText;

    public void UpdateView(string name, float hp, float atk)
    {
        nameText.text = name;
        hpText.text = $"HP: {hp}";
        atkText.text = $"ATK: {atk}";
    }
}

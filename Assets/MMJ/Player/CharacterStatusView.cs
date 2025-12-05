using TMPro;
using UnityEngine;

public class CharacterStatusView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text adText;

    public void UadateView(CharacterModel model)
    {
        if (model == null) return;

        nameText.text = model.name;
        hpText.text = $"HP : {model.hp}";
        adText.text = $"AD : {model.ad}";
    }
}

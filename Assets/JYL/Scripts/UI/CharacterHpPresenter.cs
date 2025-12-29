using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CharacterHpPresenter : MonoBehaviour
{
    [SerializeField] private GameObject[] charPanel;
    [SerializeField] private TMP_Text[] charNameText;
    [SerializeField] private TMP_Text[] charHpText;
    [SerializeField] private Image[] charIcon;
    [SerializeField] private Image[] deadImage;

    private float[] maxHp;

    public void Init()
    {
        int charCount = Manager.Game.Characters.Count;
        maxHp = new float[charCount];
        for (int i = 0; i < charPanel.Length; i++)
        {
            charPanel[i].SetActive(i<charCount); // 파티 구성원 수가 3명 이하일 경우 비활성화
            
            if (i >= charCount) continue; // 파티 구성원 수 만큼만 로직수행
            
            deadImage[i].gameObject.SetActive(false);
            
            var character = Manager.Game.Characters[i];
            var model = Manager.Character.models[character.characterId];
            
            maxHp[i] = character.maxHp;
            charIcon[i].sprite = model.Icon;
            charNameText[i].SetText(model.characterName);
            charHpText[i].SetText($"{maxHp[i]} /  {maxHp[i]}");
            
            
            int index = i;
            character.curHp.Subscribe(x=>UpdateHpUI(index, x)).AddTo(character);
            
        }
    }

    private void UpdateHpUI(int index, float amount)
    {
        if (amount <= 0f)
        {
            deadImage[index].gameObject.SetActive(true);
            charHpText[index].SetText($"0 / {maxHp[index]}");
        }
        else
        {
            deadImage[index].gameObject.SetActive(false);
            charHpText[index].SetText($"{amount} / {maxHp[index]}");
        }
    }
}

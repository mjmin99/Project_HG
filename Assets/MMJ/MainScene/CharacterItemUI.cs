using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterItemUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    public Button selectButton;

    private int characterId;

    private PartyUI cachedPartyUI; // 캐싱 변수 추가


    private void Awake()
    {
        cachedPartyUI = FindFirstObjectByType<PartyUI>();
    }

    public void Set(int id)
    {
        this.characterId = id;
        var model = CharacterManager.Instance.models[id];

        nameText.text = model.name;
        icon.sprite = model.Icon;

        selectButton.onClick.RemoveAllListeners();

        selectButton.onClick.AddListener(() =>
        {
            cachedPartyUI?.AssignCharacter(characterId);
        });
    }



    /* 이전 Set(int id) 함수
    public void Set(int id)
    {
        this.characterId = id;
        var model = CharacterManager.Instance.models[id];

        nameText.text = model.name;

        // 아이콘 로드 방식 변경
        Sprite sp = model.Icon;
        // Sprite sp = Resources.Load<Sprite>($"Icons/{model.name}");

        icon.sprite = sp;

        selectButton.onClick.AddListener(() =>
        {
            cachedPartyUI?.AssignCharacter(characterId);
        });
    }
    */
}

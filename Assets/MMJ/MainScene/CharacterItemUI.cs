using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterItemUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;

    [Header("Buttons")]
    public Button btnInfo;
    public Button btnAssign;

    private int characterId;
    private PartyUI cachedPartyUI;
    private CharacterDetailPanel cachedDetailPanel;

    public void Setup(int id, PartyUI partyUI, CharacterDetailPanel detailPanel)
    {
        characterId = id;
        cachedPartyUI = partyUI;
        cachedDetailPanel = detailPanel;

        var model = CharacterManager.Instance.models[id];

        // name → characterName으로 변경된 상태 반영
        nameText.text = model.characterName;
        icon.sprite = model.Icon;

        // 리스너 누적 방지
        btnInfo.onClick.RemoveAllListeners();
        btnAssign.onClick.RemoveAllListeners();

        // 1) 정보 버튼 → 상세창 열기
        btnInfo.onClick.AddListener(() =>
        {
            var panel = UIManager.Instance
                .OpenUI<CharacterDetailPanel>("CharacterDetailPanel");

            panel.SetCharacter(characterId);
        });

        // 2) 배치 버튼 → 파티 배치
        btnAssign.onClick.AddListener(() =>
        {
            if (cachedPartyUI == null)
            {
                Debug.LogWarning("PartyUI를 찾을 수 없습니다.");
                return;
            }

            // PartyUI가 activeSlotIndex를 가지고 있으므로
            // 슬롯 선택 안 했으면 PartyUI에서 경고 처리되게!
            cachedPartyUI.AssignCharacter(characterId);
        });
    }
}

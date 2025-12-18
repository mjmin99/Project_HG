using TMPro;
using UnityEngine;

/// <summary>
/// 플레이어 상태 표시 패널 (Resources 로드용)
/// </summary>
public class PlayerStatusPanel : UIPanel
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statText;

    public void SetData(PlayerData data)
    {
        nameText.text = data.name;
        statText.text = $"HP {data.hp} / ATK {data.attack}";
    }

    public override void OnOpen()
    {
        Debug.Log("[PlayerStatusPanel] Open");
    }

    public override void OnClose()
    {
        Debug.Log("[PlayerStatusPanel] Close");
        base.OnClose();             // 닫을땐 항상 오버라이 여기서 정해줘야함
    }
}

using UnityEngine;

public class PartyPanel : UIPanel
{
    private PartyUI partyUI;

    public override void OnOpen()
    {
        base.OnOpen();

        partyUI = GetComponentInChildren<PartyUI>(true);
        if (partyUI == null)
        {
            Debug.LogError("[PartyPanel] PartyUI not found in children.");
            return;
        }

        // UIManager가 CurrentPartyUI를 따로 들고 있어야 한다면 유지 (존재 확인됨) :contentReference[oaicite:3]{index=3}
        UIManager.Instance.RegisterPartyUI(partyUI);

        // 구버전 LoadParty ❌ → 이벤트 기반 RefreshAll ⭕
        partyUI.RefreshAll();
    }

    public override void OnClose()
    {
        if (partyUI != null)
            UIManager.Instance.UnregisterPartyUI(partyUI);

        base.OnClose();
    }
}

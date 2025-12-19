public class PartyPanel : UIPanel
{
    public PartyUI partyUI;

    public override void OnOpen()
    {
        base.OnOpen();
        UIManager.Instance.RegisterPartyUI(partyUI);
        partyUI.LoadParty();
    }

    public override void OnClose()
    {
        UIManager.Instance.UnregisterPartyUI(partyUI);
        base.OnClose();
    }
}

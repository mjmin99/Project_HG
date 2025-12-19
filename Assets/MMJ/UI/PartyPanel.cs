public class PartyPanel : UIPanel
{
    public PartyUI partyUI;

    public override void OnOpen()
    {
        base.OnOpen();
        partyUI.LoadParty();
    }
}

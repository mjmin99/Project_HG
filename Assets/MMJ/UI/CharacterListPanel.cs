public class CharacterListPanel : UIPanel
{
    public CharacterListUI listUI;

    public override void OnOpen()
    {
        base.OnOpen();
        listUI.Refresh();
    }
}

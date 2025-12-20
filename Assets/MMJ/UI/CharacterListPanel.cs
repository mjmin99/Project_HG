using UnityEngine;

public class CharacterListPanel : UIPanel
{
    private CharacterListUI listUI;

    public override void OnOpen()
    {
        base.OnOpen();

        listUI = GetComponentInChildren<CharacterListUI>(true);
        if (listUI == null)
        {
            Debug.LogError("[CharacterListPanel] CharacterListUI not found in children.");
            return;
        }

        listUI.Refresh();
    }
}

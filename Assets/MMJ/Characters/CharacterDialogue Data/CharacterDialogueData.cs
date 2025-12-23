using UnityEngine;

[CreateAssetMenu(
    fileName = "CharacterDialogueData",
    menuName = "Game/Character Dialogue Data"
)]
public class CharacterDialogueData : ScriptableObject
{
    [TextArea(2, 3)]
    public string[] dialogues;

    public string GetRandomDialogue()
    {
        if (dialogues == null || dialogues.Length == 0)
            return "...";

        return dialogues[Random.Range(0, dialogues.Length)];
    }
}

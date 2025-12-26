using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;

    public void SetText(string text)
    {
        dialogueText.text = text;
    }
}

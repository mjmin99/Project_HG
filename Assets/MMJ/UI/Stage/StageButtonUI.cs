using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text stageText;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject clearIcon;

    private int world;
    private int stage;
    private System.Action<int, int> onClick;

    public void Bind(
        int world,
        int stage,
        bool canEnter,
        bool isCleared,
        System.Action<int, int> onClick)
    {
        this.world = world;
        this.stage = stage;
        this.onClick = onClick;

        stageText.text = $"{world}-{stage}";
        lockIcon.SetActive(!canEnter);
        clearIcon.SetActive(isCleared);

        button.interactable = canEnter;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke(world, stage));
    }
}
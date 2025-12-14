using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Button button;

    public void Init(int world, Action onClick)
    {
        label.text = $"월드 {world}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}

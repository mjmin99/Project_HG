using UnityEngine;
using UnityEngine.UI;

public class StageSelectUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelRoot;

    [SerializeField] Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        panelRoot.SetActive(false);
    }

    public void Open()
    {
        panelRoot.SetActive(true);
    }

    public void Close()
    {
        panelRoot.SetActive(false);
    }
}

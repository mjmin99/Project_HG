using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Roots")]
    [SerializeField] private Transform panelRoot;
    [SerializeField] private Transform popupRoot;
    [SerializeField] private Transform toastRoot;

    private Stack<UIBase> uiStack = new();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiStack.Count > 0) CloseTop();
            else OpenUI<UIPopup>("OptionPopup");
        }
    }

    // GameObject로 로드 -> Instantiate -> GetComponent<T>
    public T OpenUI<T>(string key) where T : UIBase
    {
        string path = GetResourcePath<T>(key);

        GameObject prefabGO = Resources.Load<GameObject>(path);
        if (prefabGO == null)
        {
            Debug.LogError($"[UIManager] UI Prefab not found: {path}");
            return null;
        }

        GameObject instanceGO = Instantiate(
            prefabGO,
            GetParentForType<T>(),
            false
        );

        // UI 계층에 들어왔는지 확실히 보정
        if (instanceGO.transform is RectTransform rt)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
            rt.SetAsLastSibling(); // 제일 위로
        }

        T ui = instanceGO.GetComponent<T>();
        if (ui == null)
        {
            Debug.LogError($"[UIManager] Component {typeof(T).Name} missing on prefab root: {path}");
            Destroy(instanceGO);
            return null;
        }

        uiStack.Push(ui);
        ui.OnOpen();
        return ui;
    }

    private Transform GetParentForType<T>() where T : UIBase
    {
        // UIPopup 자신/자식 모두 true 처리
        if (typeof(UIPopup).IsAssignableFrom(typeof(T)))
            return popupRoot;

        if (typeof(UIPanel).IsAssignableFrom(typeof(T)))
            return panelRoot;

        return panelRoot;
    }

    private string GetResourcePath<T>(string key) where T : UIBase
    {
        if (typeof(UIPopup).IsAssignableFrom(typeof(T)))
            return $"UI/Popups/{key}";

        if (typeof(UIPanel).IsAssignableFrom(typeof(T)))
            return $"UI/Panels/{key}";

        return $"UI/{key}";
    }

    public void CloseTop()
    {
        if (uiStack.Count == 0) return;

        UIBase top = uiStack.Peek();
        if (!top.CanCloseByESC) return;

        uiStack.Pop();
        top.OnClose(); // Destroy는 UIBase가 담당
    }


    public void ShowToast(string key, string message, float duration = 2f)
    {
        string path = $"UI/Toasts/{key}";
        GameObject prefabGO = Resources.Load<GameObject>(path);

        if (prefabGO == null)
        {
            Debug.LogError($"[UIManager] Toast Prefab not found: {path}");
            return;
        }

        GameObject go = Instantiate(prefabGO, toastRoot, false);
        if (go.TryGetComponent<UIToast>(out var toast))
        {
            toast.Show(message, duration);
        }
        else
        {
            Debug.LogError($"[UIManager] UIToast component missing on: {path}");
            Destroy(go);
        }
    }
}

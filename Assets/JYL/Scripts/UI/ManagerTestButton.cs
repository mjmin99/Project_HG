using UnityEngine;
using UnityEngine.UI;

public class ManagerTestButton : MonoBehaviour
{
    private Button testButton;

    void Awake()
    {
        testButton = GetComponent<Button>();
        testButton.onClick.AddListener(()=>_ = Manager.Dialog.StartDialog(DialogKey.Test1));
    }
    
}

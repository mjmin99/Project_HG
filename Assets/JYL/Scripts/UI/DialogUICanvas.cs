using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogUICanvas : MonoBehaviour
{
    [Header("Set UI References")] 
    public Image fadeImage;
    public GameObject backGroundPanel;
    public Image backgroundImage;
    public GameObject portraitContent;
    public PortraitUIPanel portraitUIPanel;
    public TMP_Text nameField;
    public TMP_Text dialogText;
    public Button skipButton;
    
    public void Init()
    {
        backGroundPanel.gameObject.SetActive(false);
        fadeImage.gameObject.SetActive(false);
        
        skipButton.OnClickAsObservable().Subscribe(_ => Manager.Dialog.DialogSkip());
    }
}

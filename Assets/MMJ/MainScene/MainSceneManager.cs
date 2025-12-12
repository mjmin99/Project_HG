using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    public TMP_Text goldText;
    public Button shopButton;

    private void Awake()
    {
        shopButton.onClick.AddListener(GoToShop);
    }

    private void Start()
    {
        UpdateGoldUI();

        Debug.Log("<color=lime>MainScene 시작</color>");

        // 보유 캐릭터 로그 찍어보기
        foreach (var pair in CharacterManager.Instance.instances)
        {
            var inst = pair.Value;
            var model = CharacterManager.Instance.models[inst.id];
            string ownedStr = inst.isOwned ? "보유" : "미보유";
            Debug.Log($"캐릭터 id={inst.id}, name={model.name}, 상태={ownedStr}, 레벨={inst.level}, 별={inst.star}");
        }

        // 여기서 PartyUI.Initialize() 같은거 호출하면 됨
    }

    public void UpdateGoldUI()
    {
        int gold = SaveManager.Instance.CurrentData.gold;
        goldText.text = gold.ToString();
    }

    public void GoToShop() // 배틀씬 버튼 처럼 나중에 버튼에 직접 달아서 움직이게 역할 주는 것도 괜찮을듯
    {
        SceneManager.LoadScene("ShopScene");
        // 또는 SceneChanger.Instance.LoadScene("ShopScene");
    }
}

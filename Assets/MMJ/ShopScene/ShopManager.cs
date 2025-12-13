using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public TMP_Text goldText;
    public ResultPanelController resultPanel;
    public Button goMainSceneButton;
    public Button drawOneButton;

    private void Awake()
    {
        goMainSceneButton.onClick.AddListener(BackToMain);
        drawOneButton.onClick.AddListener(OnClickDrawOne);
    }

    private void Start()
    {
        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        goldText.text = SaveManager.Instance.CurrentData.gold.ToString();
    }

    public void OnClickDrawOne()
    {
        int cost = 50;

        if (!SaveManager.Instance.TrySpendGold(cost))
        {
            Debug.Log("골드 부족");
            return;
        }

        UpdateGoldUI();

        int id = DrawCharacter();
        CharacterModel model = CharacterManager.Instance.models[id];
        bool isNew = !CharacterManager.Instance.instances.ContainsKey(id)
                     || !CharacterManager.Instance.instances[id].isOwned;

        CharacterManager.Instance.GiveCharacter(id);
        SaveManager.Instance.SaveCurrentUser();

        resultPanel.Show(model, isNew);
    }

    int DrawCharacter()
    {
        var models = CharacterManager.Instance.models.Values.ToList();

        if (models.Count == 0)
        {
            Debug.LogError("캐릭터 모델이 비어있음!");
            return -1;
        }

        float totalWeight = models.Sum(m => GetWeight(m.rarity));
        float rand = Random.Range(0, totalWeight);
        float cumulative = 0f;

        foreach (var m in models)
        {
            cumulative += GetWeight(m.rarity);
            if (rand < cumulative)
                return m.id;
        }
        return models.OrderByDescending(m => m.rarity).First().id;
    }

    float GetWeight(int rarity)
    {
        // 너무 확률이 높으면 재미없고, 너무 낮으면 유저가 빡침ㅋ
        switch (rarity)
        {
            case 1: return 60f; // 1성 노말급
            case 2: return 25f; // 2성 희귀
            case 3: return 10f; // 3성 SR
            case 4: return 4f; // 4성 SUR
            case 5: return 1f; // 오성 과 한음
        }
        return 1f;
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("MainScene");
        // 또는 SceneChanger.Instance.LoadScene("MainScene");
    }

}

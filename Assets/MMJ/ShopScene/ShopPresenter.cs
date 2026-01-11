using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopPresenter : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text goldText;
    public ResultPanelController resultPanel;

    [Header("Buttons")]
    public Button goMainSceneButton;
    public Button drawOneButton;

    private const int DRAW_ONE_COST = 50;

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
        goldText.text = Manager.Save.CurrentData.gold.ToString();
    }

    public void OnClickDrawOne()
    {
        if (!Manager.Save.TrySpendGold(DRAW_ONE_COST))
        {
            Debug.Log("[ShopManager] 골드 부족");
            return;
        }

        UpdateGoldUI();

        int id = DrawCharacter();
        CharacterModel model = Manager.Character.models[id];

        bool isNew = !Manager.Character.instances.ContainsKey(id)
                     || !Manager.Character.instances[id].isOwned;

        Manager.Character.GiveCharacter(id);
        Manager.Save.PatchGold();
        Manager.Save.PatchCharacter(id);

        resultPanel.Show(model, isNew);
    }

    int DrawCharacter()
    {
        var models = Manager.Character.models.Values.ToList();

        if (models.Count == 0)
        {
            Debug.LogError("[ShopManager] 캐릭터 모델이 비어있음!");
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
        switch (rarity)
        {
            case 1: return 60f;
            case 2: return 25f;
            case 3: return 10f;
            case 4: return 4f;
            case 5: return 1f;
            default: return 1f;
        }
    }

    public void BackToMain()
    {
        // SceneManager.LoadScene("MainScene");
        // 어드레서블 수정 중
        Addressables.LoadSceneAsync("Scene/Main");
    }
}
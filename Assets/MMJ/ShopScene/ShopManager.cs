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
        // 세이브데이터를 캐릭터매니저에 다시 로드
        var data = SaveManager.Instance.CurrentData;

        if (data != null)
            CharacterManager.Instance.LoadFromSaveData(data);

        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        goldText.text = SaveManager.Instance.CurrentData.gold.ToString();
    }

    public void OnClickDrawOne()
    {
        int cost = 50;

        // 골드체크
        if (SaveManager.Instance.CurrentData.gold < cost)
        {
            Debug.Log("골드 부족");
            return;
        }

        // 골드 차감
        SaveManager.Instance.CurrentData.gold -= cost;

        // 랜덤 선택
        int id = DrawCharacter();  
        CharacterModel model = CharacterManager.Instance.models[id];

        bool isNew = false;

        // 신규/중복 판단
        if (!CharacterManager.Instance.instances.ContainsKey(id) ||
            !CharacterManager.Instance.instances[id].isOwned)
        {
            isNew = true;
        }

        // 캐릭터 획득 처리 (신규/중복 반영)
        CharacterManager.Instance.GiveCharacter(id);

        // 저장
        SaveManager.Instance.SaveCurrentUser();

        // 결과창 표시
        resultPanel.Show(model, isNew);
    }

    int DrawCharacter()
    {
        // rarity 기반 가중치 랜덤 처리
        var models = CharacterManager.Instance.models.Values.ToList();

        float totalWeight = models.Sum(m => GetWeight(m.rarity));
        float rand = Random.Range(0, totalWeight); // 랜덤 알고리즘 추가해서 기술문서로 만들어보자. 유니티 랜덤도 있음. C#의 랜덤과 유니티 랜덤의 비교를 해보자
        float cumulative = 0f;

        foreach (var m in models)
        {
            cumulative += GetWeight(m.rarity);
            if (rand <= cumulative)
                return m.id;
        }

        return models[0].id;
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

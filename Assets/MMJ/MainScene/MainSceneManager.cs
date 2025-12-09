using UnityEngine;

public class MainSceneManager : MonoBehaviour
{
    private void Start()
    {
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
}

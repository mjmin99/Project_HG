using UnityEngine;
using UnityEngine.UI;

public class BattleSceneButton : MonoBehaviour
{
    [SerializeField] Button battleSceneButton;
    private void Awake()
    {
        battleSceneButton.onClick.AddListener(OnClickBattleStart);
    }



    public void OnClickBattleStart()
    {
        SceneChanger.Instance.LoadScene("BattleScene");
    }
}

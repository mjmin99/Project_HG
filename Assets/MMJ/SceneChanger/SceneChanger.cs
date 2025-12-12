using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    private void Awake()
    {
        Debug.Log("SceneChanger Awake 실행. Instance=" + Instance);

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("SceneChanger Instance 등록 완료");
        }
        else 
        {
            Debug.Log("SceneChanger 중복 제거됨");
            Destroy(gameObject);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int buildIndex) // 인덱스로 전환하기 위해 오버로드 용 함수 
    {
        SceneManager.LoadScene(buildIndex);
    }
}

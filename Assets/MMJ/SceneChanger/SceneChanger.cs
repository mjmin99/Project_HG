using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(string scenename)
    { 
        SceneManager.LoadScene(scenename);
    }

    public void LoadScene(int buildIndex) // 인덱스로 전환하기 위해 오버로드 용 함수 
    {
        SceneManager.LoadScene(buildIndex);
    }
}

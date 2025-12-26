using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    protected static T Instance
    {
        get
        {
            if (instance != null) return instance;
            instance = FindFirstObjectByType<T>();
            if (instance != null) return instance;
            instance = new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(instance.gameObject);
            return instance;
        }
    }
    protected virtual void Awake()
    {
        Init();
    }

    private void Init()
    {
        if (instance == null)
        {
            instance = this as T;
            if(!gameObject) new GameObject(typeof(T).Name).AddComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    protected void OnApplicationQuit()
    {
        instance = null;
    }


    public virtual void DestroyManager()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
    }
}
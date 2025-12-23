using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestBattleManager : MonoBehaviour
{
    private List<TestCharacterController> characters;
    private List<TestEnemyController> enemies;
    [SerializeField] private TestGameManager gameManager;

    private void Awake()
    {
        characters = gameManager.GetParty().ToList();
        foreach (var c in characters)
        {
            if(c.gameObject.activeSelf) c.Init(8f);
        }
        enemies = gameManager.GetEnemies().ToList();
        foreach (var c in enemies)
        {
            if (c.gameObject.activeSelf) c.Init();
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
            TestTimeRewind();
    }

    public void TestTimeRewind()
    {
        foreach (var c in characters)
        {
            if(c.gameObject.activeSelf) 
                c.RewindTime();
        }
    }
}

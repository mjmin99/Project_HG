using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestBattleManager : MonoBehaviour
{
    private List<TestCharacterController> characters;
    [SerializeField] private TestGameManager gameManager;
    
    void Awake()
    {
        characters = gameManager.GetParty().ToList();
        foreach (var c in characters)
        {
            c.Init(15f);
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

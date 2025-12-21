using System.Collections.Generic;
using UnityEngine;

public class TestBattleManager : MonoBehaviour
{
    [SerializeField] private List<TestCharacterController> characters;
    
    
    void Awake()
    {
        foreach (var c in characters)
        {
            c.Init();
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

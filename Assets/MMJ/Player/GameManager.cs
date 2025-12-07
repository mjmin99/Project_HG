using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SaveManager saveManager;
    public PlayerController[] players;

    private List<CharacterModelRuntime> characterDB;
    public Transform[] playerSpawnPoint;

    private void Awake()
    {
        characterDB = CharacterCSVLoader.LoadFromCSV();
    }


    private void Start()
    {
        LoadParty();
    }

    private void LoadParty()
    {
        int[] party = saveManager.CurrentData.partySet;

        for (int i = 0; i < players.Length; i++)
        {
            int id = party[i];

            if (id == -1)
            {
                // 이 자리는 빈 슬롯으로 처리
                players[i].ClearModel();    
                continue;
            }

            CharacterModelRuntime model = characterDB.Find(c => c.id == id);

            if (model == null)
            {
                Debug.LogError($"CSV에서 id={id} 찾지 못함!");
                players[i].ClearModel();
                continue;
            }

            players[i].ApplyModel(model);
        }
    }
}


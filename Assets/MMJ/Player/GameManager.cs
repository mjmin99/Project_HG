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

        for (int i = 0; i < 3; i++)
        {
            int id = party[i];
            CharacterModelRuntime model = characterDB.Find(c => c.id == id);

            players[i].ApplyModel(model);
        }
    }
}


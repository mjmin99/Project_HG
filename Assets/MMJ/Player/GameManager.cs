using DG.Tweening.Core.Easing;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SaveManager saveManager;
    public CharacterDatabase database;

    public Transform[] playerSpawnPoint;
    public PlayerController[] players;

    private void Start()
    {
        LoadParty();
    }

    private void LoadParty()
    {
        var party = saveManager.CurrentData.partySet;

        for (int i = 0; i < players.Length; i++)
        {
            int id = party[i];
            CharacterModel model = database.Get(id);

            players[i].ApplyModel(model);
        }
    }
}


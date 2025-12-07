using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int[] partySet = new int[3];

    public SaveData()
    {
        partySet[0] = -1;
        partySet[1] = 1;
        partySet[2] = 2;
    }
}

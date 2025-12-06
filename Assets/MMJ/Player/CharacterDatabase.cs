using UnityEngine;

[CreateAssetMenu(fileName = "CharcterDatabase", menuName = "Scriptable Objects/CharcterDatabase")]
public class CharacterDatabase : ScriptableObject
{
    public CharacterModel[] allCharacters;

    public CharacterModel Get(int id)
    {
        foreach (var c in allCharacters)
        {
            if (c.id == id)
            {
                return c;
            }
        }
        return null;
    }
}

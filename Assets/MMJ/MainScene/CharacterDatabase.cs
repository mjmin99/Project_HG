using UnityEngine;

/// <summary>
/// 현재는 CSV를 사용하고 있기때문에 전혀 사용하고 있지 않음
/// 하지만 CSV엔 데이터 / SO에는 리소스 처럼 분리해서 사용할 가능성이 있어서 냅둔상태
/// </summary>



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

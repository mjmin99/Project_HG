using UnityEngine;

[CreateAssetMenu(fileName = "CharacterModel", menuName = "Scriptable Objects/CharacterModel")]
public class CharacterModel : ScriptableObject
{
    [Header("기본 정보")]
    public int id;
    public int maxHP;
    public int attack;
    public string characterName;
    public GameObject prefab;// 씬에 보여줄 실제 모델
}

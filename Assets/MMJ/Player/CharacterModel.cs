using UnityEngine;

[CreateAssetMenu(fileName = "CharacterModel", menuName = "Scriptable Objects/CharacterModel")]
public class CharacterModel : ScriptableObject
{
    [Header("기본 정보")]
    public int id;
    public int hp;
    public int ad;
    public string name;


    public GameObject characterPrefab;   // 씬에 보여줄 실제 모델
}

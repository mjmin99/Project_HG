using UnityEngine;

public class Manager : Singleton<Manager>
{
    public static AudioManager Audio;
    public static DialogManager Dialog;
    public static CharacterManager Character;
    public static SaveManager Save;

    [RuntimeInitializeOnLoadMethod] // 런타임 실행 시 가장 먼저 수행됨
    protected override void Awake()
    {
        base.Awake();
        Save =  SaveManager.Instance;
        Audio = AudioManager.Instance;
        Dialog = DialogManager.Instance;
        Character = CharacterManager.Instance;
    }
}

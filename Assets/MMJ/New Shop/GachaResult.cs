using System;

[Serializable]
public class GachaResult
{
    public int characterId;
    public bool isNew;

    public GachaResult(int characterId, bool isNew)
    {
        this.characterId = characterId;
        this.isNew = isNew;
    }
}

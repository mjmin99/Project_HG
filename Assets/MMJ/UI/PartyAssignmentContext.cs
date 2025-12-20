public static class PartyAssignmentContext
{
    // "편성" 버튼을 눌러 슬롯 선택 UI로 넘어갈 때 보관하는 값
    public static int PendingCharacterId { get; private set; } = -1;

    public static bool HasPending => PendingCharacterId != -1;

    public static void Begin(int characterId)
    {
        PendingCharacterId = characterId;
    }

    public static void Clear()
    {
        PendingCharacterId = -1;
    }
}

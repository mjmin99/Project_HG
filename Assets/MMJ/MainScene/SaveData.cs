using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public SaveData(StageDataSO[] stages)
    {
        // 기본 생성자: 캐릭터 데이터를 CharacterManager에서 불러올 때 채워짐
        stageProgress = new(stages);
    }

    // 파티 구성 (3명)
    public int[] partySet = { -1, -1, -1 };

    // 보유 캐릭터들 (CharacterInstance 목록)
    public List<CharacterInstance> characters = new();

    // 재화 (옵션)
    public int gold = 0;
    public int gem = 0;

    // 스테이지 진행도
    public StageProgressData stageProgress;
}

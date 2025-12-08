using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int[] partySet = new int[3];

    /* 예시 작성 모음 +++++++ 나중에 각 캐릭터 별로 캐릭터 자체의 성장/보유 여부를 저장 할 필요도 있음

    계정 기본 정보
   public string nickname;
   public int playerLevel;
   public int gold;
   public int gem;

    파티 정보
   public int[] partySet = new int[3];

    보유 캐릭터 목록 (캐릭터 ID → 캐릭터 상태 데이터)
   public Dictionary<int, CharacterSaveInfo> characters = new Dictionary<int, CharacterSaveInfo>();

   // 스테이지 진행도
   public int clearedStage;
   */


    public SaveData()
    {
        partySet[0] = -1;
        partySet[1] = 1;
        partySet[2] = 2;
    }
}

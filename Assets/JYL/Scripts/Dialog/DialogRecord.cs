using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

[Serializable]
public struct DialogRecord
{
    public List<DialogFlag> dialogFlags;
    
    [NonSerialized]
    public Dictionary<DialogCondition, bool> conditions;

    public void Init()
    {
        ListInit();
        DictionaryInit();
    }
    public void ListInit()
    {
        dialogFlags ??= new List<DialogFlag>();
        foreach(DialogCondition e in  Enum.GetValues(typeof(DialogCondition)))
        {
            var hasFlag = dialogFlags.Any(flag => flag.condition == e);

            if (!hasFlag)
            {
                dialogFlags.Add(new DialogFlag
                {
                    condition = e,
                    isFlagged = false
                });
            }
        }
    }
    
    public void DictionaryInit() // 새로 생성 시
    {
        conditions = new Dictionary<DialogCondition, bool>();
        if (dialogFlags == null || dialogFlags.Count == 0)
        {
            ListInit();
        }

        if (dialogFlags == null)
        {
            Debug.LogWarning("다이얼로그 초기화 실패");
            return;
        }
        
        conditions = dialogFlags.ToDictionary(x => x.condition, x => x.isFlagged);
    }
    
    public bool CheckDialogCondition(DialogCondition condition)
    {
        if (conditions == null) DictionaryInit();
        
        if (conditions != null && conditions.TryGetValue(condition, out var result))
        {
            Debug.Log($"딕셔너리에 값 잇음{condition}");
            return result;
        }
        
        Debug.LogWarning("딕셔너리에 키가 저장안됐음: " + condition);
        return false;
    }

    public void MarkDialogCondition(DialogCondition condition)
    {
        if (conditions[condition])
        {
            Debug.LogWarning($"이미 true로 설정 되어 있음:{condition}");
        }
        
        conditions[condition] = true;
        int index = dialogFlags.FindIndex(x => x.condition == condition);
        dialogFlags[index] = new DialogFlag
        {
            condition = condition,
            isFlagged = true
        };
        Manager.Save.PatchDialogFlag(condition, true);
    }
}
[Serializable]
public struct DialogFlag
{
    public DialogCondition condition;
    public bool isFlagged;
}

[Serializable]
public enum DialogCondition
{
    IsFirstRun,
    IsFirstBase,
    EnterW1S1,
    WorldBoss1,
    WorldBoss1Down,
    EnterW2S1,
    WorldBoss2,
    EnterW3S1,
    WorldBoss3,
    EnterW4S1,
    WorldBoss4,
    EnterW5S1,
    WorldBoss5,
    WorldBoss5Down,
    
}

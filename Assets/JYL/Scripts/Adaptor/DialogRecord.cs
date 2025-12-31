using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogRecord
{
    public Dictionary<DialogCondition, bool> conditions;

    public void Init(bool isRecreation = false)
    {
        if (isRecreation)
        {
            var temp = conditions;
            conditions = new Dictionary<DialogCondition, bool>();
            foreach (DialogCondition condition in Enum.GetValues(typeof(DialogCondition)))
            {
                conditions.Add(condition, false);
            }
            foreach (var key in temp.Keys)
            {
                conditions[key] = temp[key];
            }
        }
        
        conditions = new Dictionary<DialogCondition, bool>();
        foreach (DialogCondition condition in Enum.GetValues(typeof(DialogCondition)))
        {
            conditions.Add(condition, false);
        }
    }
    
    public bool CheckDialogCondition(DialogCondition condition)
    {
        Debug.Log($"체크 들어옴{condition}");
        
        if (conditions == null) Init();
        
        else if (conditions.TryGetValue(condition, out var result))
        {
            Debug.Log($"딕셔너리에 값 잇음{condition}");
            return result;
        }
        
        Debug.LogError("딕셔너리에 키가 저장안됐음: " + condition);
        Init(true);
        Manager.Save.SaveCurrentUser();
        return false;
    }

    public void MarkDialogCondition(DialogCondition condition)
    {
        if (conditions[condition])
        {
            Debug.LogWarning($"이미 true로 설정 되어 있음:{condition}");
        }
        
        conditions[condition] = true;
    }
}

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

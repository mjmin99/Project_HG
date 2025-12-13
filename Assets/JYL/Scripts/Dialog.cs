using System;
using System.Collections.Generic;
using UnityEngine;

public record Dialog
{
    public readonly DialogKey dialogId;
    public readonly IReadOnlyList<DialogLine> dialogContents;

    public Dialog(DialogKey id, List<DialogLine> lines)
    {
        dialogId = id;
        dialogContents = lines;
    }
}

public record DialogLine
{
    public readonly DialogKey dialogId;
    public readonly string lineId;
    public readonly string speakerId;
    public readonly DialogType dialogType;
    public readonly BackgroundType backgroundType;
    public readonly string dialogContent;
    public readonly string sfxKey;

    public DialogLine(string[] csvData)
    {
        if (!Enum.TryParse(csvData[0], out dialogId))
        {
            Debug.LogWarning($"다이얼로그 아이디에 해당 아이디가 없음{csvData[0]}");
        }
        lineId = csvData[1];
        speakerId = csvData[2];
        if (!Enum.TryParse(csvData[3], out dialogType))
        {
            Debug.LogWarning($"다이얼로그 타입 변환 실패{csvData[3]}");
        }
        switch (dialogType)
        {
            case DialogType.ChangeBackground:
                if (!Enum.TryParse(csvData[4], out backgroundType))
                {
                    Debug.LogWarning($"배경 파싱 실패함{csvData[4]}");
                }
                break;
            case DialogType.PlaySfx:
                sfxKey = csvData[4];
                break;
            default:
                backgroundType = BackgroundType.None;
                dialogContent = csvData[4];
                break;
        }
    }
}
public enum DialogKey { None, Test1, Test2 }
public enum DialogType { DialogStart, ChangeBackground, CharacterIn, CharacterOut, NoVoice, WithVoice, PlaySfx}
public enum BackgroundType {None, Forest, City}
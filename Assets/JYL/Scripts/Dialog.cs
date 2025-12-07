using System;
using System.Collections.Generic;

public record Dialog
{
    public readonly string DialogId;
    public IReadOnlyList<DialogLine> DialogContents;

    public Dialog(string id, List<DialogLine> lines)
    {
        DialogId = id;
        DialogContents = lines;
    }
}

public record DialogLine
{
    public readonly string DialogId;
    public readonly string LineId;
    public readonly string SpeakerId;
    public readonly DialogType Type;
    public readonly string Content;

    public DialogLine(string[] csvData)
    {
        DialogId = csvData[0];
        LineId = csvData[1];
        SpeakerId = csvData[2];
        Type = Enum.Parse<DialogType>(csvData[3]);
        Content = csvData[4];
    }
}
public enum DialogType { CharacterIn, CharacterOut, NoVoice, WithVoice}

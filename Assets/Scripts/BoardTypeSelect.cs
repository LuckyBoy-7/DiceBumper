using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.UI.Interactive;
using UnityEngine;

public class BoardTypeSelect : Select
{
    public override string Content { get; } = "BoardType";
    public override string StartToggleName { get; } = "Rectangle";

    protected override void CreateItems()
    {
        foreach (Settings.BoardTypes boardType in typeof(Settings.BoardTypes).GetEnumValues())
        {
            CreateItem(boardType.ToString(), () => Settings.BoardType = boardType);
        }
    }
}
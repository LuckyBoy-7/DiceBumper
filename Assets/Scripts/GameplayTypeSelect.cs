using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.UI.Interactive;
using UnityEngine;

public class GameplayTypeSelect : Select
{
    public override string Content { get; } = "GameplayType";
    public override string StartToggleName { get; } = "Collision";

    protected override void CreateItems()
    {
        foreach (Settings.GameplayTypes gameplayType in typeof(Settings.GameplayTypes).GetEnumValues())
        {
            CreateItem(gameplayType.ToString(), () => Settings.GameplayType = gameplayType);
        }
    }
}
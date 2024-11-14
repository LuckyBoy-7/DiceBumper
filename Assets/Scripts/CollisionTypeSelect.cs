using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.UI.Interactive;
using UnityEngine;

public class CollisionTypeSelect : Select
{
    public override string Content { get; } = "CollisionType";
    public override string StartToggleName { get; } = "Square";

    protected override void CreateItems()
    {
        foreach (Settings.CollisionTypes collisionType in typeof(Settings.CollisionTypes).GetEnumValues())
        {
            CreateItem(collisionType.ToString(), () => Settings.CollisionType = collisionType);
        }
    }
}
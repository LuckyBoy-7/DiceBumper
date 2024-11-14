using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : ManagedBehaviour
{
    public static Indicator instance;
    public Dice follow;
    public Image transparentImage;
    public Image fillImage;

    private void Awake()
    {
        instance = this;
    }

    protected override void ManagedUpdate()
    {
        base.ManagedUpdate();
        if (follow == null)
            return;

        transform.position = follow.transform.position;
        if (follow.campType == Dice.CampType.Red)
        {
            transparentImage.color = Color.red.WithA(transparentImage.color.a);
            fillImage.color = Color.red;
        }
        else if (follow.campType == Dice.CampType.Blue)
        {
            transparentImage.color = Color.blue.WithA(transparentImage.color.a);
            fillImage.color = Color.blue;
        }

        float radians = MathUtils.SignedRadians(Vector2.right, follow.curDir);
        transform.eulerAngles = new Vector3(0, 0, MathUtils.RadiansToDegree(radians));
        fillImage.fillAmount = follow.shootForceRatio;
    }

    public void Show()
    {
        transparentImage.enabled = true;
        fillImage.enabled = true;
    }

    public void Hide()
    {
        transparentImage.enabled = false;
        fillImage.enabled = false;
    }
}
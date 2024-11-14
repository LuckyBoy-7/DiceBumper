using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Framework;
using Lucky.Framework.Inputs_;
using Lucky.Kits.Extensions;
using Lucky.Kits.Utilities;
using UnityEngine;

public class Dice : ManagedBehaviour
{
    private SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    public Collider2D circleCollider;
    public Collider2D squareCollider;

    public List<Sprite> sprites;
    public int points;

    public enum CampType
    {
        Red,
        Blue,
        Gray
    }

    public CampType campType;
    public Settings.GameplayTypes gameplayType;
    public Settings.CollisionTypes collisionType;

    public bool readyToShoot;
    public bool invincible;

    // rotate
    public VirtualIntegerAxis rotateButton;
    public Vector2 curDir;

    public float rotateSpeed;

    // shoot
    public VirtualButton shootButton;
    public float shootMinForce;
    public float shootMaxForce;
    public float shootForceRatio;
    public float chargeDuration;
    public int chargeDir = 1;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        Roll();
    }

    public void Init(CampType camp, Settings.CollisionTypes collision, Settings.GameplayTypes gameplay)
    {
        campType = camp;
        collisionType = collision;
        gameplayType = gameplay;
        switch (campType)
        {
            case CampType.Red:
                sr.color = Color.red;
                curDir = Vector2.right;
                rotateButton = Inputs.Player1Rotate;
                shootButton = Inputs.Player1Shoot;
                break;
            case CampType.Blue:
                sr.color = Color.blue;
                curDir = Vector2.left;
                rotateButton = Inputs.Player2Rotate;
                shootButton = Inputs.Player2Shoot;
                break;
            case CampType.Gray:
                sr.color = Color.gray;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(campType));
        }

        if (gameplayType == Settings.GameplayTypes.Collision)
            sr.color = ColorUtils.GetRandomColorExcept(Color.black);

        if (collisionType == Settings.CollisionTypes.Circle)
        {
            circleCollider.enabled = true;
            squareCollider.enabled = false;
        }
        else
        {
            circleCollider.enabled = false;
            squareCollider.enabled = true;
        }
    }

    protected override void ManagedUpdate()
    {
        base.ManagedUpdate();
        if (readyToShoot)
        {
            curDir = curDir.Rotate(rotateButton.Value * rotateSpeed * Timer.DeltaTime());
            invincible = true;
            if (shootButton.Check)
            {
                shootForceRatio += chargeDir * 1 / chargeDuration * Timer.DeltaTime();
                if (shootForceRatio > 1 || shootForceRatio < 0)
                {
                    shootForceRatio = MathUtils.Clamp(shootForceRatio, 0, 1);
                    chargeDir *= -1;
                }
            }
            else if (shootButton.Released)
            {
                print("Shoot");
                rb.AddForce(curDir * MathUtils.Lerp(shootMinForce, shootMaxForce, shootForceRatio), ForceMode2D.Impulse);
                readyToShoot = false;
                Indicator.instance.Hide();
                GameController.instance.WaitForNextTurn();
                invincible = false;
            }
        }
    }

    public void Roll()
    {
        points = RandomUtils.Range(1, 7);
        sr.sprite = sprites[points - 1];
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (gameplayType == Settings.GameplayTypes.Reroll)
        {
            if (campType == CampType.Gray || invincible)
                return;

            Dice dice = other.collider.GetComponent<Dice>();
            if (dice && !dice.invincible)
            {
                if (dice.campType == campType)
                {
                    Destroy(gameObject);
                    if (campType == CampType.Red)
                        GameController.instance.redPoints += points;
                    else
                        GameController.instance.bluePoints += points;
                }
                else if ((int)dice.campType == ((int)campType ^ 1))
                {
                    Roll();
                }
            }
        }
        else
        {
            if (campType == CampType.Gray || invincible)
                return;

            Dice dice = other.collider.GetComponent<Dice>();
            if (dice && !dice.invincible)
            {
                if (GameController.instance.curCamp == CampType.Red)
                    GameController.instance.redPoints += 1;
                else
                    GameController.instance.bluePoints += 1;
                points -= 1;
                if (points == 0)
                    Destroy(gameObject);
                else
                    sr.sprite = sprites[points - 1];
            }
        }
    }
}
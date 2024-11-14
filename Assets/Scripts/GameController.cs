using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucky.Framework;
using Lucky.Kits.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : ManagedBehaviour
{
    public static GameController instance;
    public Transform redSpawnPoint;
    public Transform blueSpawnPoint;
    public Dice.CampType curCamp;
    public Dice dicePrefab;
    public List<Dice> dices = new();
    public float regardStillThreshold = 5;
    public int redPoints, bluePoints;
    public TMP_Text redPointsText, bluePointsText;
    public List<Transform> graySpawnPoints = new();

    public GameObject rectangleBoard;
    public GameObject circleBoard;

    private void Awake()
    {
        instance = this;

        // 桌面类型
        if (Settings.BoardType == Settings.BoardTypes.Circle)
        {
            circleBoard.SetActive(true);
            rectangleBoard.SetActive(false);
        }
        else
        {
            circleBoard.SetActive(false);
            rectangleBoard.SetActive(true);
        }

        if (Settings.GameplayType == Settings.GameplayTypes.Reroll)
            foreach (var graySpawnPoint in graySpawnPoints)
            {
                Dice dice = Instantiate(dicePrefab, graySpawnPoint.position, Quaternion.identity);
                dice.Init(Dice.CampType.Gray, Settings.CollisionType, Settings.GameplayType);
            }
    }

    private void Start()
    {
        curCamp = RandomUtils.Random01() == 0 ? Dice.CampType.Red : Dice.CampType.Blue;
        // curCamp = Dice.CampType.Red;
        StartTurn();
    }

    protected override void ManagedUpdate()
    {
        base.ManagedUpdate();
        redPointsText.text = $"Red Points: {redPoints}";
        bluePointsText.text = $"Blue Points: {bluePoints}";

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
    }

    private void StartTurn()
    {
        print(curCamp == Dice.CampType.Red ? "Red First!" : "Blue First!");
        // todo: 梳理卡在出生点的骰子
        Dice dice = Instantiate(dicePrefab, curCamp == Dice.CampType.Red ? redSpawnPoint.position : blueSpawnPoint.position, Quaternion.identity);
        dice.Init(curCamp, Settings.CollisionType, Settings.GameplayType);
        dice.readyToShoot = true;
        Indicator.instance.follow = dice;
        Indicator.instance.Show();
        dice.invincible = true;
        dices.Add(dice);
    }

    public void WaitForNextTurn()
    {
        StartCoroutine(WaiForNextTurnCoroutine());
    }

    IEnumerator WaiForNextTurnCoroutine()
    {
        while (true)
        {
            dices.RemoveAll(dice => dice == null);
            if (dices.Any(dice => dice.rb.velocity.magnitude > regardStillThreshold))
            {
                yield return null;
                continue;
            }

            break;
        }

        NextTurn();
    }

    public void NextTurn()
    {
        curCamp = (Dice.CampType)((int)curCamp ^ 1);
        StartTurn();
    }
}
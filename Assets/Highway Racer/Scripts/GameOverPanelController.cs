using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOverPanelController : MonoBehaviour
{
    public GameObject content;

    [Header("UI Texts On Scoreboard")] public Text totalScore;
    public Text subTotalMoney;
    public Text totalMoney;

    public Text totalDistance;
    public Text totalNearMiss;
    public Text totalOverspeed;
    public Text totalOppositeDirection;

    public Text totalDistanceMoney;
    public Text totalNearMissMoney;
    public Text totalOverspeedMoney;
    public Text totalOppositeDirectionMoney;

    int Myscore;

    public int totalDistanceMoneyMP
    {
        get { return HR_HighwayRacerProperties.Instance._totalDistanceMoneyMP; }
    }

    public int totalNearMissMoneyMP
    {
        get { return HR_HighwayRacerProperties.Instance._totalNearMissMoneyMP; }
    }

    public int totalOverspeedMoneyMP
    {
        get { return HR_HighwayRacerProperties.Instance._totalOverspeedMoneyMP; }
    }

    public int totalOppositeDirectionMP
    {
        get { return HR_HighwayRacerProperties.Instance._totalOppositeDirectionMP; }
    }

    void OnEnable()
    {
        HR_PlayerHandler.OnPlayerDied += HR_PlayerHandler_OnPlayerDied;
    }

    void HR_PlayerHandler_OnPlayerDied(HR_PlayerHandler player)
    {
        StartCoroutine(DisplayResults(player));
    }

    public IEnumerator DisplayResults(HR_PlayerHandler player)
    {
        yield return new WaitForSecondsRealtime(1f);
        content.SetActive(true);
        Myscore = (int)player.score;
        totalScore.text = Mathf.Floor(player.score).ToString("F0");
        totalDistance.text = (player.distance).ToString("F2");
        totalNearMiss.text = (player.nearMisses).ToString("F0");
        totalOverspeed.text = (player.highSpeedTotal).ToString("F1");
        totalOppositeDirection.text = (player.opposideDirectionTotal).ToString("F1");

        totalDistanceMoney.text = Mathf.Floor(player.distance * totalDistanceMoneyMP).ToString("F0");
        totalNearMissMoney.text = Mathf.Floor(player.nearMisses * totalNearMissMoneyMP).ToString("F0");
        totalOverspeedMoney.text = Mathf.Floor(player.highSpeedTotal * totalOverspeedMoneyMP).ToString("F0");
        totalOppositeDirectionMoney.text =
            Mathf.Floor(player.opposideDirectionTotal * totalOppositeDirectionMP).ToString("F0");
        float sumScore = Mathf.Floor(player.score);
        int a = (int)sumScore;

        totalMoney.text = (Mathf.Floor(player.distance * totalDistanceMoneyMP) +
                           (player.nearMisses * totalNearMissMoneyMP) +
                           Mathf.Floor(player.highSpeedTotal * totalOverspeedMoneyMP) +
                           Mathf.Floor(player.opposideDirectionTotal * totalOppositeDirectionMP)).ToString("F0");
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency", 0) + a);
        Debug.Log(totalScore);

        gameObject.BroadcastMessage("Animate");
        gameObject.BroadcastMessage("GetNumber");
    }


    void OnDisable()
    {
        HR_PlayerHandler.OnPlayerDied -= HR_PlayerHandler_OnPlayerDied;
    }

    void Update()
    {
        if (PlayerPrefs.GetInt("reward", 0) == 2)
        {
            PlayerPrefs.SetInt("reward", 0);
        }
        else if (PlayerPrefs.GetInt("reward", 0) == 1)
        {
            PlayerPrefs.SetInt("reward", 0);
            Myscore += Myscore;
            PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency", 0) + Myscore);
            totalScore.text = Myscore + "";
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicScoreDisplay : MonoBehaviour
{
    #region SINGLETON PATTERN

    public static DynamicScoreDisplay _instance;

    public static DynamicScoreDisplay Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<DynamicScoreDisplay>();
            }

            return _instance;
        }
    }

    #endregion

    private Text scoreText;
    private Text[] scoreTextObjects;
    private int index = 0;

    private float lifeTime = 1f;
    private float timer = 0f;
    private Vector3 defaultPosition;

    public enum Side
    {
        Left,
        Right,
        Center
    }

    private AudioSource nearMissAudio;

    void Start()
    {
        scoreText = GetComponentInChildren<Text>();
        scoreText.gameObject.SetActive(false);

        scoreTextObjects = new Text[10];

        for (int i = 0; i < 10; i++)
        {
            GameObject instantiatedText = GameObject.Instantiate(scoreText.gameObject, transform);
            scoreTextObjects[i] = instantiatedText.GetComponent<Text>();
            scoreTextObjects[i].color = new Color(scoreTextObjects[i].color.r, scoreTextObjects[i].color.g, scoreTextObjects[i].color.b, 0f);
            scoreTextObjects[i].gameObject.SetActive(true);
        }

        timer = 0f;
        defaultPosition = scoreTextObjects[0].transform.position;
    }

    void OnEnable()
    {
        HR_PlayerHandler.OnNearMiss += HR_PlayerHandler_OnNearMiss;
    }

    void HR_PlayerHandler_OnNearMiss(HR_PlayerHandler player, int score, Side side)
    {
        switch (side)
        {
            case Side.Left:
                DisplayScore(score, -75f);
                break;

            case Side.Right:
                DisplayScore(score, 75f);
                break;

            case Side.Center:
                DisplayScore(score, 0f);
                break;
        }
    }

    public void DisplayScore(int score, float offset)
    {
        if (index < scoreTextObjects.Length - 1)
            index++;
        else
            index = 0;

        scoreTextObjects[index].text = "+" + score.ToString();
        scoreTextObjects[index].transform.position = new Vector3(defaultPosition.x + offset, defaultPosition.y, defaultPosition.z);

        timer = lifeTime;
        nearMissAudio = RCC_CreateAudioSource.NewAudioSource(gameObject,
            HR_HighwayRacerProperties.Instance.nearMissAudioClip.name, 0f, 0f, 1f,
            HR_HighwayRacerProperties.Instance.nearMissAudioClip, false, true, true);
        nearMissAudio.ignoreListenerPause = true;
        nearMissAudio.ignoreListenerVolume = true;
    }

    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;

        timer = Mathf.Clamp(timer, 0f, lifeTime);

        for (int i = 0; i < scoreTextObjects.Length; i++)
        {
            scoreTextObjects[i].transform.Translate(Vector3.up * Time.deltaTime * 75f, Space.World);
            scoreTextObjects[i].color = Color.Lerp(scoreTextObjects[i].color,
                new Color(scoreTextObjects[i].color.r, scoreTextObjects[i].color.g, scoreTextObjects[i].color.b, 0f),
                Time.deltaTime * 10f);
        }

        if (timer > 0)
        {
            scoreTextObjects[index].color = new Color(scoreTextObjects[index].color.r, scoreTextObjects[index].color.g,
                scoreTextObjects[index].color.b, 1f);
        }
    }

    void OnDisable()
    {
        HR_PlayerHandler.OnNearMiss -= HR_PlayerHandler_OnNearMiss;
    }
}
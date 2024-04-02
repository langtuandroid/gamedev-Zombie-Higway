﻿using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class HR_MainMenuHandler : MonoBehaviour
{
    #region SINGLETON PATTERN

    private static HR_MainMenuHandler _instance;

    public static HR_MainMenuHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HR_MainMenuHandler>();
            }

            return _instance;
        }
    }

    #endregion

    [Header("Spawn Location Of The Cars")] public Transform carSpawnLocation;

    internal GameObject[] createdCars;
    public RCC_CarControllerV3 currentCar;

    internal int carIndex = 0;
    internal int modeIndex = 0;

    [Header("UI Menus")] public GameObject optionsMenu;
    public GameObject carSelectionMenu;
    public GameObject modsSelectionMenu;
    public GameObject sceneSelectionMenu;
    public GameObject coinsMenu;
    public GameObject creditsMenu;

    [Header("UI Loading Section")] private AsyncOperation async;
    public GameObject loadingScreen;
    public Slider loadingBar;

    internal RCC_CarControllerV3[] currentCarControllers;
    internal HR_ModApplier[] currentModAppliers;

    [Header("Other UIs")] public Text currency;
    public GameObject buyCarButton;
    public GameObject selectCarButton;
    public GameObject modCarPanel;
    public Text bestScoreOneWay;
    public Text bestScoreTwoWay;
    public Text bestScoreTimeLeft;
    public Text bestScoreBomb;

    int Levelsindex;

    private AudioSource mainMenuSoundtrack;

    void Awake()
    {
        Time.timeScale = 1f;
        AudioListener.volume = 1f;
        AudioListener.pause = false;
        Application.targetFrameRate = 60;

        if (HR_HighwayRacerProperties.Instance.mainMenuClips != null &&
            HR_HighwayRacerProperties.Instance.mainMenuClips.Length > 0)
        {
            mainMenuSoundtrack = RCC_CreateAudioSource.NewAudioSource(gameObject, "Main Menu Soundtrack", 0f, 0f, 1f,
                HR_HighwayRacerProperties.Instance.mainMenuClips[
                    UnityEngine.Random.Range(0, HR_HighwayRacerProperties.Instance.mainMenuClips.Length)], true, true,
                false);
            mainMenuSoundtrack.ignoreListenerPause = true;
            mainMenuSoundtrack.ignoreListenerVolume = true;
        }

        if (HR_HighwayRacerProperties.Instance._1MMoneyForTesting)
            PlayerPrefs.SetInt("Currency", 1000000);

        carIndex = PlayerPrefs.GetInt("SelectedPlayerCarIndex", 0);

        CreateCars();
        SpawnCar();
    }

    void Update()
    {
        currency.text = PlayerPrefs.GetInt("Currency", 0).ToString("F0");

        if (async != null && !async.isDone)
        {
            loadingBar.value = async.progress;
        }
    }

    void CreateCars()
    {
        createdCars = new GameObject[HR_PlayerCars.Instance.cars.Length];
        currentCarControllers = new RCC_CarControllerV3[createdCars.Length];
        currentModAppliers = new HR_ModApplier[createdCars.Length];

        for (int i = 0; i < createdCars.Length; i++)
        {
            createdCars[i] = (GameObject)Instantiate(HR_PlayerCars.Instance.cars[i].playerCar);
            createdCars[i].GetComponent<RCC_CarControllerV3>().canControl = false;
            createdCars[i].GetComponent<RCC_CarControllerV3>().KillEngine();
            createdCars[i].GetComponent<RCC_CarControllerV3>().lowBeamHeadLightsOn = true;
            createdCars[i].SetActive(false);
            currentCarControllers[i] = createdCars[i].GetComponent<RCC_CarControllerV3>();
            currentModAppliers[i] = createdCars[i].GetComponent<HR_ModApplier>();
        }
    }

    public void SpawnCar()
    {
        if (HR_PlayerCars.Instance.cars[carIndex].price <= 0 || HR_PlayerCars.Instance.cars[carIndex].unlocked)
            PlayerPrefs.SetInt(createdCars[carIndex].name + "Owned", 1);

        if (PlayerPrefs.GetInt(createdCars[carIndex].name + "Owned") == 1)
        {
            if (buyCarButton.transform.GetChild(0).GetChild(0).gameObject.GetComponentInChildren<Text>())
                buyCarButton.transform.GetChild(0).GetChild(0).gameObject.GetComponentInChildren<Text>().text = "";
            buyCarButton.SetActive(false);
            selectCarButton.SetActive(true);
            modCarPanel.SetActive(true);
        }
        else
        {
            selectCarButton.SetActive(false);
            buyCarButton.SetActive(true);
            modCarPanel.SetActive(false);
            if (buyCarButton.transform.GetChild(0).GetChild(0).gameObject.GetComponentInChildren<Text>())
                buyCarButton.transform.GetChild(0).GetChild(0).gameObject.GetComponentInChildren<Text>().text =
                    HR_PlayerCars.Instance.cars[carIndex].price.ToString("F0");
        }

        for (int i = 0; i < createdCars.Length; i++)
        {
            createdCars[i].SetActive(false);
            createdCars[i].transform.position = carSpawnLocation.position;
            createdCars[i].transform.rotation = carSpawnLocation.rotation;
        }

        createdCars[carIndex].SetActive(true);
        currentCar = createdCars[carIndex].GetComponent<RCC_CarControllerV3>();
    }

    public void BuyCar()
    {
        if (PlayerPrefs.GetInt("Currency") >= HR_PlayerCars.Instance.cars[carIndex].price)
            PlayerPrefs.SetInt("Currency",
                PlayerPrefs.GetInt("Currency") - HR_PlayerCars.Instance.cars[carIndex].price);
        else
            return;

        PlayerPrefs.SetInt(createdCars[carIndex].name + "Owned", 1);

        SpawnCar();
    }

    public void SelectCar()
    {
        PlayerPrefs.SetInt("SelectedPlayerCarIndex", carIndex);
    }

    public void PositiveCarIndex()
    {
        carIndex++;

        if (carIndex >= createdCars.Length)
            carIndex = 0;

        SpawnCar();
    }

    public void NegativeCarIndex()
    {
        carIndex--;

        if (carIndex < 0)
            carIndex = createdCars.Length - 1;

        SpawnCar();
    }

    public void EnableMenu(GameObject activeMenu)
    {
        if (activeMenu.activeSelf)
        {
            activeMenu.SetActive(false);
            return;
        }

        optionsMenu.SetActive(false);
        carSelectionMenu.SetActive(false);
        modsSelectionMenu.SetActive(false);
        sceneSelectionMenu.SetActive(false);
        coinsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        loadingScreen.SetActive(false);

        activeMenu.SetActive(true);

        if (activeMenu == modsSelectionMenu)
            BestScores();
    }

    public void SelectScene(int levelIndex)
    {
        Levelsindex = levelIndex;
        SelectCar();
        EnableMenu(loadingScreen);
        Invoke("delaymenu", 3f);
    }

    void delaymenu()
    {
        async = SceneManager.LoadSceneAsync(Levelsindex);
    }

    public void SelectMode(int _modeIndex)
    {
        PlayerPrefs.SetInt("SelectedModeIndex", _modeIndex);
        EnableMenu(sceneSelectionMenu);
    }

    public void BestScores()
    {
        bestScoreOneWay.text = "BEST SCORE\n" + PlayerPrefs.GetInt("bestScoreOneWay", 0);
        bestScoreTwoWay.text = "BEST SCORE\n" + PlayerPrefs.GetInt("bestScoreTwoWay", 0);
        bestScoreTimeLeft.text = "BEST SCORE\n" + PlayerPrefs.GetInt("bestScoreTimeAttack", 0);
        bestScoreBomb.text = "BEST SCORE\n" + PlayerPrefs.GetInt("bestScoreBomb", 0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Rateus()
    {
        Application.OpenURL(
            "https://docs.google.com/document/d/1mq1WkVtah8aFvAs_4rS8g6oRK1dQUZy9/edit#heading=h.gjdgxs");
    }

    public void More()
    {
        Application.OpenURL(
            "https://docs.google.com/document/d/1aUeEZSUOtIgozx04TlYuodIcsflPvVbH/edit#heading=h.gjdgxs");
    }
}
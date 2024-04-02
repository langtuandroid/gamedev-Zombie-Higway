﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RCC_CarControllerV3))]
public class HR_PlayerHandler : MonoBehaviour
{
    private RCC_CarControllerV3 carController;
    private PathwayPooling roadPooling;
    private Rigidbody rigid;

    private bool gameOver = false;

    private bool gameStarted
    {
        get { return GameHandler.Instance.gameStarted; }
    }

    internal float score;
    internal float timeLeft = 100f;
    internal int combo;
    internal int maxCombo;

    internal float speed = 0f;
    internal float distance = 0f;
    internal float highSpeedCurrent = 0f;
    internal float highSpeedTotal = 0f;
    internal float opposideDirectionCurrent = 0f;
    internal float opposideDirectionTotal = 0f;

    private int minimumSpeedForGainScore
    {
        get { return HR_HighwayRacerProperties.Instance._minimumSpeedForGainScore; }
    }

    private int minimumSpeedForHighSpeed
    {
        get { return HR_HighwayRacerProperties.Instance._minimumSpeedForHighSpeed; }
    }

    private Vector3 previousPosition;

    private string currentTrafficCarNameLeft;
    private string currentTrafficCarNameRight;

    internal int nearMisses;
    private float comboTime;

    internal bool bombTriggered = false;
    internal float bombHealth = 100f;

    public delegate void onPlayerSpawned(HR_PlayerHandler player);

    public static event onPlayerSpawned OnPlayerSpawned;

    public delegate void onNearMiss(HR_PlayerHandler player, int score, DynamicScoreDisplay.Side side);

    public static event onNearMiss OnNearMiss;

    public delegate void onPlayerDied(HR_PlayerHandler player);

    public static event onPlayerDied OnPlayerDied;

    public static HR_PlayerHandler _inst;

    void Awake()
    {
        _inst = this;
        if (!GameObject.FindObjectOfType<RCC_UIDashboardDisplay>())
        {
            enabled = false;
            return;
        }

        carController = GetComponentInParent<RCC_CarControllerV3>();
        roadPooling = GameObject.FindObjectOfType<PathwayPooling>();
        rigid = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        OnPlayerSpawned(this);

        if (!carController.engineRunning)
            carController.StartEngine();
    }

    void Update()
    {
        if (gameOver || !gameStarted)
            return;

        speed = carController.speed;

        distance += Vector3.Distance(previousPosition, transform.position) / 1000f;
        previousPosition = transform.position;

        if (speed >= minimumSpeedForGainScore)
        {
            score += carController.speed * (Time.deltaTime * .05f);
        }

        if (speed >= minimumSpeedForHighSpeed)
        {
            highSpeedCurrent += Time.deltaTime;
            highSpeedTotal += Time.deltaTime;
        }
        else
        {
            highSpeedCurrent = 0f;
        }

        if (speed >= (minimumSpeedForHighSpeed / 2f) && transform.position.x <= 0f &&
            GameHandler.Instance.mode == GameHandler.Mode.TwoWay)
        {
            opposideDirectionCurrent += Time.deltaTime;
            opposideDirectionTotal += Time.deltaTime;
        }
        else
        {
            opposideDirectionCurrent = 0f;
        }

        if (GameHandler.Instance.mode == GameHandler.Mode.TimeAttack)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                timeLeft = 0;
                OnGameOver(0f);
            }
        }

        comboTime += Time.deltaTime;

        if (GameHandler.Instance.mode == GameHandler.Mode.Bomb)
        {
            if (speed > 80f)
            {
                if (!bombTriggered)
                    bombTriggered = true;
                else
                    bombHealth += Time.deltaTime * 5f;
            }
            else if (bombTriggered)
            {
                bombHealth -= Time.deltaTime * 10f;
            }

            bombHealth = Mathf.Clamp(bombHealth, 0f, 100f);

            if (bombHealth <= 0f)
            {
                GameObject explosion = (GameObject)Instantiate(HR_HighwayRacerProperties.Instance.explosionEffect,
                    transform.position, transform.rotation);
                explosion.transform.SetParent(null);
                GetComponent<Rigidbody>().isKinematic = true;
                OnGameOver(2f);
            }
        }

        if (comboTime >= 2)
            combo = 0;

        CheckStatus();
    }

    void FixedUpdate()
    {
        if (!gameOver && gameStarted)
        {
            CheckNearMiss();
        }
    }

    public void SirenHeadHit()
    {
        score += 200;
        OnNearMiss(this, 200, DynamicScoreDisplay.Side.Left);
    }

    void CheckNearMiss()
    {
        RaycastHit hit;

        Debug.DrawRay(carController.COM.position, (-transform.right * 2f), Color.white);
        Debug.DrawRay(carController.COM.position, (transform.right * 2f), Color.white);
        Debug.DrawRay(carController.COM.position, (transform.forward * 20f), Color.white);

        if (Physics.Raycast(carController.COM.position, (-transform.right), out hit, 2f,
                HR_HighwayRacerProperties.Instance.trafficCarsLayer) && !hit.collider.isTrigger)
        {
            currentTrafficCarNameLeft = hit.transform.name;
        }
        else
        {
            if (currentTrafficCarNameLeft != null &&
                speed > HR_HighwayRacerProperties.Instance._minimumSpeedForGainScore)
            {
                nearMisses++;
                combo++;
                comboTime = 0;
                if (maxCombo <= combo)
                    maxCombo = combo;

                score += 100f * Mathf.Clamp(combo / 1.5f, 1f, 20f);
                OnNearMiss(this, (int)(100f * Mathf.Clamp(combo / 1.5f, 1f, 20f)), DynamicScoreDisplay.Side.Left);

                currentTrafficCarNameLeft = null;
            }
            else
            {
                currentTrafficCarNameLeft = null;
            }
        }

        if (Physics.Raycast(carController.COM.position, (transform.right), out hit, 2f,
                HR_HighwayRacerProperties.Instance.trafficCarsLayer) && !hit.collider.isTrigger)
        {
            currentTrafficCarNameRight = hit.transform.name;
        }
        else
        {
            if (currentTrafficCarNameRight != null &&
                speed > HR_HighwayRacerProperties.Instance._minimumSpeedForGainScore)
            {
                nearMisses++;
                combo++;
                comboTime = 0;
                if (maxCombo <= combo)
                    maxCombo = combo;

                score += 100f * Mathf.Clamp(combo / 1.5f, 1f, 20f);
                OnNearMiss(this, (int)(100f * Mathf.Clamp(combo / 1.5f, 1f, 20f)), DynamicScoreDisplay.Side.Right);

                currentTrafficCarNameRight = null;
            }
            else
            {
                currentTrafficCarNameRight = null;
            }
        }

        if (Physics.Raycast(carController.COM.position, (transform.forward), out hit, 40f,
                HR_HighwayRacerProperties.Instance.trafficCarsLayer) && !hit.collider.isTrigger)
        {
            if (carController.highBeamHeadLightsOn)
                hit.transform.SendMessage("ChangeLines");
        }
    }

    private int collisionCount = 0;


    void OnCollisionEnter(Collision col)
    {
        combo = 0;
        if (col.gameObject.CompareTag("Enemy"))
        {
            BloodOverlay bloodOverlayManager = FindObjectOfType<BloodOverlay>();
            if (bloodOverlayManager != null)
            {
                bloodOverlayManager.ShowBloodOverlay();
            }
        }

        if (col.relativeVelocity.magnitude < HR_HighwayRacerProperties.Instance._minimumCollisionForGameOver ||
            (1 << col.gameObject.layer) != HR_HighwayRacerProperties.Instance.trafficCarsLayer.value)
            return;

        if (GameHandler.Instance.mode == GameHandler.Mode.Bomb)
        {
            bombHealth -= col.relativeVelocity.magnitude / 2f;
            return;
        }

        collisionCount++;

        if (collisionCount >= 2)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            OnGameOver(1f);
        }
    }

    void CheckStatus()
    {
        if (!roadPooling || rigid.isKinematic)
            return;

        if (GameHandler.Instance && !gameStarted)
            return;

        if (speed < 5f || Mathf.Abs(transform.position.x) > 10f || Mathf.Abs(transform.position.y) > 10f)
        {
            transform.position = new Vector3(0f, 2f, transform.position.z + 10f);
            transform.rotation = Quaternion.identity;
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = new Vector3(0f, 0f, 20 / 2f);
        }
    }

    void OnGameOver(float delayTime)
    {
        OnPlayerDied(this);

        gameOver = true;
        carController.canControl = false;
        carController.engineRunning = false;
    }

}
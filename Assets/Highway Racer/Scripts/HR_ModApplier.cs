using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(RCC_CarControllerV3))]
public class HR_ModApplier : MonoBehaviour
{
    private RCC_CarControllerV3 carController;

    internal Color bodyColor;
    public MeshRenderer bodyRenderer;
    public int materialIndex;

    internal GameObject selectedWheel;
    internal int wheelIndex;

    internal bool isSirenPurchased = false;
    internal bool isSirenAttached = false;

    internal bool isNOSPurchased = false;
    internal bool isTurboPurchased = false;

    internal GameObject attachedFrontSiren;
    internal GameObject attachedRearSiren;

    private int _speedLevel = 0;

    public int speedLevel
    {
        get { return _speedLevel; }
        set
        {
            if (value <= 5)
                _speedLevel = value;
        }
    }

    private int _handlingLevel = 0;

    public int handlingLevel
    {
        get { return _handlingLevel; }
        set
        {
            if (value <= 5)
                _handlingLevel = value;
        }
    }

    private int _brakeLevel = 0;

    public int brakeLevel
    {
        get { return _brakeLevel; }
        set
        {
            if (value <= 5)
                _brakeLevel = value;
        }
    }

    private float defMaxSpeed;
    private float defHandling;
    private float defMaxBrake;

    public float maxUpgradeSpeed = 220f;
    public float maxUpgradeHandling = 4;
    public float maxUpgradeBrake = 4;

    void Awake()
    {
        carController = GetComponent<RCC_CarControllerV3>();

        defMaxSpeed = carController.maxspeed;
        defHandling = carController.highwaySteeringHelper;
        defMaxBrake = carController.highwayBrakingHelper;

        if (PlayerPrefs.HasKey(transform.name + "SelectedWheel"))
        {
            wheelIndex = PlayerPrefs.GetInt(transform.name + "SelectedWheel", 0);
            selectedWheel = WheelCollection.Instance.wheels[wheelIndex].wheel;
        }
        else
        {
            selectedWheel = null;
        }

        _speedLevel = PlayerPrefs.GetInt(transform.name + "SpeedLevel");
        _handlingLevel = PlayerPrefs.GetInt(transform.name + "HandlingLevel");
        _brakeLevel = PlayerPrefs.GetInt(transform.name + "BrakeLevel");

        bodyColor = PlayerPrefsX.GetColor(transform.name + "BodyColor",
            HR_HighwayRacerProperties.Instance._defaultBodyColor);
        isSirenPurchased = PlayerPrefsX.GetBool(transform.name + "Siren", false);
        isSirenAttached = PlayerPrefsX.GetBool(transform.name + "SirenAttached", false);

        isNOSPurchased = PlayerPrefsX.GetBool(transform.name + "NOS", false);
        isTurboPurchased = PlayerPrefsX.GetBool(transform.name + "Turbo", false);
    }

    void OnEnable()
    {
        ModificationUpgradeController[] mods = GameObject.FindObjectsOfType<ModificationUpgradeController>();

        for (int i = 0; i < mods.Length; i++)
        {
            mods[i].applier = GetComponent<HR_ModApplier>();
        }

        UpdateStats();
        CheckGroundGap();
    }

    public void UpdateStats()
    {
        carController.maxspeed = Mathf.Lerp(defMaxSpeed, maxUpgradeSpeed, _speedLevel / 5f);
        carController.highwaySteeringHelper = Mathf.Lerp(defHandling, maxUpgradeHandling, _handlingLevel / 5f);
        carController.highwayBrakingHelper = Mathf.Lerp(defMaxBrake, maxUpgradeBrake, _brakeLevel / 5f);

        if (bodyRenderer)
            bodyRenderer.sharedMaterials[materialIndex].color = bodyColor;


        if (isSirenPurchased && !attachedFrontSiren)
        {
            CreateSiren();
        }

        if (isNOSPurchased)
            carController.useNOS = true;
        else
            carController.useNOS = false;

        if (isTurboPurchased)
            carController.useTurbo = true;
        else
            carController.useTurbo = false;

        if (selectedWheel)
        {
            ChangeWheels(carController, selectedWheel);

            carController.FrontLeftWheelCollider.wheelCollider.radius =
                RCC_GetBounds.MaxBoundsExtent(selectedWheel.transform);
            carController.FrontRightWheelCollider.wheelCollider.radius =
                RCC_GetBounds.MaxBoundsExtent(selectedWheel.transform);
            carController.RearLeftWheelCollider.wheelCollider.radius =
                RCC_GetBounds.MaxBoundsExtent(selectedWheel.transform);
            carController.RearRightWheelCollider.wheelCollider.radius =
                RCC_GetBounds.MaxBoundsExtent(selectedWheel.transform);

            PlayerPrefs.SetInt(transform.name + "SelectedWheel", wheelIndex);
        }

        PlayerPrefs.SetInt(transform.name + "SpeedLevel", _speedLevel);
        PlayerPrefs.SetInt(transform.name + "HandlingLevel", _handlingLevel);
        PlayerPrefs.SetInt(transform.name + "BrakeLevel", _brakeLevel);
        PlayerPrefsX.SetColor(transform.name + "BodyColor", bodyColor);
        PlayerPrefsX.SetBool(transform.name + "Siren", isSirenPurchased);
        PlayerPrefsX.SetBool(transform.name + "NOS", isNOSPurchased);
        PlayerPrefsX.SetBool(transform.name + "Turbo", isTurboPurchased);
    }

    void Update()
    {
        if (maxUpgradeSpeed < carController.maxspeed)
            maxUpgradeSpeed = carController.maxspeed;

        if (maxUpgradeHandling < carController.highwaySteeringHelper)
            maxUpgradeHandling = carController.highwaySteeringHelper;

        if (maxUpgradeBrake < carController.highwayBrakingHelper)
            maxUpgradeBrake = carController.highwayBrakingHelper;
    }

    void CreateSiren()
    {
        if (GetComponent<RCC_CarControllerV3>().headLights.Length < 2 ||
            GetComponent<RCC_CarControllerV3>().brakeLights.Length < 2)
        {
            Debug.LogError(
                "Vehicle is missing headlights or brakelights! In order to use sirens, you need to create at least two headlights and two brakelights for your vehicles.");
            return;
        }

        attachedFrontSiren = new GameObject("Front Siren Lights");
        attachedRearSiren = new GameObject("Rear Siren Lights");

        attachedFrontSiren.transform.SetParent(GetComponent<RCC_CarControllerV3>().chassis.transform, false);
        attachedRearSiren.transform.SetParent(GetComponent<RCC_CarControllerV3>().chassis.transform, false);

        GameObject frontSiren = (GameObject)Instantiate(HR_HighwayRacerProperties.Instance.attachableSiren,
            GetComponent<RCC_CarControllerV3>().transform.position, transform.rotation);
        GameObject rearSiren = (GameObject)Instantiate(HR_HighwayRacerProperties.Instance.attachableSiren,
            GetComponent<RCC_CarControllerV3>().transform.position,
            transform.rotation * Quaternion.Euler(0f, 180f, 0f));

        MeshFilter[] frontSirenLights = frontSiren.GetComponentsInChildren<MeshFilter>();
        MeshFilter[] rearSirenLights = rearSiren.GetComponentsInChildren<MeshFilter>();

        for (int i = 0; i < 2; i++)
        {
            frontSiren.transform.SetParent(attachedFrontSiren.transform, true);

            if (frontSirenLights[i].transform.localPosition.x >= 0f)
            {
                frontSirenLights[i].transform.position =
                    GetComponent<RCC_CarControllerV3>().headLights[i].transform.position;
                frontSirenLights[i].transform.localPosition = new Vector3(.25f,
                    frontSirenLights[i].transform.localPosition.y, frontSirenLights[i].transform.localPosition.z);
            }
            else
            {
                frontSirenLights[i].transform.position =
                    GetComponent<RCC_CarControllerV3>().headLights[i].transform.position;
                frontSirenLights[i].transform.localPosition = new Vector3(-.25f,
                    frontSirenLights[i].transform.localPosition.y, frontSirenLights[i].transform.localPosition.z);
            }
        }

        for (int i = 0; i < 2; i++)
        {
            rearSiren.transform.SetParent(attachedRearSiren.transform, true);

            if (rearSirenLights[i].transform.localPosition.x >= 0f)
            {
                rearSirenLights[i].transform.position =
                    GetComponent<RCC_CarControllerV3>().brakeLights[i].transform.position;
                rearSirenLights[i].transform.localPosition = new Vector3(.25f,
                    rearSirenLights[i].transform.localPosition.y, rearSirenLights[i].transform.localPosition.z);
            }
            else
            {
                rearSirenLights[i].transform.position =
                    GetComponent<RCC_CarControllerV3>().brakeLights[i].transform.position;
                rearSirenLights[i].transform.localPosition = new Vector3(-.25f,
                    rearSirenLights[i].transform.localPosition.y, rearSirenLights[i].transform.localPosition.z);
            }
        }

        if (!isSirenAttached)
        {
            attachedFrontSiren.SetActive(false);
            attachedRearSiren.SetActive(false);
        }
    }

    public void ChangeWheels(RCC_CarControllerV3 car, GameObject wheel)
    {
        RCC_Customization.ChangeWheels(car, selectedWheel);
    }

    public void ToggleSiren()
    {
        if (isSirenPurchased && attachedFrontSiren)
        {
            isSirenAttached = !isSirenAttached;

            if (isSirenAttached)
            {
                attachedFrontSiren.SetActive(true);
                attachedRearSiren.SetActive(true);
                PlayerPrefsX.SetBool(transform.name + "SirenAttached", true);
            }
            else
            {
                attachedFrontSiren.SetActive(false);
                attachedRearSiren.SetActive(false);
                PlayerPrefsX.SetBool(transform.name + "SirenAttached", false);
            }
        }
    }

    void CheckGroundGap()
    {
        WheelCollider wheel = GetComponentInChildren<WheelCollider>();
        float distancePivotBetweenWheel = Vector3.Distance(new Vector3(0f, transform.position.y, 0f),
            new Vector3(0f, wheel.transform.position.y, 0f));

        RaycastHit hit;

        if (Physics.Raycast(wheel.transform.position, -Vector3.up, out hit, 10f))
        {
            transform.position = new Vector3(transform.position.x,
                hit.point.y + distancePivotBetweenWheel + (wheel.radius / 1f) + (wheel.suspensionDistance / 2f),
                transform.position.z);
        }

        carController.sleepingRigid = false;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Serialization;

public class ControllerTypeSelector : MonoBehaviour
{
    [FormerlySerializedAs("_controllerType")] public controllerType controllerTypeOption;

    public enum controllerType
    {
        keypad,
        accelerometer
    }

    private Button sprite;
    private Color defCol;

    void Start()
    {
        sprite = GetComponent<Button>();
        defCol = sprite.image.color;

        if (!PlayerPrefs.HasKey("ControllerType"))
            PlayerPrefs.SetInt("ControllerType", 0);

        CheckControllerType();
    }


    public void OnClick()
    {
        if (controllerTypeOption == controllerType.keypad)
        {
            PlayerPrefs.SetInt("ControllerType", 0);
        }

        if (controllerTypeOption == controllerType.accelerometer)
        {
            PlayerPrefs.SetInt("ControllerType", 1);
        }

        ControllerTypeSelector[] ct = GameObject.FindObjectsOfType<ControllerTypeSelector>();

        foreach (ControllerTypeSelector cts in ct)
        {
            cts.CheckControllerType();
        }
    }

    void CheckControllerType()
    {
        if (PlayerPrefs.GetInt("ControllerType") == 0)
        {
            if (controllerTypeOption == controllerType.keypad)
            {
                sprite.image.color = new Color(.667f, 1f, 0f);
            }

            if (controllerTypeOption == controllerType.accelerometer)
            {
                sprite.image.color = defCol;
            }
        }

        if (PlayerPrefs.GetInt("ControllerType") == 1)
        {
            if (controllerTypeOption == controllerType.keypad)
            {
                sprite.image.color = defCol;
            }

            if (controllerTypeOption == controllerType.accelerometer)
            {
                sprite.image.color = new Color(.667f, 1f, 0f);
            }
        }

        if (GameObject.FindObjectOfType<RCC_CarControllerV3>())
            GameObject.FindObjectOfType<RCC_CarControllerV3>().GetControllerType();
    }
}
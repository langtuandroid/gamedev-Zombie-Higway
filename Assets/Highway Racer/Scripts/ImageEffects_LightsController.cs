using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageEffects_LightsController : MonoBehaviour
{
    private GameImageEffects imageEffects;
    private Button sprite;
    private Color defaultColor;

    void Start()
    {
        imageEffects = GameObject.FindObjectOfType<GameImageEffects>();
        sprite = GetComponent<Button>();
        defaultColor = sprite.image.color;

        if (!PlayerPrefs.HasKey("HQLights"))
            PlayerPrefs.SetInt("HQLights", 0);

        CheckLightsState();
    }


    public void OnClick()
    {
        if (PlayerPrefs.GetInt("HQLights") == 0)
            PlayerPrefs.SetInt("HQLights", 1);
        else if (PlayerPrefs.GetInt("HQLights") == 1)
            PlayerPrefs.SetInt("HQLights", 0);

        CheckLightsState();
    }

    void CheckLightsState()
    {
        if (PlayerPrefs.GetInt("HQLights") == 1)
        {
            sprite.image.color = new Color(.667f, 1f, 0f);
        }

        if (PlayerPrefs.GetInt("HQLights") == 0)
        {
            sprite.image.color = defaultColor;
        }

        imageEffects.Check();
    }
}
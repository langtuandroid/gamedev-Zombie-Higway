using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageEffects_LensFlareController : MonoBehaviour
{
    private GameImageEffects imageEffects;
    private Button sprite;
    private Color defCol;

    void Start()
    {
        imageEffects = GameObject.FindObjectOfType<GameImageEffects>();
        sprite = GetComponent<Button>();
        defCol = sprite.image.color;

        if (!PlayerPrefs.HasKey("LensFlare"))
            PlayerPrefs.SetInt("LensFlare", 1);

        CheckLensFlareState();
    }


    public void OnClick()
    {
        if (PlayerPrefs.GetInt("LensFlare") == 0)
            PlayerPrefs.SetInt("LensFlare", 1);
        else if (PlayerPrefs.GetInt("LensFlare") == 1)
            PlayerPrefs.SetInt("LensFlare", 0);

        CheckLensFlareState();
    }

    void CheckLensFlareState()
    {
        if (PlayerPrefs.GetInt("LensFlare") == 1)
        {
            sprite.image.color = new Color(.667f, 1f, 0f);
        }

        if (PlayerPrefs.GetInt("LensFlare") == 0)
        {
            sprite.image.color = defCol;
        }

        imageEffects.Check();
    }
}
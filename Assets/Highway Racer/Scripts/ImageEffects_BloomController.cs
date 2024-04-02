using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageEffects_BloomController : MonoBehaviour
{
    private GameImageEffects imageEffects;
    private Button button;
    private Color defaultColor;

    void Start()
    {
        imageEffects = GameObject.FindObjectOfType<GameImageEffects>();
        button = GetComponent<Button>();
        defaultColor = button.image.color;

        if (!PlayerPrefs.HasKey("Bloom"))
            PlayerPrefs.SetInt("Bloom", 0);

        CheckBloomState();
    }


    public void OnClick()
    {
        if (PlayerPrefs.GetInt("Bloom") == 0)
            PlayerPrefs.SetInt("Bloom", 1);
        else if (PlayerPrefs.GetInt("Bloom") == 1)
            PlayerPrefs.SetInt("Bloom", 0);

        CheckBloomState();
    }

    void CheckBloomState()
    {
        if (PlayerPrefs.GetInt("Bloom") == 1)
        {
            button.image.color = new Color(.667f, 1f, 0f);
        }

        if (PlayerPrefs.GetInt("Bloom") == 0)
        {
            button.image.color = defaultColor;
        }

        imageEffects.Check();
    }
}
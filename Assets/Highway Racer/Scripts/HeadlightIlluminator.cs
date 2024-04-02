using UnityEngine;
using System.Collections;

public class HeadlightIlluminator : MonoBehaviour
{
    private GameHandler gameMaster;
    private Projector projector;
    private Light headlight;

    void Start()
    {
        gameMaster = GameHandler.Instance;
        projector = GetComponent<Projector>();
        headlight = GetComponentInParent<Light>();

        Material newMaterial = new Material(projector.material);
        projector.material = newMaterial;
    }

    void Update()
    {
        if (!headlight.enabled)
        {
            projector.enabled = false;
            return;
        }
        else
        {
            projector.enabled = true;
        }

        if (gameMaster && gameMaster.dayOrNight == GameHandler.DayOrNight.Day)
            projector.material.color = headlight.color * headlight.intensity * .05f;
        else
            projector.material.color = headlight.color * headlight.intensity * .25f;

        projector.farClipPlane = Mathf.Lerp(10f, 40f, (headlight.range - 50) / 150);
        projector.fieldOfView = Mathf.Lerp(40f, 30f, (headlight.range - 50) / 150);
    }
}
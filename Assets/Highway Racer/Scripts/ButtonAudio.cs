using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour, IPointerClickHandler
{
    private AudioSource buttonSound;

    public void OnPointerClick(PointerEventData data)
    {
        if (Camera.main != null)
        {
            buttonSound = RCC_CreateAudioSource.NewAudioSource(Camera.main.gameObject,
                HR_HighwayRacerProperties.Instance.buttonClickAudioClip.name, 0f, 0f, 1f,
                HR_HighwayRacerProperties.Instance.buttonClickAudioClip, false, true, true);
            buttonSound.ignoreListenerPause = true;
            buttonSound.ignoreListenerVolume = true;
        }
    }
}
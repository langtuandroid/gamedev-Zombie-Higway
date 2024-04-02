using UnityEngine;

public class MuteButton : MonoBehaviour
{
    private SoundManager soundManager;

    private void Start()
    {
        soundManager = SoundManager.Instance;
        if (soundManager == null)
        {
            Debug.LogError("SoundManager не найден в сцене.");
        }
        else
        {
            soundManager.muteSoundEvent.AddListener(OnMuteSound);
        }
    }

    public void MuteSounds()
    {
        if (soundManager != null)
        {
            soundManager.MuteSounds(!soundManager.IsMuted());
        }
        else
        {
            Debug.LogError("SoundManager не найден в сцене.");
        }
    }

    private void OnMuteSound(bool mute)
    {
        AudioListener.pause = mute;
    }
}
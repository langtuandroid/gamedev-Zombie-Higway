using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

    public UnityEvent<bool> muteSoundEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MuteSounds(bool mute)
    {
        muteSoundEvent.Invoke(mute);
    }

    public bool IsMuted()
    {
        return AudioListener.pause;
    }
}
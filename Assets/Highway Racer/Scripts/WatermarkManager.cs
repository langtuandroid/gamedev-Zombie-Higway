using UnityEngine;
using System.Collections;

public class WatermarkManager : MonoBehaviour
{
    static WatermarkManager instance;

    void Start()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
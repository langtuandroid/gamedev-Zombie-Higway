using UnityEngine;
using System.Collections;

[System.Serializable]
public class WheelCollection : ScriptableObject
{
    public static WheelCollection instance;

    public static WheelCollection Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load("HR_Assets/WheelCollection") as WheelCollection;
            return instance;
        }
    }

    [System.Serializable]
    public class Wheels
    {
        public GameObject wheel;
        public bool unlocked;
        public int price;
    }

    public Wheels[] wheels;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootScript : MonoBehaviour
{
    public bool Shoot;
    public static ShootScript _inst;
    // Start is called before the first frame update
    void Start()
    {
        _inst=this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressFire(){
        Shoot=true;
    }

    public void OnPressExitFire(){
        Shoot=false;
    }
}

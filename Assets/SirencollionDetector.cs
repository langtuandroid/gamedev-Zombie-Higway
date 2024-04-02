using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SirencollionDetector : MonoBehaviour
{
    
    public GameObject SirenRagDoll;
    GameObject Ragdoll;
    // Start is called before the first frame update
    void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.tag=="Player" || collisionInfo.gameObject.tag=="Bullet"){
            HR_PlayerHandler._inst.SirenHeadHit();
            this.gameObject.SetActive(false);
            // Ragdoll=Instantiate(SirenRagDoll, this.gameObject.transform.position, Quaternion.identity);
            // Invoke("DieDelay",1f);
        }
    }

    void DieDelay(){
        // Destroy(Ragdoll);
    }
}

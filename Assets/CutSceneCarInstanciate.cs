using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCarInstanciate : MonoBehaviour
{
    public GameObject InstanciatePoint;
    public GameObject[] Cars;
    int selectedCarIndex=0;
    // Start is called before the first frame update
    void Start()
    {
        selectedCarIndex = PlayerPrefs.GetInt("SelectedPlayerCarIndex");
        GameObject Car=GameObject.Instantiate(Cars[selectedCarIndex],InstanciatePoint.transform.position,InstanciatePoint.transform.rotation);
        Invoke("ChangeScene",12.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeScene(){
        
        Application.LoadLevel(2);			
		//AdManager._Instance.ShowInterstitial();
    }

}

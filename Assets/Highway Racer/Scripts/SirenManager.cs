using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SirenManager : MonoBehaviour
{
    #region SINGLETON PATTERN

    public static SirenManager instance;

    public static SirenManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<SirenManager>();
            return instance;
        }
    }

    #endregion

    internal Transform reference;
    public Transform[] lines;

    private bool isTrafficActive
    {
        get { return GameHandler.Instance.gameStarted; }
    }

    public TrafficCars[] trafficCars;

    [System.Serializable]
    public class TrafficCars
    {
        public GameObject trafficCar;
        public int frequence = 1;
    }

    private List<HR_SireInfo> _trafficCars = new List<HR_SireInfo>();

    void Start()
    {
        reference = Camera.main.transform;
        CreateTraffic();
    }

    void Update()
    {
        if (isTrafficActive)
            AnimateTraffic();
    }

    void CreateTraffic()
    {
        for (int i = 0; i < trafficCars.Length; i++)
        {
            for (int k = 0; k < trafficCars[i].frequence; k++)
            {
                GameObject go = (GameObject)GameObject.Instantiate(trafficCars[i].trafficCar,
                    trafficCars[i].trafficCar.transform.position, trafficCars[i].trafficCar.transform.rotation);
                _trafficCars.Add(go.GetComponent<HR_SireInfo>());
                go.SetActive(false);
            }
        }
    }

    void AnimateTraffic()
    {
        for (int i = 0; i < _trafficCars.Count; i++)
        {
            if (_trafficCars[i] != null)
            {
                if (reference.transform.position.z > (_trafficCars[i].transform.position.z + 15) ||
                    reference.transform.position.z < (_trafficCars[i].transform.position.z - (325)))
                {
                    ReAlignTraffic(_trafficCars[i]);
                }
            }
            else
            {
                int value = Random.Range(0, trafficCars.Length);
                GameObject go = (GameObject)GameObject.Instantiate(trafficCars[value].trafficCar,
                    trafficCars[value].trafficCar.transform.position, trafficCars[value].trafficCar.transform.rotation);
                _trafficCars[i] = go.GetComponent<HR_SireInfo>();

                ReAlignTraffic(_trafficCars[i]);
            }
        }
    }

    void ReAlignTraffic(HR_SireInfo realignableObject)
    {
        if (!realignableObject.gameObject.activeSelf)
            realignableObject.gameObject.SetActive(true);

        int randomLine = Random.Range(0, lines.Length);

        realignableObject.currentLine = randomLine;
        realignableObject.transform.position = new Vector3(lines[randomLine].position.x,
            realignableObject.transform.position.y, (reference.transform.position.z + (Random.Range(100, 300))));

        switch (GameHandler.Instance.mode)
        {
            case (GameHandler.Mode.OneWay):
                realignableObject.transform.rotation = Quaternion.identity;
                break;
            case (GameHandler.Mode.TwoWay):
                if (realignableObject.transform.position.x <= 0f)
                    realignableObject.transform.rotation = Quaternion.identity * Quaternion.Euler(0f, 180f, 0f);
                else
                    realignableObject.transform.rotation = Quaternion.identity;
                break;
            case (GameHandler.Mode.TimeAttack):
                realignableObject.transform.rotation = Quaternion.identity;
                break;
            case (GameHandler.Mode.Bomb):
                realignableObject.transform.rotation = Quaternion.identity;
                break;
        }

        realignableObject.SendMessage("OnReAligned");
    }

    bool CheckIfClipping(BoxCollider trafficCarBound)
    {
        for (int i = 0; i < _trafficCars.Count; i++)
        {
            if (_trafficCars[i] != null)
            {
                if (!trafficCarBound.transform.IsChildOf(_trafficCars[i].transform) &&
                    _trafficCars[i].gameObject.activeSelf)
                {
                    if (BoundingBoxExtensions.ContainsBoundingBox(trafficCarBound.transform, trafficCarBound.bounds,
                            _trafficCars[i].triggerCollider.bounds))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
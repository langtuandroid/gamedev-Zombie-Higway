using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathwayPooling : MonoBehaviour
{
    internal Transform reference;
    private bool keepAnimating = true;

    [System.Serializable]
    public class RoadObjects
    {
        public GameObject roadObject;
    }

    public int roadAmountInPool = 10;
    private float[] pathwayLengths;

    [Header("Use This Layer On Road For Calculating Road Length")]
    public LayerMask asphaltLayer;

    [Header("Pooling Road Objects. Select Them While They Are On Your Scene")]
    public RoadObjects[] roadObjects;

    internal List<GameObject> roads = new List<GameObject>();

    public float roadWidth = 13.5f;
    private int index = 0;

    void Awake()
    {
        reference = Camera.main.transform;
        pathwayLengths = new float[roadObjects.Length];

        for (int i = 0; i < roadObjects.Length; i++)
        {
            pathwayLengths[i] = GetRoadLength(i);
        }

        ConstructPathways();
    }

    protected float GetRoadLength(int roadIndex)
    {
        GameObject roadReference =
            (GameObject)GameObject.Instantiate(roadObjects[roadIndex].roadObject, Vector3.zero, Quaternion.identity);

        Bounds combinedBounds = roadReference.GetComponentInChildren<Renderer>().bounds;
        Renderer[] renderers = roadReference.GetComponentsInChildren<Renderer>();

        foreach (Renderer render in renderers)
        {
            if (render != roadReference.GetComponent<Renderer>() && 1 << render.gameObject.layer == asphaltLayer)
                combinedBounds.Encapsulate(render.bounds);
        }

        Destroy(roadReference);
        return combinedBounds.size.z;
    }

    void ConstructPathways()
    {
        GameObject allRoads = new GameObject("All Roads");
        allRoads.transform.position = Vector3.zero;
        allRoads.transform.rotation = Quaternion.identity;

        for (int i = 0; i < roadAmountInPool; i++)
        {
            for (int k = 0; k < roadObjects.Length; k++)
            {
                GameObject go = (GameObject)GameObject.Instantiate(roadObjects[k].roadObject,
                    roadObjects[k].roadObject.transform.position, roadObjects[k].roadObject.transform.rotation);
                go.isStatic = false;
                roads.Add(go);
                LightmapAligner.Instance.AlignLightmaps(roadObjects[k].roadObject, go);
                go.transform.SetParent(allRoads.transform);
            }
        }

        for (int i = 0; i < roads.Count; i++)
        {
            if (i != 0)
                roads[i].transform.position = new Vector3(0f, roads[i].transform.position.y,
                    roads[i - 1].transform.position.z + pathwayLengths[(index <= 0) ? roadObjects.Length - 1 : index - 1]);

            index++;

            if (index >= roadObjects.Length)
                index = 0;
        }

        for (int j = 0; j < roadObjects.Length; j++)
        {
            if (roadObjects[j].roadObject.activeSelf)
                roadObjects[j].roadObject.SetActive(false);
        }

        index = 0;
    }

    void Update()
    {
        if (keepAnimating)
            SteerPathways();
    }

    void SteerPathways()
    {
        for (int i = 0; i < roads.Count; i++)
        {
            if (reference.transform.position.z > (roads[i].transform.position.z + (pathwayLengths[index] * 2f)))
            {
                roads[i].transform.position = new Vector3(0f, roads[i].transform.position.y,
                    (roads[i].transform.position.z + (pathwayLengths[index] * roads.Count)));
            }

            index++;

            if (index >= roadObjects.Length)
                index = 0;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 1f, 0f, .75f);
        Gizmos.DrawCube(Vector3.zero, new Vector3(roadWidth * 3f, 1f, 10f));
    }
}
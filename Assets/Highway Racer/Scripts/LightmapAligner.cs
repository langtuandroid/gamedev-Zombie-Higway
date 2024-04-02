using UnityEngine;
using System.Collections;

public class LightmapAligner : MonoBehaviour
{
    #region SINGLETON PATTERN

    public static LightmapAligner _instance;

    public static LightmapAligner Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<LightmapAligner>();

                if (_instance == null)
                {
                    GameObject container = new GameObject("SetLightmaps");
                    _instance = container.AddComponent<LightmapAligner>();
                }
            }

            return _instance;
        }
    }

    #endregion

    internal Renderer[] referenceRenderers;
    internal Renderer[] targetRenderers;

    internal Texture2D[] m_lightmapArray;

    public void AlignLightmaps(GameObject referenceMainGameObject, GameObject targetMainGameObject)
    {
        referenceRenderers = referenceMainGameObject.GetComponentsInChildren<Renderer>();
        targetRenderers = targetMainGameObject.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < targetRenderers.Length; i++)
        {
            targetRenderers[i].lightmapIndex = referenceRenderers[i].lightmapIndex;
            targetRenderers[i].lightmapScaleOffset = referenceRenderers[i].lightmapScaleOffset;
        }
    }
}
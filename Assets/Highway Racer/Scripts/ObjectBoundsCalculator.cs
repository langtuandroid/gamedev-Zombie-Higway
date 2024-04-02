using UnityEngine;
using System.Collections;

public class ObjectBoundsCalculator : MonoBehaviour
{
    public static Vector3 GetBoundsCenter(Transform obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();

        Bounds bounds = new Bounds();
        bool initBounds = false;
        foreach (Renderer r in renderers)
        {
        }

        Vector3 center = bounds.center;
        return center;
    }
}
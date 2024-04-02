using UnityEngine;
using System.Collections;

public static class BoundingBoxExtensions
{
    public static bool ContainsBoundingBox(Transform t, Bounds bounds, Bounds target)
    {
        if (bounds.Contains(target.ClosestPoint(t.position)))
        {
            return true;
        }

        return false;
    }
}
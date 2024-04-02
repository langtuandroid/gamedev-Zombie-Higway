using UnityEngine;
using System.Collections;

public class ShadowConstraint : MonoBehaviour
{
    private Transform parentTransform;

    void Start()
    {
        parentTransform = transform.parent;
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(90, parentTransform.eulerAngles.y, 0);
    }
}
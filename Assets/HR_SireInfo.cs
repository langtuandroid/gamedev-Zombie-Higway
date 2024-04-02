using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HR_SireInfo : MonoBehaviour
{
    // public BoxCollider CapsuleCollider;
	internal BoxCollider triggerCollider;

	public ChangingLines changingLines;
	public enum ChangingLines{Straight, Right, Left}
	internal int currentLine = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

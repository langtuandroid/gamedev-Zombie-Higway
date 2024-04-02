using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
	public bool SpawnOnStart = true;
	public float TimeSpawn = 120;
	public GameObject[] Object;
	public int MaxSpwawn = 3;
	public Vector3 Offset = new Vector3 (0, 0.1f, 0);
	private float timeTemp = 0;
	private List<GameObject> spwnedList = new List<GameObject> ();
	
	void Start ()
	{
		if(SpawnOnStart)
			Spawn ();
	}
	
	void Spawn ()
	{
		ObjectExistCheck ();
		if (ObjectsNumber < MaxSpwawn) {
			if (Object.Length > 0) {
				GameObject spawnPick = Object [Random.Range (0, Object.Length)];
				GameObject objspawned = null;

				Vector3 spawnPoint = DetectGround (transform.position + new Vector3 (Random.Range (-(int)(this.transform.localScale.x / 2.0f), (int)(this.transform.localScale.x / 2.0f)), 0, Random.Range ((int)(-this.transform.localScale.z / 2.0f), (int)(this.transform.localScale.z / 2.0f))));
				objspawned = (GameObject)GameObject.Instantiate (spawnPick.gameObject, spawnPoint, Quaternion.identity);

				if (objspawned)
					spwnedList.Add (objspawned);

			}
			timeTemp = Time.time;
		}
	}
	
	private int ObjectsNumber;

	void ObjectExistCheck ()
	{
		ObjectsNumber = 0;
		foreach (var obj in spwnedList) {
			if (obj != null)
				ObjectsNumber++;
		}
	}
	
	void Update ()
	{
		if (Time.time > timeTemp + TimeSpawn) {
			Spawn ();
		}
	}
	
	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (transform.position, 0.2f);
		Gizmos.DrawWireCube (transform.position, this.transform.localScale);
	}
	
	Vector3 DetectGround (Vector3 position)
	{
		RaycastHit hit;
		if (Physics.Raycast (position, -Vector3.up, out hit,float.MaxValue)) {
			return hit.point + Offset;
		}
		return position;
	}
}

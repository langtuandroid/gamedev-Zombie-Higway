using UnityEngine;
using System.Collections;
using HWRWeaponSystem;
public class Warfactory : DamageManager {

	[HideInInspector]
	public GameObject LatestHit;

	void Start () {
	
	}
	public void ApplyDamage (DamagePack damage)
	{

		if (HP < 0)
			return;

		LatestHit = damage.Owner;
		HP -= damage.Damage;
		if (HP <= 0) {
			Dead ();
		}
	}

	void Update () {
	
	}
}

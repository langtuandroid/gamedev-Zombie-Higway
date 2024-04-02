﻿using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class ObjectPool : MonoBehaviour
	{
		[HideInInspector]
		public bool Active;
		[HideInInspector]
		public GameObject Prefab;
		public float LifeTime;
		private Vector3 positionTemp;
		private Quaternion rotationTemp;
		private Vector3 scaleTemp;
		private Rigidbody rigidBody;
		private LineRenderer lineRenderer;
		private TrailRenderer trailRenderer;
		private float trailTemp;
	
		void Awake ()
		{
			scaleTemp = this.transform.localScale;
			positionTemp = this.transform.position;
			rotationTemp = this.transform.rotation;
			rigidBody = this.GetComponent<Rigidbody> ();
			lineRenderer = this.GetComponent<LineRenderer> ();
			trailRenderer = this.GetComponent<TrailRenderer> ();
			if (trailRenderer)
				trailTemp = trailRenderer.time;
		}

		void Start ()
		{
		
		}
	
		void OnEnable ()
		{
			if (LifeTime > 0) {
				StartCoroutine (setDestrying (LifeTime));
			}
		}

		public virtual void OnSpawn (Vector3 position, Vector3 scale, Quaternion rotation, GameObject prefab, float lifeTime)
		{
			if (lifeTime != -1)
				LifeTime = lifeTime;
		
			if (GetComponent<Renderer>())
				GetComponent<Renderer>().enabled = true;

			Prefab = prefab;
			this.transform.position = position;
			this.transform.rotation = rotation;
			this.transform.localScale = scale;
			scaleTemp = this.transform.localScale;
			positionTemp = this.transform.position;
			rotationTemp = this.transform.rotation;
		
			if (rigidBody) {
				rigidBody.velocity = Vector3.zero;
				rigidBody.angularVelocity = Vector3.zero;
			}
			if (lineRenderer) {
				lineRenderer.SetPosition (0, this.transform.position);
				lineRenderer.SetPosition (1, this.transform.position);
			}
			if (GetComponent<ParticleSystem>()) {
				GetComponent<ParticleSystem>().Play ();
			}
			if (trailRenderer) {
				trailRenderer.time = trailTemp;	
			}
		
			Active = true;
		
			this.gameObject.SetActive (true);
		
			if (LifeTime > 0) {
				StartCoroutine (setDestrying (LifeTime));
			}
			StartCoroutine (resetTrail ());
		}
	
		private IEnumerator resetTrail ()
		{
			if (trailRenderer) {
				trailRenderer.time = -trailRenderer.time;	
				yield return new WaitForSeconds(0.01f);        
				trailRenderer.time = trailTemp; 
			}
		}

		public IEnumerator setDestrying (float time)
		{
			yield return new WaitForSeconds(time);
			// OnDestroyed ();

			Destroy(this);
		}

		public void SetDestroy (float time)
		{
			StartCoroutine (setDestrying (time));
			
		}
		
		public void Destroying (float time)
		{

			Destroy(this);
			// SetDestroy(time);
		}

		public void Destroying ()
		{
		
			if (GetComponent<Renderer>())
				GetComponent<Renderer>().enabled = false;
		
			this.transform.localScale = scaleTemp;
			this.transform.position = positionTemp;
			this.transform.rotation = rotationTemp;
			if (rigidBody) {
				rigidBody.velocity = Vector3.zero;
				rigidBody.angularVelocity = Vector3.zero;
			}
			if (lineRenderer) {
				lineRenderer.SetPosition (0, this.transform.position);
				lineRenderer.SetPosition (1, this.transform.position);
			}

			if (GetComponent<ParticleSystem>()) {
				GetComponent<ParticleSystem>().Stop ();
			}
			if (trailRenderer) {
				trailRenderer.time = 0;	
#if UNITY_5
				trailRenderer.Clear();
#endif
			}
			Destroy(this);
			// this.gameObject.SetActive (false);
			Active = false;
		}

		public virtual void OnDestroyed ()
		{
			Destroying ();
		}
	}
}
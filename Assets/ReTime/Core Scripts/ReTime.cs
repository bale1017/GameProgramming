using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReTime : MonoBehaviour {
	//flagging to enable or disable rewind
	[HideInInspector]
	public bool isRewinding = false;

	//use a linked list data structure for better performance of accessing previous positions and rotations;
	private LinkedList<PointInTime> PointsInTime;

	//the key that triggers the rewind
	[Tooltip("Type the letter or name of the key you want to trigger the rewind with. Check the PDF guide for more information.")]
	public string KeyTrigger;

	[HideInInspector]
	public bool UseInputTrigger;
	private bool hasAnimator = false;
	private bool hasRb = false;
	public Animator animator;

	[HideInInspector]
	public float RewindSeconds = 5;

	[HideInInspector]
	public float RewindSpeed = 1;
	private bool isFeeding = true;
	private ParticleSystem Particles;

	[HideInInspector]
	public bool PauseEnd;


	// Use this for initialization
	void Start () {
		PointsInTime = new LinkedList<PointInTime> ();

		//if contains particle system, then cache and add comp.
		if (GetComponent<ParticleSystem> ())
			Particles = GetComponent<ParticleSystem> ();
		
		//if going to use keyboard input, cache the key result and transform to lower case to avoid errors
		if(UseInputTrigger)
			KeyTrigger = KeyTrigger.ToLower ();

		//cache if has animator
		if (GetComponent<Animator> ()) {
			hasAnimator = true;
			animator = GetComponent<Animator> ();
		}

		//has rigidbody or not
		if (GetComponent<Rigidbody> ())
			hasRb = true;

		//Add the time rewind script to all children - Bubbling
		foreach(Transform child in transform){
			child.gameObject.AddComponent<ReTime> ();
			child.GetComponent<ReTime> ().UseInputTrigger = UseInputTrigger;
			child.GetComponent<ReTime> ().KeyTrigger = KeyTrigger;
			child.GetComponent<ReTime> ().RewindSeconds = RewindSeconds;
			child.GetComponent<ReTime> ().RewindSpeed = RewindSpeed;
			child.GetComponent<ReTime> ().PauseEnd = PauseEnd;
		}
	}

	void Update () {
		//when specific input is triggered then start rewind else, stop
		if (UseInputTrigger) 
			if (Input.GetKey (KeyTrigger))
				StartRewind ();
			else
				StopRewind ();
	}

	void FixedUpdate(){
		ChangeTimeScale (RewindSpeed);
		//if true then run the rewind method, else record the events
		if (isRewinding) {
			Rewind ();
		}else{
			Time.timeScale = 1f;
			if(isFeeding)
				Record ();
		}
	}

	//The Rewind method
	void Rewind(){
		if (PointsInTime.Count > 0 ) {
			PointInTime PointInTime = PointsInTime.First.Value;
			transform.position = PointInTime.position;
			transform.rotation = PointInTime.rotation;
			PointsInTime.RemoveFirst();
		} else {
			if(PauseEnd)
				Time.timeScale = 0;
			else
				StopRewind ();
		}
	}

	//use the constructor to add the new data
	void Record(){
		if(PointsInTime.Count > Mathf.Round(RewindSeconds / Time.fixedDeltaTime)){
			PointsInTime.RemoveLast ();
		}
		PointsInTime.AddFirst (new PointInTime (transform.position, transform.rotation));
		if (Particles)
		if (Particles.isPaused) {
			Particles.Play ();
		}
	}

	void StartRewind(){
		isRewinding = true;
		if(hasAnimator)
			animator.enabled = false;

		if (hasRb)
			GetComponent<Rigidbody> ().isKinematic = true;
	}

	void StopRewind(){
		Time.timeScale = 1;
		isRewinding = false;
		if(hasAnimator)
			animator.enabled = true;

		if (hasRb)
			GetComponent<Rigidbody> ().isKinematic = false;
	}
		
	void ChangeTimeScale(float speed){
		Time.timeScale = speed;
		if (speed > 1)
			Time.fixedDeltaTime = 0.02f / speed;
		else
			Time.fixedDeltaTime = speed * 0.02f;
	}

	//exposed method to enable rewind
	public void StartTimeRewind(){
		isRewinding = true;

		if(hasAnimator)
			animator.enabled = false;

		if (hasRb)
			GetComponent<Rigidbody> ().isKinematic = true;
		
		if(transform.childCount > 0){
			foreach (Transform child in transform)
				child.GetComponent<ReTime> ().StartRewind ();
		}
	}

	//exposed method to disable rewind
	public void StopTimeRewind(){
		isRewinding = false;
		Time.timeScale = 1;
		if(hasAnimator)
			animator.enabled = true;

		if (hasRb)
			GetComponent<Rigidbody> ().isKinematic = false;

		if(transform.childCount > 0){
			foreach (Transform child in transform) {
				child.GetComponent<ReTime> ().StopTimeRewind ();
			}
		}
	}

	//Check point end for parent obect
	public void StopFeeding(){
		isFeeding = false;

		if(transform.childCount > 0){
			foreach (Transform child in transform) {
				child.GetComponent<ReTime> ().StopFeeding ();
			}
		}
	}

	//Check point start for parent obect
	public void StartFeeding(){
		isFeeding = true;

		if(transform.childCount > 0){
			foreach (Transform child in transform) {
				child.GetComponent<ReTime> ().StartFeeding ();
			}
		}
	}

	//on adding ReTime component, also add the particles script to all objects that are PS
	void Reset(){
		if(GetComponent<ParticleSystem>())
			gameObject.AddComponent<ReTimeParticles> ();
		
		//Add the particles script to all children that are PS - Bubbling
		foreach (Transform child in transform) {
			if(child.GetComponent<ParticleSystem>())
				child.gameObject.AddComponent<ReTimeParticles> ();
		}
	}
}

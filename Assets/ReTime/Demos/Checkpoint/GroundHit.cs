using UnityEngine;

public class GroundHit : MonoBehaviour {

	//when the cube collides with the ground, stop recording
	void OnCollisionEnter(Collision col){
		GetComponent<ReTime> ().StopFeeding();
	}


	//when holding A to rewind, start recording again. After stop feeding you always need to re-enable recording by start feeding
	void Update(){
		if(Input.GetKey(KeyCode.A)){
			GetComponent<ReTime> ().StartFeeding();
		}
	}

}

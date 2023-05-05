using UnityEngine;

public class MethodRun : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space)){
			GetComponent<ReTime> ().StartTimeRewind ();
		}

		if(Input.GetKeyDown(KeyCode.Return)){
			GetComponent<ReTime> ().StopTimeRewind ();
		}
	}
}

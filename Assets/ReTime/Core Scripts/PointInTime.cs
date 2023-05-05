using UnityEngine;

public class PointInTime {
	//save position and rotation
	public Vector3 position;
	public Quaternion rotation;

	//constructor
	public PointInTime(Vector3 _position, Quaternion _rotation){
		position = _position;
		rotation = _rotation;
	}
}

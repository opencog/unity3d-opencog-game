using UnityEngine;
using System.Collections;

public class Trackball : MonoBehaviour {

	void Update () {
		if(Input.GetMouseButton(1)) {
			float screenSize = Mathf.Max(Screen.width, Screen.height);
			float tx = (Input.mousePosition.x-Screen.width/2)/screenSize * 2;
			float ty = (Input.mousePosition.y-Screen.height/2)/screenSize * 2;
			
			tx = Mathf.Clamp(tx, -1, 1);
			ty = Mathf.Clamp(ty, -1, 1);
			
			
			Vector3 lever = new Vector3(tx, ty, 0);
			lever.z = -Mathf.Clamp01(1-lever.magnitude);
			lever.Normalize();
			
			Vector3 delta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			
			Vector3 axis = Vector3.Cross(lever, delta);
			axis = Camera.mainCamera.transform.TransformDirection(axis);
			transform.Rotate(axis, delta.magnitude*5, Space.World);
			
			Debug.DrawLine(transform.position, transform.position+lever, Color.green);
		}
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position, 0.1f);
	}
}

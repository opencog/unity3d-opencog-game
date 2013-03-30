using UnityEngine;
using System.Collections;

public class CharacterCollider : MonoBehaviour {
	
	public float height;
	public float radius;
	
	public Vector3 top {
		get {
			return Vector3.up*(height-radius);
		}
	}
	
	public Vector3 bottom {
		get {
			return Vector3.up*radius;
		}
	}
	
	private Map map;
	
	private Vector3 groundPoint, groundNormal;
	private Vector3 deltaPosition;

	// Use this for initialization
	void Awake () {
		map = (Map) GameObject.FindObjectOfType(typeof(Map));
	}
	
	void OnDrawGizmos() {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position+top, radius);
		Gizmos.DrawWireSphere(transform.position+bottom, radius);
		
		if(IsGrounded()) {
			Gizmos.color = Color.red;
			Gizmos.DrawRay(groundPoint, groundNormal*0.5f);
		}
	}
	
	public void Move(Vector3 delta) {
		deltaPosition = delta;
		Vector3 oldPos = transform.position;
		transform.position += delta;
		groundPoint = groundNormal = Vector3.zero;
		MapCharacterCollision.Collision(map, this);
		deltaPosition = transform.position - oldPos;
	}
	
	public void OnCollision(Vector3 point, Vector3 normal) {
		if( deltaPosition.y < 0 &&normal.y > 0.001f && normal.y > this.groundNormal.y) {
			this.groundPoint = point;
			this.groundNormal = normal;
		}
	}
	
	public Vector3 GetDeltaPosition() {
		return deltaPosition;
	}
	
	public bool IsGrounded() {
		return groundNormal.y > 0;
	}
	
}

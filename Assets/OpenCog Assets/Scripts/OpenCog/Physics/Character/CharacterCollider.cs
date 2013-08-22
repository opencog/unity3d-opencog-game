using UnityEngine;
using System.Collections;

//public class CharacterCollider
//: MonoBehaviour {
//	
//	public float height;
//	public float radius;
//	
//	//private float yCorrection = 0.50f;
//	
//	public Vector3 top {
//		get {
//			Vector3 v3Top = Vector3.up*(height-radius);
//			
//			//v3Top.y -= yCorrection;
//			
////			if (System.DateTime.Now.Millisecond.ToString ().EndsWith ("99"))
////				Debug.Log ("v3Top [" + v3Top.x + ", " + v3Top.y + ", " + v3Top.z + "]");
//			
//			return v3Top;
//		}
//	}
//	
//	public Vector3 bottom {
//		get {
//			Vector3 v3Bottom = Vector3.up*radius;
//			
//			//v3Bottom.y -= yCorrection;
//			
////			if (System.DateTime.Now.Millisecond.ToString ().EndsWith ("99"))
////				Debug.Log ("v3Bottom [" + v3Bottom.x + ", " + v3Bottom.y + ", " + v3Bottom.z + "]");
//			
//			return v3Bottom;
//		}
//	}
//	
//	private OpenCog.Map.OCMap map;
//	
//	private Vector3 groundPoint, groundNormal;
//	private Vector3 deltaPosition;
//
//	// Use this for initialization
//	void Awake () {
//		map = (OpenCog.Map.OCMap) GameObject.FindObjectOfType(typeof(OpenCog.Map.OCMap));
//	}
//	
//	void OnDrawGizmos() {
//		Gizmos.color = Color.green;
//		Gizmos.DrawWireSphere(transform.position+top, radius);
//		Gizmos.DrawWireSphere(transform.position+bottom, radius);
//		
//		if(IsGrounded()) {
//			Gizmos.color = Color.red;
//			Gizmos.DrawRay(groundPoint, groundNormal*0.5f);
//		}
//	}
//	
//	public void Move(Vector3 delta) {
//		deltaPosition = delta;
//		Vector3 oldPos = transform.position;
//		transform.position += delta;
//		groundPoint = groundNormal = Vector3.zero;
//		MapCharacterCollision.Collision(map, this);
//		deltaPosition = transform.position - oldPos;
//	}
//	
//	public void OnCollision(Vector3 point, Vector3 normal) {
//		if( deltaPosition.y < 0 &&normal.y > 0.001f && normal.y > this.groundNormal.y) {
//			this.groundPoint = point;
//			this.groundNormal = normal;
//		}
//	}
//	
//	public Vector3 GetDeltaPosition() {
//		return deltaPosition;
//	}
//	
//	public bool IsGrounded() {
//		return groundNormal.y > 0;
//	}
//	
//}

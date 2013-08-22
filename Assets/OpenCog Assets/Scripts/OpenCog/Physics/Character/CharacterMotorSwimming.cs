using UnityEngine;
using System.Collections;

//public class CharacterMotorSwimming : MonoBehaviour {
//	
//	private const float moveSpeed = 5f;
//	private const float maxAcceleration = 10;
//	
//	private const float gravity = 15;
//	private const float maxFallSpeed = 4;
//	
//	private CharacterController character;
//	
//	
//	[System.NonSerialized]
//	public Vector3 inputMoveDirection = Vector3.zero;
//	
//	[System.NonSerialized]
//	public bool inputEmersion = false;
//	
//	
//	void Awake() {
//		character = GetComponent<CharacterController>();
//	}
//	
//	
//	void FixedUpdate() {
//		Vector3 velocity = character.GetDeltaPosition() / Time.deltaTime;
//		ApplyMoving(ref velocity);
//		ApplyGravity(ref velocity);
//		ApplyEmersion(ref velocity);
//		
//		character.Move( velocity*Time.deltaTime );
//	}
//	
//	
//	private void ApplyMoving(ref Vector3 velocity) {
//		Vector3 desiredVelocity = inputMoveDirection * moveSpeed;
//		Vector3 delta = desiredVelocity - new Vector3(velocity.x, 0, velocity.z);
//		velocity += Vector3.ClampMagnitude(delta, maxAcceleration);
//	}
//	
//	private void ApplyGravity(ref Vector3 velocity) {
//		velocity.y -= gravity * Time.deltaTime;
//		velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
//	}
//	
//	private void ApplyEmersion(ref Vector3 velocity) {
//		if(inputEmersion) {
//			velocity.y += 2*gravity * Time.deltaTime;
//			velocity.y = Mathf.Min(velocity.y, maxFallSpeed);
//		}
//	}
//	
//	
//	
//}

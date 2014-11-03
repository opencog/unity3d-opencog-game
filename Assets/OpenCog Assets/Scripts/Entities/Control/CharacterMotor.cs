using UnityEngine;
using System.Collections;

//public class CharacterMotor : MonoBehaviour {
//	
//	private CharacterController character;
//	
//	private CharacterMotorMoving motorMoving = new CharacterMotorMoving();
//	private CharacterMotorJumping motorJumping = new CharacterMotorJumping();
//	
//	[System.NonSerialized]
//	public Vector3 inputMoveDirection = Vector3.zero;
//	
//	[System.NonSerialized]
//	public bool inputJump = false;
//	
//	[System.NonSerialized]
//	public bool holdingInputJump = false;
//	
//	void Awake() {
//		character = GetComponent<CharacterController>();
//	}
//	
//	
//	void FixedUpdate() {
//		Vector3 velocity = character.GetDeltaPosition() / Time.deltaTime;
//		motorMoving.ApplyMoving(this, ref velocity);
//		motorMoving.ApplyGravity(this, ref velocity);
//		motorJumping.ApplyJumping(this, ref velocity);
//		
//		character.Move( velocity*Time.deltaTime );
//	}
//	
//	public bool IsGrounded() {
//		return character.IsGrounded();
//	}
//	
//}
//
//class CharacterMotorMoving {
//	// максимаьные скорости движения
//	private const float moveSpeed = 6f;
//	
//	// максимальное изменение скорости за секунду
//	private const float maxGroundAcceleration = 20;
//	private const float maxAirAcceleration = 5;
//
//	// гравитация
//	public const float gravity = 20;
//	// максимальная скорость падения
//	private const float maxFallSpeed = 5;
//	
//	public void ApplyMoving(CharacterMotor motor, ref Vector3 velocity) {
//		Vector3 desiredVelocity = motor.inputMoveDirection * moveSpeed;
//		Vector3 delta = desiredVelocity - new Vector3(velocity.x, 0, velocity.z);
//		float maxDelta = GetMaxAcceleration(motor.IsGrounded()) * Time.deltaTime;
//		velocity += Vector3.ClampMagnitude(delta, maxDelta);
//	}
//	
//	public void ApplyGravity(CharacterMotor motor, ref Vector3 velocity) {
//		velocity.y -= gravity * Time.deltaTime;
//		velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
//		if(motor.IsGrounded()) velocity.y = Mathf.Min(velocity.y, 0);
//	}
//	
//	private static float GetMaxAcceleration(bool grounded) {
//		if(grounded) return maxGroundAcceleration;
//		return maxAirAcceleration;
//	}
//	
//}
//
//class CharacterMotorJumping {
//	
//	private const float baseHeight = 1.0f;
//	private const float extraHeight = 1.4f;
//	
//	private bool jumping = false;
//	private float jumpStartTime;
//	
//	public void ApplyJumping(CharacterMotor motor, ref Vector3 velocity) {
//		if (motor.IsGrounded() && !jumping && motor.inputJump) {
//			jumping = true;
//			jumpStartTime = Time.time;
//			
//			// Apply the jumping force to the velocity. Cancel any vertical velocity first.
//			velocity.y = 0;
//			velocity += Vector3.up * CalculateJumpVerticalSpeed(baseHeight);
//			return;
//		}
//		if (jumping && motor.holdingInputJump) {
//			// увеличиваем высоту прыжка
//			if (Time.time < jumpStartTime + extraHeight / CalculateJumpVerticalSpeed(baseHeight)) {
//				velocity += Vector3.up * CharacterMotorMoving.gravity * Time.deltaTime;
//			}
//		}
//		if(motor.IsGrounded() || velocity.y <= 0) {
//			jumping = false;
//		}
//	}
//	
//	
//	private static float CalculateJumpVerticalSpeed(float targetJumpHeight) {
//		// From the jump height and gravity we deduce the upwards speed 
//		// for the character to reach at the apex.
//		return Mathf.Sqrt(2 * targetJumpHeight * CharacterMotorMoving.gravity);
//	}
//	
//}
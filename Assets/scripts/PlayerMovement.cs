using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed = 6f;            // The speed that the player will move at.
	public float jump_power=6f;
	Vector3 movement;                   // The vector to store the direction of the player's movement.
	Animator anim;                      // Reference to the animator component.
	Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
	int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
	float camRayLength = 100f;          // The length of the ray from the camera into the scene.
	Vector3 m_GroundNormal;
	bool m_IsGrounded;
	[SerializeField] float m_GroundCheckDistance = 0.1f;
	float m_OrigGroundCheckDistance;
	CharacterController controler;
	Vector3 vel;
	Vector3 myGravity ;
	void Awake ()
	{
		// Create a layer mask for the floor layer.
		//floorMask = LayerMask.GetMask ("Floor");
		
		// Set up references.
		anim = GetComponent <Animator> ();
		controler = GetComponent <CharacterController> ();
		playerRigidbody = GetComponent <Rigidbody> ();
		m_OrigGroundCheckDistance = m_GroundCheckDistance;
		myGravity.Set (0.0f, 9.8f, 0.0f);
	}
	
	
	void FixedUpdate ()
	{
		// Store the input axes.
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		bool jump= false;
		if (!jump)
		{
			//jump = CrossPlatformInputManager.GetButtonDown("Jump");
			jump=Input.GetButtonDown("Jump");
		}
	
		// Move the player around the scene.
		Move (h, v,jump);
		
		// Turn the player to face the mouse cursor.

		
		// Animate the player.
		Animating (h, v);
	}
	void GroundedMove(bool jump)
	{
		//playerRigidbody.MovePosition (transform.position + movement);
		if (movement.magnitude > 0.1) {
			vel = speed * movement.normalized;
		} else {
			vel.Set(0f,0f,0f);
		}
		if (jump) {
			vel.Set (vel.x,jump_power,vel.z);
			m_IsGrounded = false;
			m_GroundCheckDistance = 0.1f;
			anim.SetTrigger("Jump");
		} else {
			//playerRigidbody.velocity = speed * movement.normalized;

			Turning (movement.x,movement.z);
		}
		playerRigidbody.MovePosition (transform.position + vel*Time.deltaTime);
	}
	void AirborneMovement()
	{
		// apply extra gravity from multiplier:
		//Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
		//m_Rigidbody.AddForce(extraGravityForce);

		playerRigidbody.MovePosition (transform.position + vel*Time.deltaTime);
		vel = vel - myGravity* Time.deltaTime;

		m_GroundCheckDistance = vel.y < 0 ? m_OrigGroundCheckDistance : 0.1f;
	}

	void Move (float h, float v,bool jump)
	{
		// Set the movement vector based on the axis input.

		movement.Set (h, 0f, v);

		CheckGroundStatus ();
		//m_IsGrounded = controler.isGrounded;
		//m_IsGrounded = true;
		//m_IsGrounded = transform.position.y < m_GroundCheckDistance;
		// Normalise the movement vector and make it proportional to the speed per second.
		//movement = movement.normalized * speed * Time.deltaTime;
		
		// Move the player to it's current position plus the movement.
		//if (!m_IsGrounded) {

		

		//playerRigidbody.MovePosition (transform.position + movement);
		//if (m_IsGrounded && (movement.magnitude > 0 || jump)) {
		if (m_IsGrounded) {
			GroundedMove (jump);
		
		} else {
			AirborneMovement();
		}
	}
	
	void Turning (float h,float v)
	{
		// Create a ray from the mouse cursor on screen in the direction of the camera.
		//Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		
		// Create a RaycastHit variable to store information about what was hit by the ray.
		//RaycastHit floorHit;
		
		// Perform the raycast and if it hits something on the floor layer...
		/*if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
		{
			// Create a vector from the player to the point on the floor the raycast from the mouse hit.
			Vector3 playerToMouse = floorHit.point - transform.position;
			
			// Ensure the vector is entirely along the floor plane.
			playerToMouse.y = 0f;
			
			// Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
			
			// Set the player's rotation to this new rotation.
			playerRigidbody.MoveRotation (newRotation);
		}*/
		bool walking = h != 0f || v != 0f;
		if (walking) {
			Quaternion newRotation = Quaternion.LookRotation (movement);
			playerRigidbody.MoveRotation (newRotation);
		}
	}
	
	void Animating (float h, float v)
	{
		// Create a boolean that is true if either of the input axes is non-zero.
		bool walking = h != 0f || v != 0f;
		
		// Tell the animator whether or not the player is walking.
		anim.SetBool ("IsWalking", walking);

		anim.SetBool ("IsGrounded", m_IsGrounded);



	}
	void CheckGroundStatus()
	{
		RaycastHit hitInfo;
		#if UNITY_EDITOR
		// helper to visualise the ground check ray in the scene view
		Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
		#endif
		// 0.1f is a small offset to start the ray from inside the character
		// it is also good to note that the transform position in the sample assets is at the base of the character
		if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
		{
			m_GroundNormal = hitInfo.normal;
			m_IsGrounded = true;
			//anim.applyRootMotion = true;
		}
		else
		{
			m_IsGrounded = false;
			m_GroundNormal = Vector3.up;
			//anim.applyRootMotion = false;
		}
	}
}

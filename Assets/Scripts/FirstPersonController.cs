﻿using UnityEngine;
using System;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	//This class controls everything related to the player
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Tooltip("Modifier for acceleration and deceleration in midair")]
		public float AirSpeedModifier = 0.5f;

		[Tooltip("Modifier for acceleration and deceleration on the ground")]
		public float GroundedSpeedModifier = 1.5f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;
		[Tooltip("How many times the player can jump")]
		public int Jumps = 2;
		[Space(10)]
		[Tooltip("Player's resetting position")]
		public Vector3 ResetPosition;
		[Tooltip("Player's resetting layers")]
		public LayerMask ResetLayers;


		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;
		[Tooltip("Time required to pass before being able to dodge again.")]
		public float DodgeTimeout;

		[Tooltip("Initial speed of the player's dodge")]
		public float DodgeDistance = 50.0f;
		[Tooltip("Wall jump horizontal speed")]
		public float WallJumpHorizontalSpeed = 25.0f;
		[Tooltip("Time player has to wait between wall jumps, prevents some bugs")]
		public float WallJumpTimeout = 0.1f;

		[Tooltip("Dodge slow down speed (higher values decrease speed quicker)")]
		public float DodgeSlowSpeed = 2.0f;

		[Tooltip("Duration of the player's slide")]
		public float SlideDuration;
		[Tooltip("Modifier for the player's slide")]
		public float SlideModifier = 1.5f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;
		[Tooltip("How long the player has to be on the ground before GroundedSpeedModifier takes effect")]
		public float GroundedTime = 0.1f;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;

		// player
		private float _speed;
		private float _rotationVelocity; // how quick to rotate camera
		private float _verticalVelocity; //Speed used to add jump velocity
		private float _horizontalSpeed; // speed to send the player in with vector _horizontalDirection
		private Vector3 _horizontalDirection; // current direction that the player is moving in with _horizontalSpeed speed
		private float _terminalVelocity = 53.0f;
		private int _jumpsLeft; // number of jumps left

		// timeout deltatime
		private float _jumpTimeoutDelta; //jump timeout
		private float _fallTimeoutDelta; //time to fall until no longer can jump
		private float _dodgeTimeoutDelta; //dodge timeout
		private float _slideTime; //length of slide

		private Vector3 _previousDirection; //previous direction that was moved in
		private Vector3 _slideMovement; //velocity of slide
		private Vector3 _cameraPosition;
		// private float _groundedTimeout;
		private Vector3 _collisionModifiedVector; //new direction to go when colliding with a wall - makes collisions with walls smoother
		private Vector3 _lastCollisionPoint = Vector3.zero; //last collision point with a wall or other non floor object, used to know when the wall is too far away to wall run on

		private bool _wallRunPossible = false; //is a wall run currently possible
		private Vector3 _wallRunDirection = Vector3.zero; //current wall run movement vector, passed into _controller.Move() function
		private bool _wallRunning = false; //currently wall running vector
		private Vector3 _previousWallNormal = Vector3.zero; //previous normal vector that was wall run on

		private float _wallJumpSpeed = 0.0f; //Small speed to get player away from wall after jumping.

		private Vector3 _wallJumpDirection = Vector3.zero; // direction of jump away from wall while wall running

		private float _wallJumpTimeoutDelta;

		private Vector3 _inputDirection;

		private Vector3 _grappleDirection;

		private GrapplingBehavior _grappleType;


	
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		[SerializeField] public GameObject _grapplingGun;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
			
			
			
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
			_wallJumpTimeoutDelta = WallJumpTimeout;
			_jumpsLeft = Jumps;
			_cameraPosition = CinemachineCameraTarget.transform.position;
			_grappleType = new AntiGrav(_input, GetComponent<LineRenderer>(), _grapplingGun.GetComponentInChildren<Transform>(), CinemachineCameraTarget.transform);
		}

		private void Update()
		{
			WallRunAndSlide();
			Jump();
			GroundedCheck();
			Dodge();
			Slide();
			SwitchGrappleInput();
			Move();
		}

		private void LateUpdate()
		{
			CameraRotation();
		}
		private int notifyUISwitch(){
			UIDisplay.observables -= notifyUISwitch;
			return -1;
		}
		private void SwitchGrappleInput(){
			if(_input.switchGrapple){
				UIDisplay.observables += notifyUISwitch;
				_input.switchGrapple = false;
				if(_grappleType.GetType().Name == "AntiGrav"){
					_grappleType = new Impulse(_input, GetComponent<LineRenderer>(), _grapplingGun.GetComponentInChildren<Transform>(), CinemachineCameraTarget.transform);
				}
				else if(_grappleType.GetType().Name == "Impulse"){
					_grapplingGun.SetActive(false);
					_grappleType = new Unequipped(_input, GetComponent<LineRenderer>(), _grapplingGun.GetComponentInChildren<Transform>(), CinemachineCameraTarget.transform);
				}
				else{
					_grapplingGun.SetActive(true);
					_grappleType = new AntiGrav(_input, GetComponent<LineRenderer>(), _grapplingGun.GetComponentInChildren<Transform>(), CinemachineCameraTarget.transform);
				}
			}
		}
		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			bool isGrounded = Grounded;
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
			//very first occurence of being grounded will have the previous check be not grounded
			// if (!isGrounded && Grounded){
			// 	_groundedTimeout = GroundedTime;
			// }
			// else{
			// 	_groundedTimeout -= Time.deltaTime;
			// }
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
				// _grapplingGun.transform.Rotate(Vector3.up * _rotationVelocity);
				_grapplingGun.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
			//if there is no movement, set target speed to false
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			//
			//
			// Figure out why _wallJumpSpeed is causing a horizontal jump like effect where it causes a bouncing effect on the wall
			// It starts positive and ends up negative maybe?
			//
			// Also, consider bringing back the _speed = Lerp calculation when _horizontalSpeed is really small, so that it is easier to precisely move on ledges without going 0.0f or 6.0f (sprint speed)
			//

			/////////////////////////////////////////////////////////////////////////////////////////////////
			// Lastly, CONSIDER
			// making an array of a struct composed of (vector + velocity + slow down ) to iterate over before calling _controller.Move()
			// This could allow any function to add a velocity to the array, with it's slow down time included.
			//////////////////////////////////////////////////////////////////////////////////////////////////
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
			if (_horizontalSpeed < targetSpeed - 0.1f){
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(_horizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * (SpeedChangeRate));
				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			// else if(_horizontalSpeed > targetSpeed + 0.1f)
			// {
			// 	// creates curved result rather than a linear one giving a more organic speed change
			// 	// note T in Lerp is clamped, so we don't need to clamp our speed
			// 	_speed = Mathf.Lerp(_horizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * (effectiveSpeedChangeRate * _horizontalSpeed));

			// 	// round speed to 3 decimal places
			// 	_speed = Mathf.Round(_speed * 1000f) / 1000f;

			// }
			else
			{
				_speed = targetSpeed;
			}
			// }
			//set _speed to targetSpeed without modification
			// _speed = targetSpeed;
			
			Vector3 grappleGravity = _grappleType.grapple();
			
			_grappleDirection += grappleGravity;

			if(_grappleDirection.magnitude < 1.0f)
				_grappleDirection = Vector3.zero;
			_grappleDirection = _grappleDirection.normalized * (_grappleDirection.magnitude - (100.0f * Time.deltaTime));

			
			if((_horizontalSpeed <= SprintSpeed) && targetSpeed == 0.0f){ // if _horizontalSpeed speed is sufficiently small, decelerate faster
				_horizontalSpeed -= (10.0f) * Time.deltaTime;
			}
			//_horizontalSpeed decreases faster the larger it is.
			else if(_horizontalSpeed > targetSpeed){
				// print("speed: " + _speed + " _horizontalV: " + _horizontalSpeed);
				
				_horizontalSpeed -= (_horizontalSpeed/2.0f) * Time.deltaTime; 
			}
			//immediately speed up to the target speed if below it
			else if(_horizontalSpeed <= SprintSpeed && targetSpeed > 0.0f){
				_horizontalSpeed = _speed;
				_horizontalDirection = _inputDirection;
			}
			if(_horizontalSpeed <= 0.0f){ // if _horizontalSpeed is zero, set _horizontalDirection to Vector3.zero
				_horizontalSpeed = 0.0f;
				_horizontalDirection = Vector3.zero;
			}
			//Decrease vertical velocity every update
			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				if(_wallRunning || grappleGravity.magnitude != 0){
					_verticalVelocity = 0.0f; // no gravity when wall running
				}
				else{
					_verticalVelocity += Gravity * Time.deltaTime;
				}
			}
			// stop our velocity dropping infinitely when grounded
			if (Grounded && _verticalVelocity < 0f)
			{
				_verticalVelocity = -2f;
			}
			//find how related our desired direction (_inputDirection) is to our currently moved in direction (_horizontalDirection)
			//if it is in opposite directions, decrease our speed
			if(Vector3.Dot(_horizontalDirection, _inputDirection.normalized) <= 0.0f){
				
				_horizontalSpeed -= targetSpeed * Time.deltaTime;
			}
			else{
				//rotate our moving direction towards our desired direction
				_horizontalDirection = Vector3.RotateTowards(_horizontalDirection, _inputDirection.normalized, Mathf.Abs(1f/Vector3.Dot(_horizontalDirection, _inputDirection.normalized))*_inputDirection.magnitude*Time.deltaTime, 0.0f);
			}
			// print("_horizontalSpeed: " + _horizontalSpeed + " _horizontalDirection: " + _horizontalDirection + " targetSpeed: " + targetSpeed);
			// print("_verticalVelocity: " + _verticalVelocity);
			// print("wallJumpDirection: " + _wallJumpDirection);
			Debug.Log("Direction: " + _grappleDirection + " Gravity: "  +grappleGravity  +  " Magnitude: " + _grappleDirection.magnitude);
			_controller.Move((new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime) +
							(_horizontalDirection.normalized * (_horizontalSpeed * Time.deltaTime)) +
							(_wallJumpDirection * (_wallJumpSpeed * Time.deltaTime)) +
							_grappleDirection * Time.deltaTime
							);
		}
		private void WallRunAndSlide(){

			// normalize input direction
			_inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				_inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}
			
			//note: take just the x and y position of the last collision point (set in OnControllerColliderHit()) because the y position varies every hit
			if(Vector3.Distance(new Vector3(_lastCollisionPoint.x, transform.position.y, _lastCollisionPoint.z), transform.position) > .7f) // note: this 0.7f value is very much a hardcoded estimate value of when we are considered too far from a wall to continue wall runing on it
				_collisionModifiedVector = Vector3.zero;

			//exit wall run if the _collisionModifiedVector was set to zero before because we got too far from the wall
			if(!_wallRunPossible ||  _collisionModifiedVector == Vector3.zero){
				_wallJumpTimeoutDelta -= Time.deltaTime; //start decreasing the wall jump timeout
				_wallRunPossible = false;
				_wallRunning = false;
			}
			//start wall run
			else if(!Grounded && _wallRunPossible && !_wallRunning && _wallJumpTimeoutDelta <= 0.0f){ 
				_wallJumpTimeoutDelta = WallJumpTimeout; //reset the wall jump timeout
				_wallRunning = true;
				_horizontalSpeed += 5.0f;
				_verticalVelocity = 0.0f;
			}
			//this code is for finding the new vector after colliding with a wall.
			//This is very important, because it gives the player the feeling of slippery walls that do not decrease speed
			if(_collisionModifiedVector != Vector3.zero){
				float dot = Vector3.Dot(_inputDirection.normalized, _collisionModifiedVector.normalized);
				if(dot < 0.4f  || dot > 1.5f){
					_collisionModifiedVector = Vector3.zero;
				}
				
				_inputDirection = Vector3.Project(_inputDirection, _collisionModifiedVector);
			}
			
			//if we are not currently wall running, remember the last good _inputDirection in _wallRunDirection, so we can set _horizontalDirection to this value when we are wall running
			if(!_wallRunning){
				_wallRunDirection = _inputDirection.normalized;
				// print("_wallJumpSpeed: " + _wallJumpSpeed);
				if(_wallJumpSpeed > 0.0f) // _wallJumpSpeed is just a speed that takes us away from walls with a burst and then goes away,, it is decreased here
					_wallJumpSpeed -= 20f * Time.deltaTime;
				else
					_wallJumpSpeed = 0.0f;
			}
			else{
                _horizontalDirection = _wallRunDirection;
			}

		}
		private void Jump()
		{
			if (Grounded || _wallRunning)
			{
				
				_jumpsLeft = Jumps;
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					_jumpTimeoutDelta = JumpTimeout;
					_input.jump = false;
					_wallRunPossible = false;
					if(_wallRunning){
						// _horizontalSpeed += WallJumpHorizontalSpeed;
						// _horizontalDirection = (_previousWallNormal + _wallRunDirection.normalized* 0.50f);
						// _wallJumpVector = _previousWallNormal + _wallRunDirection.normalized;
						_wallJumpSpeed = WallJumpHorizontalSpeed;
						_wallJumpDirection = _previousWallNormal;
						print("wall jumped");
					}
					_wallRunning = false;
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
				
			}
			else if(_fallTimeoutDelta >= 0.0f){
				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					_jumpTimeoutDelta = JumpTimeout;
					_input.jump = false;
				}
				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				if(_jumpsLeft > 0){


					// Jump
					if (_input.jump && _jumpTimeoutDelta <= 0.0f)
					{
						// print("Jumped!");
						// the square root of H * -2 * G = how much velocity needed to reach desired height
						_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
						_jumpsLeft -= 1;
						_jumpTimeoutDelta = JumpTimeout;
					}else{
						_input.jump = false;
					}

					// jump timeout
					if (_jumpTimeoutDelta >= 0.0f)
					{
						_jumpTimeoutDelta -= Time.deltaTime;
					}
				}
				// if we are not grounded, do not jump
				_input.jump = false;
			}
			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}

		}
		private int notifyUIDodge(){
			
			UIDisplay.observables -= notifyUIDodge;
			
			return 1;
		}
		private void Dodge(){
			if(_input.dodge){
				if(_dodgeTimeoutDelta <= 0.0f){
					_horizontalSpeed += DodgeDistance;
					_horizontalDirection = _inputDirection;
					_dodgeTimeoutDelta = DodgeTimeout;
					UIDisplay.observables += notifyUIDodge;
				}
				_input.dodge = false;
			}
			if(_dodgeTimeoutDelta >= 0.0f){
				_dodgeTimeoutDelta -= Time.deltaTime;
			}
			
		}
		private void Slide(){
			if(_input.slide && _slideTime < -0.1f){
				//
				//
				//
				// CONSIDER: using Vector3.SmoothDamp
				//
				//
                //
				
				CinemachineCameraTarget.transform.localPosition = new Vector3(_cameraPosition.x, _cameraPosition.y - 0.5f, _cameraPosition.z);
				if(_input.sprint && Grounded){
					_slideTime = SlideDuration;
					_horizontalSpeed += SlideModifier;
				}
			}
			_slideTime -= Time.deltaTime;

			if(_slideTime <= 0.0f){
				//only want to do the following once after the slideTime has ended
				if(_input.slide){
					CinemachineCameraTarget.transform.localPosition = _cameraPosition;
				}
				_input.slide = false;
			}
			if(!Grounded){
				if(_input.slide){
					CinemachineCameraTarget.transform.localPosition = _cameraPosition;
				}
				_input.slide = false;
			}
		}
		private void OnTriggerEnter(Collider collider){
			// print("Collided: " + collider.name);
			if((1 << collider.gameObject.layer & ResetLayers.value) != 0){
				// print(gameObject.name);
				_controller.enabled = false;
				_horizontalSpeed = 0.0f;
				_verticalVelocity = 0.0f;
				_jumpsLeft = 0;
				gameObject.transform.position = new Vector3(0.0f, 50.0f, 0.0f);
				_controller.enabled = true;
			}
		}

		private void OnControllerColliderHit(ControllerColliderHit hit){

			if((hit.moveDirection.y < -0.3f) || hit.moveDirection == hit.normal || hit.gameObject.tag == "PushableObject"){
				return;
			}

			Vector3 newDirection1 = Vector3.zero;
			if(hit.gameObject.tag == "WallRunAble"){
				_wallRunPossible = true;
			}
			// Vector3 newDirection2 = Vector3.zero;
			if(hit.normal.x != 0.0f){
				newDirection1.x = hit.normal.z;
				newDirection1.z = -hit.normal.x;
			}
			else if(hit.normal.z != 0.0f){
				newDirection1.z = hit.normal.x;
				newDirection1.x = -hit.normal.z;
			}

			_collisionModifiedVector = (Vector3.Project(hit.moveDirection, newDirection1).normalized);

			_lastCollisionPoint = hit.point;
			_previousWallNormal = hit.normal;

		}
		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}
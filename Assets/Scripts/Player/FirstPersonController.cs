﻿using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		public float MoveSpeed = 4.0f;
		public float SpeedChangeRate = 10.0f;

		public float JumpHeight = 1.1f;
		public float Gravity = -15.0f;

		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		public bool grounded = true;
        [Tooltip("Useful for rough ground")]
		public static float GroundedOffset = -0.1f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		public float soundDelay = 2.0f;
		public float soundTimer;

        [Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

        public static float LeftClamp;
        public static float RightClamp;

        // cinemachine
        private float _cinemachineTargetPitch;
		public static float _cinemachineTargetYaw;

        // player
        private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = -53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

	
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		private FootstepSounds footstepSounds;
        [SerializeField] private AudioSource landingSound;
		private Rigidbody rb;
		
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private Image square;

        private const float _threshold = 0.001f;

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
            footstepSounds = GetComponent<FootstepSounds>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
			soundTimer = soundDelay;

            rb = GetComponent<Rigidbody>();
        }

		private void FixedUpdate()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();
        }

        private void Update()
        {
            CheckPause();
        }

        private void LateUpdate()
		{
			if (Time.timeScale >= 1)
				CameraRotation();
		}

        private void OnCollisionEnter(Collision collision)
		{
            landingSound.volume = collision.relativeVelocity.magnitude/2;
            landingSound.Play();
        }

        private void GroundedCheck()
		{
			// set player position, with offset
			Vector3 playerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

			Collider[] objs = Physics.OverlapBox(playerPosition, new Vector3(.2f, .1f, .1f), Quaternion.identity, GroundLayers, QueryTriggerInteraction.Ignore);
			if (objs.Length == 0 || _verticalVelocity > 0 || (objs.Length == 1 && objs[0].gameObject == InteractController.heldObject))
				grounded = false;
			else
				grounded = true;

        }

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y;
				_rotationVelocity = _input.look.x;

				if (InteractController.interactable)
				{
					_cinemachineTargetYaw += _input.look.x;
                    _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, LeftClamp, RightClamp);
				}
				else
					_cinemachineTargetYaw = 0;

                // clamp our pitch rotation
                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);

				// rotate the player left and right
				if (!InteractController.interactable)
					transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			if (InteractController.interactable)
				return;

			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = MoveSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

			if (grounded)
			{
                soundTimer -= Time.deltaTime * _controller.velocity.magnitude;

                if (soundTimer < 0)
                {
                    footstepSounds.PlaySound();
                    soundTimer = soundDelay;
                }
            }
			
        }

		private void JumpAndGravity()
		{
			if (grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				_verticalVelocity = 0f;

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
                // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
                if (_verticalVelocity > _terminalVelocity)
                {
                    _verticalVelocity += Gravity * Time.deltaTime;
                }

                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void CheckPause()
		{
            if (_input.pause)
			{
                _input.pause = false;
                _input.jump = false;
                _input.shrink = false;
                _input.grow = false;
                _input.action = false;
                _input.reset = false;
                if (square.enabled || mainMenu.transform.GetChild(0).gameObject.activeSelf)
					return;

                pauseMenu.PauseOrBack();
            }
        }

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}
	}
}


/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace EtAlii.FracturedPlanet
{
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.AI;

    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonBotController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float moveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float sprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float rotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float speedChangeRate = 10.0f;

        public AudioClip landingAudioClip;
        public AudioClip[] footstepAudioClips;
        [Range(0, 1)] public float footstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float jumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float jumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float fallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool grounded = true;

        [Tooltip("Useful for rough ground")]
        public float groundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float groundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask groundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject cinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float topClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float bottomClamp = -30.0f;

        [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        public float cameraAngleOverride;

        [Tooltip("For locking the camera position on all axis")]
        public bool lockCameraPosition;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

        private Animator _animator;
        private CharacterController _controller;

        private const float Threshold = 0.01f;

        [SerializeField] private NavMeshAgent navMeshAgent;

        private Vector2 _inputMove = Vector2.zero; // _input.move
        private bool _inputJump = false; // _input.Jump
        private bool _inputSprint = false; // _input.Sprint
        private bool _inputIsCurrentDeviceMouse = false;
        private bool _inputAnalogMovement = false;
        private float _inputMoveMagnitude = 1f;
        private Vector2 _inputLook = Vector2.zero;

        private void Start()
        {
            _cinemachineTargetYaw = cinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            //navMeshAgent = GetComponent<NavMeshAgent>();
            // navMeshAgent.updatePosition = false;
            // navMeshAgent.updateRotation = false;
            // navMeshAgent.updateUpAxis = false;

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = jumpTimeout;
            _fallTimeoutDelta = fallTimeout;
        }

        private void Update()
        {
            //_hasAnimator = TryGetComponent(out _animator);

            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && !navMeshAgent.pathPending)
            {
                //var nextPosition = navMeshAgent.nextPosition;
                var move = navMeshAgent.nextPosition - transform.position;
                //var move = navMeshAgent.steeringTarget - transform.position;
                _inputMove = new Vector2(move.x, move.z).normalized;
                _inputMoveMagnitude = navMeshAgent.speed;
                //_inputLook = new Vector2(navMeshAgent.pathEndPosition.)
            }
            else
            {
                _inputMoveMagnitude = 0f;
            }

            JumpAndGravity();
            GroundedCheck();
            Move();

            // if (_controller.velocity != Vector3.zero)
            // {
            //     navMeshAgent.velocity = _controller.velocity;
            // }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            var position = transform.position;
            var spherePosition = new Vector3(position.x, position.y - groundedOffset, position.z);
            grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);

            // update animator if using character
            _animator.SetBool(_animIDGrounded, grounded);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_inputLook.sqrMagnitude >= Threshold && !lockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                var deltaTimeMultiplier = _inputIsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _inputLook.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _inputLook.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

            // Cinemachine will follow this target
            cinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            var targetSpeed = _inputSprint ? sprintSpeed : moveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_inputMove == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            var velocity = _controller.velocity;
            var currentHorizontalSpeed = new Vector3(velocity.x, 0.0f, velocity.z).magnitude;

            var speedOffset = 0.1f;
            var inputMagnitude = _inputAnalogMovement ? _inputMoveMagnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * speedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            var inputDirection = new Vector3(_inputMove.x, 0.0f, _inputMove.y).normalized;

            //var navMeshAgentDirection = (navMeshAgent.nextPosition - transform.position).normalized;
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_inputMove != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + 0f;//MainCamera.transform.eulerAngles.y;
                var rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            // var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;


            // move the player
            _controller.Move(inputDirection * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
            //                   new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }

        private void JumpAndGravity()
        {
            if (grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = fallTimeout;

                // update animator if using character
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_inputJump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                    // update animator if using character
                    _animator.SetBool(_animIDJump, true);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = jumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    _animator.SetBool(_animIDFreeFall, true);
                }

                // if we are not grounded, do not jump
                _inputJump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = grounded ? transparentGreen : transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            var position = transform.position;
            Gizmos.DrawSphere(
                new Vector3(position.x, position.y - groundedOffset, position.z),
                groundedRadius);
        }

        [UsedImplicitly]
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!(animationEvent.animatorClipInfo.weight > 0.5f) || footstepAudioClips.Length <= 0)
            {
                return;
            }

            var index = Random.Range(0, footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_controller.center), footstepAudioVolume);
        }

        [UsedImplicitly]
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_controller.center), footstepAudioVolume);
            }
        }
    }
}

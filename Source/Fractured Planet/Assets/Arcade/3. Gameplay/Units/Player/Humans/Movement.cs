// ReSharper disable All

namespace Complete
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Movement : MonoBehaviour
    {
        public int playerNumber = 1;               // Used to identify which tank belongs to which player.  This is set by this tank's manager.
        public float speed = 12f;                  // How fast the tank moves forward and back.
        public float turnSpeed = 180f;             // How fast the tank turns in degrees per second.
        public AudioSource movementAudio;          // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.
        public AudioClip engineIdling;             // Audio to play when the tank isn't moving.
        public AudioClip engineDriving;            // Audio to play when the tank is moving.
		public float pitchRange = 0.2f;            // The amount by which the pitch of the engine noises can vary.

        private Rigidbody _rigidbody;              // Reference used to move the tank.
        private float _originalPitch;              // The pitch of the audio source at the start of the scene.
        private ParticleSystem[] _particleSystems; // References to all the particles systems used by the Tanks
        private DefaultInputActions _inputActions;

        private void Awake ()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _inputActions = new DefaultInputActions();
            _inputActions.Player.Enable();
        }


        private void OnEnable ()
        {
            // When the tank is turned on, make sure it's not kinematic.
            _rigidbody.isKinematic = false;

            // We grab all the Particle systems child of that Tank to be able to Stop/Play them on Deactivate/Activate
            // It is needed because we move the Tank when spawning it, and if the Particle System is playing while we do that
            // it "think" it move from (0,0,0) to the spawn point, creating a huge trail of smoke
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < _particleSystems.Length; ++i)
            {
                _particleSystems[i].Play();
            }
        }


        private void OnDisable ()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
            _rigidbody.isKinematic = true;

            // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
            for(int i = 0; i < _particleSystems.Length; ++i)
            {
                _particleSystems[i].Stop();
            }
        }


        private void Start ()
        {
            // Store the original pitch of the audio source.
            _originalPitch = movementAudio.pitch;
        }

        private void Update ()
        {
            EngineAudio ();
        }

        private void EngineAudio ()
        {
            var control = _inputActions.Player.Move.ReadValue<Vector2>();

            // If there is no input (the tank is stationary)...
            if (Mathf.Abs (control.y) < 0.1f && Mathf.Abs (control.x) < 0.1f)
            {
                // ... and if the audio source is currently playing the driving clip...
                if (movementAudio.clip == engineDriving)
                {
                    // ... change the clip to idling and play it.
                    movementAudio.clip = engineIdling;
                    movementAudio.pitch = Random.Range (_originalPitch - pitchRange, _originalPitch + pitchRange);
                    movementAudio.Play ();
                }
            }
            else
            {
                // Otherwise if the tank is moving and if the idling clip is currently playing...
                if (movementAudio.clip == engineIdling)
                {
                    // ... change the clip to driving and play.
                    movementAudio.clip = engineDriving;
                    movementAudio.pitch = Random.Range(_originalPitch - pitchRange, _originalPitch + pitchRange);
                    movementAudio.Play();
                }
            }
        }


        private void FixedUpdate ()
        {
            // Adjust the rigidbodies position and orientation in FixedUpdate.
            var control = _inputActions.Player.Move.ReadValue<Vector2>();
            Move (control);
            Turn (control);
        }


        private void Move (Vector2 control)
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = transform.forward * control.y * speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            _rigidbody.MovePosition(_rigidbody.position + movement);
        }


        private void Turn (Vector2 control)
        {
            // Determine the number of degrees to be turned based on the input, speed and time between frames.
            float turn = control.x * turnSpeed * Time.deltaTime;

            // Make this into a rotation in the y axis.
            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);

            // Apply this rotation to the rigidbody's rotation.
            _rigidbody.MoveRotation (_rigidbody.rotation * turnRotation);
        }
    }
}

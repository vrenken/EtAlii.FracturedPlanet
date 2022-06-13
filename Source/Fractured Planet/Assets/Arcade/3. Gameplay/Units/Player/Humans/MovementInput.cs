using UnityEngine;
using UnityEngine.InputSystem;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the input magnitude and allow blending between animations.

// ReSharper disable once CheckNamespace
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{

    public float Velocity;
    [Space]

	// public float InputX;
	// public float InputZ;
	// public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.025f;
	public Animator anim;
	public float Speed;
	public float allowPlayerRotation = 0.1f;
	public CharacterController controller;
	public bool isGrounded;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0,1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    // public float verticalVel;
    // private Vector3 moveVector;

    private DefaultInputActions _inputActions;
    private static readonly int _blend = Animator.StringToHash("Blend");

    public void Awake()
    {
        _inputActions = new DefaultInputActions();
        _inputActions.Player.Enable();
    }

    // Use this for initialization
    private void Start () {
		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController> ();

	}

	// Update is called once per frame
    private void Update ()
    {
		InputMagnitude ();

        isGrounded = controller.isGrounded;

        var input = _inputActions.Player.Move.ReadValue<Vector2>();

        var t = transform;
        var forward = t.forward;
        var right = t.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize ();
        right.Normalize ();

        var movement = forward * input.y + right * input.x;

        // if (blockRotationPlayer == false)
        // {
             transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (movement), desiredRotationSpeed);
        //     controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
        // }


        controller.SimpleMove(movement.normalized * Speed * 10f);

        // moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
        // controller.Move(moveVector);
    }

 //    private void PlayerMoveAndRotation()
 //    {
 //        var movement = _inputActions.Player.Move.ReadValue<Vector2>();
	// 	InputX = movement.x;
	// 	InputZ = movement.y;
 //
 //        var t = transform;
 //        var forward = t.forward;
	// 	var right = t.right;
 //
	// 	forward.y = 0f;
	// 	right.y = 0f;
 //
	// 	forward.Normalize ();
	// 	right.Normalize ();
 //
 //        desiredMoveDirection = forward * InputZ + right * InputX;
 //
	// 	if (blockRotationPlayer == false)
 //        {
	// 		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (desiredMoveDirection), desiredRotationSpeed);
 //            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
	// 	}
	// }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    // public void RotateToCamera(Transform t)
    // {
    //
    //     // var camera = Camera.main;
    //     // var forward = cam.transform.forward;
    //     // var right = cam.transform.right;
    //
    //     desiredMoveDirection = forward;
    //
    //     t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    // }

    private void InputMagnitude()
    {
		//Calculate Input Vectors

        var input = _inputActions.Player.Move.ReadValue<Vector2>().normalized;
        //InputX = movement.x;
        //InputZ = movement.y;

		//anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		//anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		//Calculate the Input Magnitude
		Speed = new Vector2(input.x, input.y).sqrMagnitude;

        //Physically move player

		if (Speed > allowPlayerRotation)
        {
			anim.SetFloat (_blend, Speed, StartAnimTime, Time.deltaTime);
			// PlayerMoveAndRotation();
		} else if (Speed < allowPlayerRotation)
        {
			anim.SetFloat (_blend, Speed, StopAnimTime, Time.deltaTime);
		}
	}
}

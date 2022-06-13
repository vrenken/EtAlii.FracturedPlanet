using UnityEngine;
using UnityEngine.AI;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.

// ReSharper disable once CheckNamespace
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class BotMovement : MonoBehaviour
{

    public float Velocity;
    [Space]

	public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float Speed;
	public float allowPlayerRotation = 0.1f;
	public CharacterController controller;
    public NavMeshAgent navMeshAgent;
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

    private static readonly int _blend = Animator.StringToHash("Blend");

    // public float verticalVel;
    // private Vector3 moveVector;

    // Use this for initialization
    private void Start () {
		anim = GetComponent<Animator> ();
		controller = GetComponent<CharacterController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

	// Update is called once per frame
    private void Update ()
    {
		InputMagnitude ();

        isGrounded = controller.isGrounded;
        // if (isGrounded)
        // {
        //     verticalVel -= 0;
        // }
        // else
        // {
        //     verticalVel -= 1;
        // }
        //moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
        //controller.Move(moveVector);
        controller.SimpleMove(navMeshAgent.velocity.normalized * (Speed * 10f));
    }

 //    private void PlayerMoveAndRotation()
 //    {
 //        var n = navMeshAgent.velocity.normalized;
 //        desiredMoveDirection = new Vector3(n.x, 0f, n.z);
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

    private void InputMagnitude() {
		//Calculate Input Vectors

        var n = navMeshAgent.velocity.normalized;
        desiredMoveDirection = new Vector3(n.x, 0f, n.z);

		//anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		//anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		//Calculate the Input Magnitude
		Speed = new Vector2(desiredMoveDirection.x, desiredMoveDirection.z).sqrMagnitude;

        //Physically move player

		if (Speed > allowPlayerRotation)
        {
			anim.SetFloat (_blend, Speed, StartAnimTime, Time.deltaTime);
			// PlayerMoveAndRotation();
		}
        else if (Speed < allowPlayerRotation)
        {
			anim.SetFloat (_blend, Speed, StopAnimTime, Time.deltaTime);
		}
	}
}

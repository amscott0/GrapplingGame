using UnityEngine;

// public class BasicRigidBodyPush : MonoBehaviour
// {
// 	public LayerMask pushLayers;
// 	public bool canPush;
// 	private CharacterController _controller;


// 	[Range(0.5f, 5f)] public float strength = 1.1f;

// 	private void Start(){
// 		_controller = GetComponent<CharacterController>();
// 	}

// 	private void OnControllerColliderHit(ControllerColliderHit hit)
// 	{
// 		if (canPush) PushRigidBodies(hit);
// 	}

// 	private void PushRigidBodies(ControllerColliderHit hit)
// 	{
// 		// https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

// 		// make sure we hit a non null rigidbody
// 		Rigidbody body = hit.collider.attachedRigidbody;
// 		if (body == null || body.isKinematic) return;

// 		var bodyLayerMask = 1 << body.gameObject.layer;

// 		// make sure we only push desired layer(s)
		
// 		if ((bodyLayerMask & pushLayers.value) == 0) return;

// 		// We dont want to push objects below us
// 		if (hit.moveDirection.y < -0.3f) return;

// 		// Calculate push direction from move direction, horizontal motion only
// 		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

// 		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

// 		// Apply the push and take strength into account
// 		body.AddForce(pushDir * strength * currentHorizontalSpeed, ForceMode.Impulse);
// 	}
// }
public class BasicRigidBodyPush : MonoBehaviour
{
    // Add a private static instance variable to the class that will hold the single instance of the class
    private static BasicRigidBodyPush _instance;

    public LayerMask pushLayers;
    public bool canPush;
    private CharacterController _controller;


    [Range(0.5f, 5f)] public float strength = 1.1f;

    // Make the class's constructor private, so that it can only be instantiated from within the class itself
    private BasicRigidBodyPush()
    {
        // Do nothing
    }

    // Add a public static property to the class that provides access to the single instance of the class
    public static BasicRigidBodyPush Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BasicRigidBodyPush();
            }
            return _instance;
        }
    }

    // Make the Start() and OnControllerColliderHit() methods static, since they will be accessed through the class's static property
    public void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (canPush) PushRigidBodies(hit);
    }

    // Add a public static method that allows other classes to access the PushRigidBodies() method
    public void PushRigidBodies(ControllerColliderHit hit)
	{
		// https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

		// make sure we hit a non null rigidbody
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body == null || body.isKinematic) return;

		var bodyLayerMask = 1 << body.gameObject.layer;

		// make sure we only push desired layer(s)
		
		if ((bodyLayerMask & pushLayers.value) == 0) return;

		// We dont want to push objects below us
		if (hit.moveDirection.y < -0.3f) return;

		// Calculate push direction from move direction, horizontal motion only
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		// Apply the push and take strength into account
		body.AddForce(pushDir * strength * currentHorizontalSpeed, ForceMode.Impulse);
	}
}
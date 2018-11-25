using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public GameObject StatueBodyPrefab;
    public bool WalkingMode;
	public Transform camera;
    public float walkingSpeed = 10;
    public float jumpingSpeed = 10;
    public float groundDetectionDistance = 1f;
    private float rotation;
    private Rigidbody rb;
	public Vector3 rotationOffset;
	public float movementForce = 1;
	public float movementTorque = 1;
    private Body body;

	

	// Use this for initialization
	void Start ()
	{
	    rb = GetComponent<Rigidbody>();
	    rotation = transform.rotation.y;
	    body = GetComponentInChildren<Body>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    var vel = rb.velocity;
	    rb.velocity = new Vector3(0, vel.y, 0);

	    if (!camera)
	    {
	        Debug.Log("Player does not have reference to camera!");
	        return;
	    }


	    var cameraForward = Vector3.Scale(camera.forward, new Vector3(1, 0, 1)).normalized;
	    var cameraRight = Vector3.Scale(camera.right, new Vector3(1, 0, 1)).normalized;
		
		var movementVector = new Vector3();

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
	    {
	        //rb.AddForce(Vector3.forward * walkingSpeed);
	        //rb.velocity += cameraForward * walkingSpeed;
		    movementVector = cameraForward * walkingSpeed;

	    }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
	    {
	        movementVector += -cameraRight * walkingSpeed;
	    }
	    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
	    {
	        movementVector += cameraRight * walkingSpeed;
	    }
	    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
	    {
	        movementVector += -cameraForward * walkingSpeed;
	    }
	    if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
	    {
	        movementVector += Vector3.up * jumpingSpeed;
	    }

		if (WalkingMode)
		{
			rb.velocity += movementVector;
			var cameraRot = camera.transform.eulerAngles;
			var newRot = new Vector3(0, cameraRot.y, 0);
			transform.rotation = Quaternion.Euler(newRot + rotationOffset);
		}
		else
		{
			rb.AddForce(movementVector * Time.fixedDeltaTime * movementForce);
			rb.AddTorque(movementVector * Time.fixedDeltaTime * movementTorque);
			var cameraRot = camera.transform.eulerAngles;
			var newRot = transform.eulerAngles;
			newRot.y = cameraRot.y;
			transform.rotation = Quaternion.Euler(newRot + rotationOffset);
		}
		
    }

    public void SetCamera(Transform mainCamera)
    {
        camera = mainCamera;
    }

    bool IsOnGround()
    {
        var hitMask = ~LayerMask.NameToLayer("Player"); // Ignore player
        Debug.DrawRay(transform.position, Vector3.down * groundDetectionDistance, Color.green, 2f);
        return Physics.Raycast(transform.position, Vector3.down, groundDetectionDistance, hitMask);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Body body = GetComponentInChildren<Body>();
        if (body)
        {
            body.HandleCollision(collision);
        }
    }

    public void SetScale(float scale)
    {
        if (body)
            body.SetScale(scale);
    }

}

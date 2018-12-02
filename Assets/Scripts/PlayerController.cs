using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    public AudioClip attachBodyPartClip;
    public AudioClip[] jumpAudioClips;

    //public GameObject StatueBodyPrefab;
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

    private Vector3 feetColliderOffset;
    private BoxCollider feetCollider;

    private AudioSource audioSource;
    private float height = 0;


    // Use this for initialization
    void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
	    rotation = transform.rotation.y;
	    body = GetComponentInChildren<Body>();
	    feetCollider = GetComponent<BoxCollider>();
	    if (feetCollider)
	        feetColliderOffset = feetCollider.center;
        SetColliderHeight(0);
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

	    SetMovementAnimationSpeed(0);
        EnableMovementAnimation(true);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
	    {
	        //rb.AddForce(Vector3.forward * walkingSpeed);
	        //rb.velocity += cameraForward * walkingSpeed;
		    movementVector = cameraForward;
            SetMovementAnimationSpeed(1);
	    }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
	    {
	        movementVector += -cameraRight;
	        SetMovementAnimationSpeed(1);
        }
	    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
	    {
	        movementVector += cameraRight;
	        SetMovementAnimationSpeed(1);
        }
	    if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
	    {
	        movementVector += -cameraForward;
	        SetMovementAnimationSpeed(1);
        }
	    

		if (WalkingMode)
		{
            //rb.velocity += movementVector;
		    if (movementVector != Vector3.zero)
		    {
		        movementVector = movementVector * (1 / movementVector.magnitude) * walkingSpeed;

		    }
		    rb.AddForce(movementVector * Time.fixedDeltaTime * movementForce);
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

	    if (Input.GetKeyDown(KeyCode.Space) && IsOnGround())
	    {
	        int index = Random.Range(0, jumpAudioClips.Length);
	        audioSource.clip = jumpAudioClips[index];
	        audioSource.Play();
	        Vector3 velocity = rb.velocity;
	        velocity.y = jumpingSpeed;
	        rb.velocity = velocity;
	        //movementVector += Vector3.up * jumpingSpeed;
	    }

    }

    public void SetMovementAnimationSpeed(float value)
    {
        if (body)
        {
            body.SetMovementAnimationSpeed(value);
        }
    }

    public void EnableMovementAnimation(bool value)
    {
        if (body)
        {
            body.EnableMovementAnimation(value);
        }
    }

    public void SetCamera(Transform mainCamera)
    {
        camera = mainCamera;
    }

    bool IsOnGround()
    {
        var hitMask = ~((1 << LayerMask.NameToLayer("Player")) | (1 <<LayerMask.NameToLayer("BodyPart"))) ; // Ignore player
        var dist = groundDetectionDistance + height;
        Debug.DrawRay(transform.position, Vector3.down * dist, Color.green, 2f);
        return Physics.Raycast(transform.position, Vector3.down, dist, hitMask);
    }

    private void OnCollisionEnter(Collision collision)
    {
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

    public void OnBodyPartAttach()
    {
        audioSource.clip = attachBodyPartClip;
        audioSource.Play();
        SetColliderHeight(body.GetMaxLegHeight());
    }

    public void SetColliderHeight(float newHeight)
    {
        height = newHeight;

        // Adjust the collider beneath the player to fit given height
        if (!feetCollider) return;
        float prevHeight = feetCollider.size.y;
        var newSize = feetCollider.size;
        newSize.y = height;
        feetCollider.size = newSize;
        Vector3 newCenter = feetCollider.center;
        newCenter.y = -height / 2 + feetColliderOffset.y;
        feetCollider.center = newCenter;

        // Adjust player position to avoid clipping the collider inside ground
        float heightDifference = height - prevHeight;
        Vector3 newPos = transform.localPosition;
        newPos.y += heightDifference;
        transform.localPosition = newPos;
        Debug.Log("Adjusted player height to " + height.ToString());
    }

    public int GetNumberOfConnectedLimbs()
    {
        if (!body) return -1;
        return body.GetNumberOfConnectedLimbs();
    }

}

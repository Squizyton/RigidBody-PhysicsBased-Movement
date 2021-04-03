using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//This Script is based off of a Tutorial by DanisTutorials
/// <summary>
/// https://www.youtube.com/watch?v=XAC8U9-dTZU
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    //Assignables
    public Transform playerCam;
    public Transform orientation;

    //Other
    private Rigidbody rb;

    //Rotation and Look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;
    
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    
    
    //Crouch and Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 500;
    public float slideCounterMovement = 0.2f;


    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;


    //Input
    private float x, y;
    private bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

    //Grab rigid body on awake
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    //After awake(grabbing rigidbody)
    private void Start()
    {
        //Grab player's current scale
        playerScale = transform.localScale;

        //Lock cursor in middle and turn it invisible;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        Look();
    }

    /// <summary>
    /// Find user input, should go in it's own script (later implemented)
    /// </summary>
    private void MyInput()
    {
        //Get X movement. 0 = not moving 1 = moving
        x = Input.GetAxisRaw("Horizontal");
        //Get Y movement. 0 = not moving 1 = moving
        y = Input.GetAxisRaw("Vertical");

        //Get Jump Button;
        jumping = Input.GetButton("Jump");

        crouching = Input.GetKey(KeyCode.LeftControl);
        
        
        //Crouching 
        if(Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if(Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        //Set localScale to crouchScale
        transform.localScale = crouchScale;
        //Push player down when crouching
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f,transform.position.z);
        
        
        //Sliding
        //If player's velocity is greater then .5, start sliding
        if (rb.velocity.magnitude > 0.5f)
        {
            //If the player is on ground and not in air
            if (grounded)
            {
                //Add force to the slide
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch()
    {
        //Set player back to normal scale
        transform.localScale = playerScale;
        //Push player back up
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f,transform.position.z);
    }

    private void Movement()
    {
        //Extra gravity to make sure player isn't flying
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        
        //Find the actual velocity relative to where player is lookinhg
        Vector2 mag = FindVelRelativeToLook();
        
        float xMag = mag.x, yMag = mag.y;
        
        
        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);
        
        
        //if Holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();
        
        //Set the max speed a player can go
        float maxSpeed = this.maxSpeed;
        
        //if sliding down a ramp, add force down so the player stays on the ground but can also build up speed;
        if (crouching && grounded && readyToJump)
        {
            //ADd a bunch of force downwards
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        
        //if speed is larger then maxspeed, cancel out the input so you don't go over max speed
        //If x > 0, and the magnitude of x is greater then the max speed, then set x to 0 so player can't surpass maxSpeed
        if (x > 0 && xMag > maxSpeed) x = 0;
        //If x < 0, which is negative speed and is LESS then the max speed, then set x to 0 so player can't go backward in time; 
        if (x < 0 && xMag < -maxSpeed) x = 0;
        
        //Same as x but, replace x with Y. This is based on jump velocity.
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;
        

        //Multipliers to slow down movement/increase movement control
        float multiplier = 1f, multiplierV = 1f;

        //Movement in Air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        
        //Movement while sliding
        if (grounded && crouching) multiplierV = 0f;
        
        
        //Apply all the forces generated to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }


    private void Jump()
    {
        //If player is ground and ready to jump
        if (grounded && readyToJump)
        {
            readyToJump = false;
            
            //Add the jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);
            
            //If Jumping while falling, reset y velocity
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
            {
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            }else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            //Invoke ResetJump with the jumpCooldown
            Invoke(nameof(ResetJump),jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;

    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        
        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we don't over or under rotate
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90);
        
        //Perform the rotation of camera
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0,desiredX,0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;
        
        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }
        
        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) ||
            (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }  if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        //Limit diagonal running
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }
    
    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement an dlmiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;


        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 98 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }


    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;
        
        //Iterate through every collision in a physics update

        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            
            //Floor
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }
        
        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }
}

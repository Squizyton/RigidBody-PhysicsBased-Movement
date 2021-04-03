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
        //Movement();
    }

    private void Update()
    {
        MyInput();
        //Lookup();
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
    
    

}
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

    //Crouch and Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    
    
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
        //MyInput();
        //Lookup();
    }

/// <summary>
/// Find user input, should go in it's own script (later implemented)
/// </summary>
    private void MyInput()
    {
    }
}

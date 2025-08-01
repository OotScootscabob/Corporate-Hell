using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class Movement : MonoBehaviour
{
    public static Movement Instance;

    [Header("Objects")]

    public Transform cam;
    private Transform look;
    [HideInInspector]
    public Transform head;
    private Transform groundCheck;
    private Rigidbody rb;

    [Header("Movement")]

    public float maxSpeed;
    private float speed;
    public float mouseSensitivity = 3.5f;

    [Space]

    [Tooltip("How much Movement Control in the Air: 0 = No Air Movement | 1 = Same as Ground")]
    [Range(0.0f, 1.0f)]
    public float airMovement = 0.6f;

    [Space]

    [Tooltip("Player Drag when grounded")]
    [Range(0.0f, 10.0f)]
    public float groundDrag = 4f;
    [Tooltip("Player Drag when not grounded")]
    [Range(0.0f, 10.0f)]
    public float airDrag = 3f;
    public float artificialGravityAmmount = 100f;

    [Header("Jumping")]

    public float jumpForce = 1300f;

    private bool readyToJump;

    public float maxNumOfJumpsInAir;

    private float numOfAirJumps;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    [Header("Slide")]

    public float slideForce = 400f;

    private bool slideDirCheck = true;

    private Vector3 slideDir;

    private Vector3 slideScale = new Vector3(1, 0.5f, 1);

    private Vector3 playerScale = new Vector3(1, 1, 1);

    private bool isSliding;

    [Header("Wallrunning")]

    public bool useWallrun = true;

    [Space]

    public LayerMask wallrunlayer;
    public float wallRunCheckRange = 1f;
    private Vector3 wallNormal;

    [Space]

    [Tooltip("The upwards force applied to the player while wallrunning")]
    public float wallRunUp = 12;
    [Tooltip("The jump force of the wallrun in the normal direction of the wall")]
    public long wallRunJump = 300;

    [Space]

    [Range(0.0f, 1.5f)]
    [Tooltip("The Jump multiplier relative to jumpforce")]
    public float wallRunJumpUpMulti = 0.6f;
    [Range(0.0f, 1.5f)]
    [Tooltip("The Movemnt speed multiplier relative to jumpforce")]
    public float wallRunMovementMulti = 0.7f;

    private bool wallRunCooldown = false;
    private float wallRunCooldownTimer;
    public float maxWallRunCooldownTimer;

    [Space]

    public bool blockDoubleWallrun = true;
    private GameObject lastWallRunObject;

    [Header("GroundCheck")]

    [Tooltip("Ground Detection Type: Spherecast is more accurate but uses more performance, Raycast uses less performance but is less accurate")]
    public GroundCheckType checkType;
    public enum GroundCheckType
    {
        Spherecast, Raycast
    }

    public bool grounded;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;
    private Vector3 groundNormal;
    private RaycastHit[] groundHits;

    //States
    private bool isWallrunning;

    //Inputs
    private float vertical;
    private float horizontal;
    private bool jump;
    private bool slide;

    //Camera
    CameraShakeInstance shake;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);

        rb = GetComponent<Rigidbody>();

        look = transform.GetChild(0);
        head = transform.GetChild(1);
        groundCheck = transform.GetChild(2);

        groundNormal = Vector3.zero;
        lastWallRunObject = gameObject;
        wallNormal = Vector3.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        readyToJump = true;

        groundHits = new RaycastHit[10];

        isWallrunning = false;
        wallRunCooldown = false;

        PhysicsMaterial mat = new PhysicsMaterial("tempMat");
        mat.bounceCombine = PhysicsMaterialCombine.Average;
        mat.bounciness = 0;
        mat.frictionCombine = PhysicsMaterialCombine.Minimum;
        mat.staticFriction = 0;
        mat.dynamicFriction = 0;
        gameObject.GetComponent<Collider>().material = mat;

        wallRunCooldownTimer = maxWallRunCooldownTimer;
    }

    private void Start()
    {
        maxSpeed *= 12.5f;//Multiply by 12.5 if ground drag is 10
        speed = maxSpeed;
        groundCheck.transform.localPosition = new Vector3(0f, -0.95f, 0f);
        setMaxJumps();
    }

    private void Update()
    {
        Look();

        //Input
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        jump = Input.GetKey(KeyCode.Space);
        slide = Input.GetKey(KeyCode.LeftControl);

        GroundCheck();
    }

    private void FixedUpdate()
    {
        //Physics
        //Adding Gravity
        if (!grounded && rb.linearVelocity.y <= -5f)
        {
            rb.AddForce(rb.transform.up * (-artificialGravityAmmount), ForceMode.Acceleration);
        }
        if (!grounded && rb.linearVelocity.y <= -35f)
        {
            if (shake == null)
            {
                shake = CameraShaker.Instance.StartShake(5f, 4f, 5f);
            }
        }
        else
        {
            ResetCameraShake();
        }
        if (shake != null && rb.linearVelocity.magnitude <= 30f)
        {
            ResetCameraShake();
        }

        rb.linearDamping = grounded ? groundDrag : airDrag;

        if (slide)
        {
            StartSlide();
        }
        if (readyToJump && jump && ((coyoteTimeCounter >= 0) || ((numOfAirJumps > 0) && (coyoteTimeCounter < 0) && !isWallrunning)))
        {
                Jump();
            if (isSliding)
            {
                ResetSlide();
            }
        }
        if (!readyToJump && !jump)
            ResetJump();

        if (grounded || isWallrunning)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        if(!slide)
            ResetSlide();

        if (useWallrun)
            CheckWallRun();

        if (isWallrunning && vertical == 1)
        {
            rb.AddForce(look.up * (wallRunUp * Time.fixedDeltaTime), ForceMode.Impulse);
        }

        if (vertical == 0 && horizontal == 0)
            return;

        float multi = 1f;

        if (!grounded)
            multi = airMovement;

        if (isWallrunning)
            multi = wallRunMovementMulti;

        if (wallRunCooldown)
        {
            wallRunCooldownTimer -= Time.fixedDeltaTime;
            if (wallRunCooldownTimer <= 0)
            {
                wallRunCooldown = false;
                wallRunCooldownTimer = maxWallRunCooldownTimer;
            }
        }
        if (groundNormal != Vector3.zero)
        {
            if (!slide)
            rb.AddForce(Vector3.Cross(look.right, groundNormal) * (vertical * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
            rb.AddForce(Vector3.Cross(look.forward, groundNormal) * (-horizontal * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(look.forward * (vertical * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
            rb.AddForce(look.right * (horizontal * speed * Time.fixedDeltaTime * multi), ForceMode.Impulse);
        }
    }

    //Camera Look
    private float xRotation = 0f;
    private float desiredX;
    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        desiredX = cam.localRotation.eulerAngles.y + mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        cam.localRotation = Quaternion.Euler(xRotation, desiredX, cam.localRotation.eulerAngles.z);
        look.localRotation = Quaternion.Euler(0, desiredX, 0f);
    }

    private void GroundCheck()
    {
        int c = 0;

        if (checkType == GroundCheckType.Spherecast)
        {
            c = Physics.SphereCastNonAlloc(groundCheck.position, groundCheckRadius, -transform.up, groundHits,
                groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }
        else if (checkType == GroundCheckType.Raycast)
        {
            c = Physics.RaycastNonAlloc(groundCheck.position, -transform.up, groundHits, groundCheckRadius,
                groundLayer, QueryTriggerInteraction.Ignore);
        }

        if (c > 0 && readyToJump)
        {
            grounded = true;
            lastWallRunObject = gameObject;
            groundNormal = groundHits[0].normal;
            setMaxJumps();

            ResetCameraShake();
        }
        else
        {
            grounded = false;
            groundNormal = Vector3.zero;
        }
    }

    private void Jump()
    {
        if (rb.linearVelocity.y < 0)
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (!isWallrunning && coyoteTimeCounter < 0)
        {
            numOfAirJumps--;
        }

        coyoteTimeCounter = 0f;

        if (isWallrunning)
        {
            //Jumping On Wall
            CameraController.Instance.StopWallrun();
            isWallrunning = false;
            wallRunCooldown = true;

            rb.AddForce(wallNormal * (wallRunJump * Time.fixedDeltaTime), ForceMode.Impulse);

            rb.AddForce(transform.up * (jumpForce * wallRunJumpUpMulti * Time.fixedDeltaTime), ForceMode.Impulse);
            ResetCameraShake() ;
        }
        else
        {
            if (groundNormal != Vector3.zero)
            {
                rb.AddForce(transform.up * jumpForce / 2 * Time.fixedDeltaTime, ForceMode.Impulse);
                rb.AddForce(groundNormal * jumpForce / 2 * Time.fixedDeltaTime, ForceMode.Impulse);
                ResetCameraShake();
            }
            else
            {
                rb.AddForce(transform.up * (jumpForce * Time.fixedDeltaTime), ForceMode.Impulse);
                ResetCameraShake();
            }
        }

        readyToJump = false;
        grounded = false;
        groundNormal = Vector3.zero;
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void CheckWallRun()
    {
        if (wallRunCooldown)
            return;

        if (grounded)
        {
            if (isWallrunning)
            {
                CameraController.Instance.StopWallrun();
                isWallrunning = false;
                wallRunCooldown = true;
            }

            return;
        }

        if (Physics.Raycast(transform.position, look.right, out RaycastHit righthit, wallRunCheckRange, wallrunlayer))
        {
            if (!isWallrunning && blockDoubleWallrun && righthit.transform.gameObject == lastWallRunObject)
                return;

            if (!isWallrunning)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallRunJumpUpMulti, rb.linearVelocity.z);

            lastWallRunObject = righthit.transform.gameObject;
            wallNormal = righthit.normal;
            CameraController.Instance.StartWallrun(true);
            isWallrunning = true;//Something bad here
            ResetCameraShake();
            setMaxJumps();
        }
        else if (Physics.Raycast(transform.position, -look.right, out RaycastHit lefthit, wallRunCheckRange, wallrunlayer))
        {
            if (!isWallrunning && blockDoubleWallrun && lefthit.transform.gameObject == lastWallRunObject)
                return;

            if (!isWallrunning)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, wallRunJumpUpMulti, rb.linearVelocity.z);

            lastWallRunObject = lefthit.transform.gameObject;
            wallNormal = lefthit.normal;
            CameraController.Instance.StartWallrun(false);
            isWallrunning = true;//Something bad here
            ResetCameraShake() ;
            setMaxJumps();
        }
        else if (isWallrunning)
        {
            //Stop Wallrunning
            CameraController.Instance.StopWallrun();
            isWallrunning = false;
            wallRunCooldown = true;
        }
    }
    private void setMaxJumps()
    {
        numOfAirJumps = maxNumOfJumpsInAir;
    }
    private void StartSlide()
    {
        if(grounded)
        {
            if (horizontal != 0 || vertical != 0)
            {
                if (slideDirCheck)
                {
                    slideDirCheck = false;
                    slideDir = rb.linearVelocity.normalized;

                }
                Slide();
            }
            else
            {
                if (slideDirCheck)
                {
                    slideDirCheck = false;
                    slideDir = look.transform.forward;

                }
                Slide();
            }
        }
        else
        {
            slideDirCheck = true;
        }
    }
    private void Slide()
    {
        isSliding = true;
        transform.localScale = slideScale;
        rb.AddForce(slideDir * slideForce);
    }
    private void ResetSlide()
    {
        isSliding = false;
        transform.localScale = playerScale;
        slideDirCheck = true;
    }
    private void ResetCameraShake()
    {
        if (shake != null)
        {
            shake.StartFadeOut(0);
            shake = null;
        }
    }
}
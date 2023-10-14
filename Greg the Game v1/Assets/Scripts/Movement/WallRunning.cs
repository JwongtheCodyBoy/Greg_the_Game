using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    public KeyCode wallJumpKey = KeyCode.Space;
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall = false;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Check for New Wall")]
    private Transform lastWall;
    private Vector3 lastWallNormal;
    private bool newWall;
    public float minWallNormalAngleChange;

    [Header("References")]
    public Transform orientation;
    public PlayerCam cam;
    private PlayerMovement pm;
    private Rigidbody rb;
    private LedgeGrabbing lg;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        lg = GetComponent<LedgeGrabbing>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);

        RaycastHit frontWallhit = wallRight? rightWallHit: leftWallHit;

        newWall = frontWallhit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallhit.normal)) > minWallNormalAngleChange;
        if (pm.grounded)
        {
            lastWall = null;
            lastWallNormal = lastWallNormal * 2f;
        }

    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        //State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall) //Next to wall, and moving foward, and Above ground
        {
            if (!pm.wallrunning) //if player not Wallrunning already Start Wallrun
                StartWallRun();

            if (Input.GetKeyDown(wallJumpKey)) WallJump();

            //wallrun timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;
            
            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
        }
        //State 2- Exiting
        else if (exitingWall)
        {
            if (pm.wallrunning)
                StopWallRun();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        //State 3 - None
        else
        {
            if (pm.wallrunning) //if player is Wallrunning, then stop wallrun
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        if (!newWall) return;
        pm.wallrunning = true;

        wallRunTimer = maxWallRunTime;

        //apply camera effects
        cam.DoFov(90f);
        if (wallLeft) cam.DoTilt(-5f);
        if (wallRight) cam.DoTilt(5f);


        lastWall = wallRight ? rightWallHit.transform : leftWallHit.transform;
        lastWallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
    }

    private void WallRunningMovement()
    {
        //if restricted, cannot wall climb
        if (pm.isDead)
        {
            StopWallRun();
            return;
        }

        if (pm.wallrunning)
        {
            rb.useGravity = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        }

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallFoward = Vector3.Cross(wallNormal, transform.up);

        //Which way should force be added
        if ((orientation.forward - wallFoward).magnitude > (orientation.forward - -wallFoward).magnitude)
            wallFoward = -wallFoward;

        //foward force
        rb.AddForce(wallFoward * wallRunForce, ForceMode.Force);

        //running upwards/downwards force
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        //push player to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rb.AddForce(-wallNormal * 100f, ForceMode.Force);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;

        cam.DoFov(60f);
        cam.DoTilt(0f);
    }

    private void WallJump()
    {
        if (lg.holding || lg.exitingLedge) return;

        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        //reset y velocity and add force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhysicsBasedCharacterController : MonoBehaviour
{
    private Rigidbody rb;
    private LayerMask terrainLayer;
    private float gravitationalForce;

    [Header("Other:")]
    [SerializeField] private bool _adjustInputsToCameraAngle = false;

    enum lookDirectionOptions{velocity, acceleration, moveInput};
    [SerializeField] private lookDirectionOptions _characterLookDirection = lookDirectionOptions.velocity;

    private Vector3 rayDir = Vector3.down;

    [Header("Height Spring:")]
    [SerializeField] private float rideHeight = 1.75f; // rideHeight: desired distance to ground (Note, this is distance from the original raycast position (currently centre of transform)). 
    [SerializeField] private float rayToGroundLength = 3f; // rayToGroundLength: max distance of raycast to ground (Note, this should be greater than the rideHeight).
    [SerializeField] public float rideSpringStrength = 50f; // rideSpringStrength: strength of spring. (?)
    [SerializeField] private float rideSpringDamper = 5f; // rideSpringDampener: dampener of spring. (?)
    private bool shouldMaintainHeight = true;
    private Vector3 maintainHeightForce;
    [SerializeField] private Oscillator _squashAndStretchOcillator;

    [Header("Upright Spring:")]
    [SerializeField] private float uprightSpringStrength = 40f;
    [SerializeField] private float uprightSpringDamper = 5f;
    private Quaternion uprightTargetRot = Quaternion.identity; // Adjust y value to match the desired direction to face.

    [Header("Movement:")]
    [SerializeField] private float MaxSpeed = 8f;
    [SerializeField] private float Acceleration = 200f;
    [SerializeField] private float MaxAccelForce = 150f;
    [SerializeField] private AnimationCurve AccelerationFactorFromDot;
    [SerializeField] private AnimationCurve MaxAccelerationForceFactorFromDot;
    [SerializeField] private Vector3 MoveForceScale = new Vector3(1f, 0f, 1f);
    private float speedFactor = 1f;
    private float maxAccelForceFactor = 1f;
    private Vector3 m_GoalVel = Vector3.zero;

    [Header("Jump:")]
    [SerializeField] private float jumpForceFactor = 10f;
    [SerializeField] private float fallGravityFactor = 10f; // typically > 1f (i.e. 5f). Only using fallGravityFactor or riseGravityFactor one at a time.
    [SerializeField] private float lowJumpFactor = 2.5f;
    [SerializeField] private float jumpBuffer = 0.15f; // Note, jumpBuffer shouldn't really exceed the time of the jump.
    [SerializeField] private float coyoteTime = 0.25f;
    private float timeSinceJumpPressed = 0f;
    private float timeSinceUngrounded = 0f;
    private float timeSinceJump = 0f;
    private bool jumpReady = true;
    private bool isJumping = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        terrainLayer = LayerMask.GetMask("Platform");
        gravitationalForce = Physics.gravity.y * rb.mass; // To counteract the excess gravitational force (which would make the actual heigh permanently lower than the desired rideHeight.

        MaintainUpright(new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z)); // Set characters to look at the camera.
    }

    private bool CheckIfGrounded(bool rayHitGround, RaycastHit rayHit)
    {
        bool grounded;
        if (rayHitGround == true)
        {
            grounded = rayHit.distance <= rideHeight * 1.1f; // 1.1f allows for greater leniancy (as the value will oscillate about the rideHeight).
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    private Vector3 GetLookDirection(lookDirectionOptions lookDirectionOption)
    {
        Vector3 lookDirection = Vector3.zero;
        if (lookDirectionOption == lookDirectionOptions.velocity || lookDirectionOption == lookDirectionOptions.acceleration)
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0f;
            if (lookDirectionOption == lookDirectionOptions.velocity)
            {
                lookDirection = velocity;
            }
                if (lookDirectionOption == lookDirectionOptions.acceleration)
            {
                Vector3 deltaVelocity = velocity - _previousVelocity;
                _previousVelocity = velocity;
                Vector3 acceleration = deltaVelocity / Time.fixedDeltaTime;
                lookDirection = acceleration;
            }
        }
        else if (lookDirectionOption == lookDirectionOptions.moveInput)
        {
            lookDirection = _moveInput;
        }
        return lookDirection;
    }

    private Vector3 _previousVelocity = Vector3.zero;

    private void FixedUpdate()
    {
        if (_adjustInputsToCameraAngle)
        {
            _moveInput = AdjustInputToCameraAngle(_moveInput);
        }

        (bool rayHitGround, RaycastHit rayHit) = RaycastToGround();

        bool grounded = CheckIfGrounded(rayHitGround, rayHit);

        if (grounded == true)
        {
            timeSinceUngrounded = 0f;

            if (timeSinceJump > 0.2f)
            {
                isJumping = false;
            }
        }
        else
        {
            timeSinceUngrounded += Time.fixedDeltaTime;
        }

        CharacterMove(_moveInput);
        CharacterJump(_jumpInput, grounded, rayHit);

        if (rayHitGround && shouldMaintainHeight)
        {
            MaintainHeight(rayHit);
        }          

        Vector3 lookDirection = GetLookDirection(_characterLookDirection);

        MaintainUpright(lookDirection);
    }


    // Cast raycast to get distance to the ground.
    private (bool, RaycastHit) RaycastToGround()
    {
        RaycastHit rayHit;
        Ray rayToGround = new Ray(transform.position, rayDir);
        bool rayHitGround = Physics.Raycast(rayToGround, out rayHit, rayToGroundLength, terrainLayer.value);
        Debug.DrawRay(transform.position, rayDir * rayToGroundLength, Color.blue);
        return (rayHitGround, rayHit);
    }

    private void MaintainHeight(RaycastHit rayHit)  // hmmm. it's just an oscillator...
    {
        Vector3 vel = rb.velocity;

        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = rayHit.rigidbody;
        if (hitBody != null)
        {
            otherVel = hitBody.velocity;
        }

        float rayDirVel = Vector3.Dot(rayDir, vel);
        float otherDirVel = Vector3.Dot(rayDir, otherVel);

        float relVel = rayDirVel - otherDirVel;

        float currHeight = rayHit.distance - rideHeight;

        float springForce = (currHeight * rideSpringStrength) - (relVel * rideSpringDamper);

        Debug.DrawLine(transform.position, transform.position + (rayDir * springForce), Color.yellow);

        maintainHeightForce = Vector3.down * (springForce + gravitationalForce);
        rb.AddForce(maintainHeightForce);

        // Squash and Stretch stuff.
        Vector3 oscillatorForce = springForce * Vector3.down;
        _squashAndStretchOcillator.ApplyForce(oscillatorForce);

        // Apply force to objects beneath
        if (hitBody != null)
        {
            hitBody.AddForceAtPosition(rayDir * -springForce, rayHit.point);
        }
    }




    private void MaintainUpright(Vector3 yLookAt)
    {
        if (yLookAt != Vector3.zero)
        {
            uprightTargetRot = Quaternion.LookRotation(yLookAt, Vector3.up);
        }

        Quaternion charCurr = transform.rotation; 

        Quaternion toGoal = MathsUtils.ShortestRotation(uprightTargetRot, charCurr);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        rb.AddTorque((rotAxis * (rotRadians * uprightSpringStrength)) - (rb.angularVelocity * uprightSpringDamper));
    }

    private Vector3 _moveInput;

    public void MoveInputAction(InputAction.CallbackContext context)
    {
        Vector2 moveContext = context.ReadValue<Vector2>();
        _moveInput = new Vector3(moveContext.x, 0, moveContext.y); 
    }

    private Vector3 _jumpInput;

    public void JumpInputAction(InputAction.CallbackContext context)
    {
        float jumpContext = context.ReadValue<float>();
        _jumpInput = new Vector3(0, jumpContext, 0);

        if (context.started) // button down
        {
            timeSinceJumpPressed = 0f;
        }
    }


    private Vector3 AdjustInputToCameraAngle(Vector3 moveInput) // Source: https://forum.unity.com/threads/move-character-relative-to-camera-angle.474375/
    {
        float facing = Camera.main.transform.eulerAngles.y; // Getting the angle the camera is facing
        return (Quaternion.Euler(0, facing, 0) * moveInput);
    }




    private void CharacterMove(Vector3 moveInput) 
    {
        Vector3 m_UnitGoal = moveInput;

        // Calculate new goal vel...
        Vector3 unitVel = m_GoalVel.normalized;

        float velDot = Vector3.Dot(m_UnitGoal, unitVel);

        float accel = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);

        Vector3 goalVel = m_UnitGoal * MaxSpeed * speedFactor;

        m_GoalVel = Vector3.MoveTowards(m_GoalVel,
                                        goalVel, //+ groundVel,
                                        accel * Time.fixedDeltaTime);

        // Actual force...
        Vector3 neededAccel = (m_GoalVel - rb.velocity) / Time.fixedDeltaTime;

        float maxAccel = MaxAccelForce * MaxAccelerationForceFactorFromDot.Evaluate(velDot) * maxAccelForceFactor;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        // Using AddForceAtPosition in order to both move the player and cause the play to lean in the direction of input.
        rb.AddForceAtPosition(Vector3.Scale(neededAccel * rb.mass, MoveForceScale), transform.position + new Vector3(0f, transform.localScale.y * +0.25f, 0f));
    }




    private void CharacterJump(Vector3 jumpInput, bool grounded, RaycastHit rayHit)
    {
        timeSinceJumpPressed += Time.fixedDeltaTime;
        timeSinceJump += Time.fixedDeltaTime;

        if (rb.velocity.y < 0)
        {
            shouldMaintainHeight = true;
            jumpReady = true;

            if (!grounded)
            {
                if (isJumping)
                {
                    // Increase downforce for a sudden plummet.
                    rb.AddForce(gravitationalForce * Vector3.up * (fallGravityFactor - 1f)); // Hmm... this feels a bit weird. I want a reactive jump, but I don't want it to dive all the time...
                }
            }

        }

        else if (rb.velocity.y > 0)
        {
            if (!grounded)
            {

                if (jumpInput == Vector3.zero)
                {
                    // Impede the jump height to achieve a low jump.
                    rb.AddForce(gravitationalForce * Vector3.up * (lowJumpFactor - 1f));
                }
            }

        }

        if (timeSinceJumpPressed < jumpBuffer)
        {
            if (timeSinceUngrounded < coyoteTime)
            {
                if (jumpReady)
                {
                    jumpReady = false;
                    shouldMaintainHeight = false;
                    isJumping = true;

                    // Cheat fix...
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                    if (rayHit.distance != 0) // i.e. if the ray has hit
                    {
                        rb.position = new Vector3(rb.position.x, rb.position.y - (rayHit.distance - rideHeight), rb.position.z);
                    }

                    rb.AddForce(Vector3.up * jumpForceFactor, ForceMode.Impulse); // This does not work very consistently... Jump height is affected by initial y velocity and y position relative to RideHeight... Want to adopt a fancier approach (more like PlayerMovement). A cheat fix to ensure consistency has been issued above...

                    timeSinceJumpPressed = jumpBuffer; // So as to not activate further jumps, in the case that the player lands before the jump timer surpasses the buffer.
                    timeSinceJump = 0f;
                }
            }
        }
    }
}

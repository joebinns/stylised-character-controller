using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A floating-capsule oriented physics based character controller. Based on the approach devised by Toyful Games for Very Very Valet.
/// </summary>
public class PhysicsBasedCharacterController : MonoBehaviour
{
    private Rigidbody _rb;
    private Vector3 _gravitationalForce;
    private Vector3 _rayDir = Vector3.down;
    private Vector3 _previousVelocity = Vector3.zero;

    [Header("Other:")]
    [SerializeField] private bool _adjustInputsToCameraAngle = false;
    [SerializeField] private LayerMask _terrainLayer;
    //[SerializeField] private RigidParent _rigidParent;


    private bool _shouldMaintainHeight = true;

    [Header("Height Spring:")]
    [SerializeField] private float _rideHeight = 1.75f; // rideHeight: desired distance to ground (Note, this is distance from the original raycast position (currently centre of transform)). 
    [SerializeField] private float _rayToGroundLength = 3f; // rayToGroundLength: max distance of raycast to ground (Note, this should be greater than the rideHeight).
    [SerializeField] public float _rideSpringStrength = 50f; // rideSpringStrength: strength of spring. (?)
    [SerializeField] private float _rideSpringDamper = 5f; // rideSpringDampener: dampener of spring. (?)
    [SerializeField] private Oscillator _squashAndStretchOcillator;


    private enum lookDirectionOptions { velocity, acceleration, moveInput };
    private Quaternion _uprightTargetRot = Quaternion.identity; // Adjust y value to match the desired direction to face.

    [Header("Upright Spring:")]
    [SerializeField] private lookDirectionOptions _characterLookDirection = lookDirectionOptions.velocity;
    [SerializeField] private float _uprightSpringStrength = 40f;
    [SerializeField] private float _uprightSpringDamper = 5f;



    [Header("Movement:")]
    [SerializeField] private float _maxSpeed = 8f;
    [SerializeField] private float _acceleration = 200f;
    [SerializeField] private float _maxAccelForce = 150f;
    [SerializeField] private AnimationCurve _accelerationFactorFromDot;
    [SerializeField] private AnimationCurve _maxAccelerationForceFactorFromDot;
    [SerializeField] private Vector3 _moveForceScale = new Vector3(1f, 0f, 1f);
    private float _speedFactor = 1f;
    private float _maxAccelForceFactor = 1f;
    private Vector3 _m_GoalVel = Vector3.zero;



    [Header("Jump:")]
    [SerializeField] private float _jumpForceFactor = 10f;
    [SerializeField] private float _fallGravityFactor = 10f; // typically > 1f (i.e. 5f). Only using fallGravityFactor or riseGravityFactor one at a time.
    [SerializeField] private float _lowJumpFactor = 2.5f;
    [SerializeField] private float _jumpBuffer = 0.15f; // Note, jumpBuffer shouldn't really exceed the time of the jump.
    [SerializeField] private float _coyoteTime = 0.25f;
    private float _timeSinceJumpPressed = 0f;
    private float _timeSinceUngrounded = 0f;
    private float _timeSinceJump = 0f;
    private bool _jumpReady = true;
    private bool _isJumping = false;


    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _gravitationalForce = Physics.gravity * _rb.mass; // To counteract the excess gravitational force (which would make the actual heigh permanently lower than the desired rideHeight.
        //_rigidParent = transform.parent.GetComponent<RigidParent>();

        MaintainUpright(new Vector3(Camera.main.transform.position.x, this.transform.position.y, Camera.main.transform.position.z)); // Set characters to look at the camera.
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rayHitGround"></param>
    /// <param name="rayHit"></param>
    /// <returns></returns>
    private bool CheckIfGrounded(bool rayHitGround, RaycastHit rayHit)
    {
        bool grounded;
        if (rayHitGround == true)
        {
            grounded = rayHit.distance <= _rideHeight * 1.1f; // 1.1f allows for greater leniancy (as the value will oscillate about the rideHeight).
        }
        else
        {
            grounded = false;
        }
        return grounded;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lookDirectionOption"></param>
    /// <returns></returns>
    private Vector3 GetLookDirection(lookDirectionOptions lookDirectionOption)
    {
        Vector3 lookDirection = Vector3.zero;
        if (lookDirectionOption == lookDirectionOptions.velocity || lookDirectionOption == lookDirectionOptions.acceleration)
        {
            Vector3 velocity = _rb.velocity;
            velocity.y = 0f;
            if (lookDirectionOption == lookDirectionOptions.velocity)
            {
                lookDirection = velocity;
            }
            else if (lookDirectionOption == lookDirectionOptions.acceleration)
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

    /// <summary>
    /// 
    /// </summary>
    private void FixedUpdate()
    {
        if (_adjustInputsToCameraAngle)
        {
            _moveInput = AdjustInputToCameraAngle(_moveInput);
        }

        (bool rayHitGround, RaycastHit rayHit) = RaycastToGround();

        SetPlatform(rayHit);

        bool grounded = CheckIfGrounded(rayHitGround, rayHit);

        if (grounded == true)
        {
            _timeSinceUngrounded = 0f;

            if (_timeSinceJump > 0.2f)
            {
                _isJumping = false;
            }
        }
        else
        {
            _timeSinceUngrounded += Time.fixedDeltaTime;
        }

        CharacterMove(_moveInput, rayHit);
        CharacterJump(_jumpInput, grounded, rayHit);

        if (rayHitGround && _shouldMaintainHeight)
        {
            MaintainHeight(rayHit);
        }          

        Vector3 lookDirection = GetLookDirection(_characterLookDirection);

        MaintainUpright(lookDirection);
    }


    // Cast raycast to get distance to the ground.
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private (bool, RaycastHit) RaycastToGround()
    {
        RaycastHit rayHit;
        Ray rayToGround = new Ray(transform.position, _rayDir);
        bool rayHitGround = Physics.Raycast(rayToGround, out rayHit, _rayToGroundLength, _terrainLayer.value);
        Debug.DrawRay(transform.position, _rayDir * _rayToGroundLength, Color.blue);
        return (rayHitGround, rayHit);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="rayHit"></param>
    private void MaintainHeight(RaycastHit rayHit)
    {
        Vector3 vel = _rb.velocity;
        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = rayHit.rigidbody;
        if (hitBody != null)
        {
            otherVel = hitBody.velocity;
        }
        float rayDirVel = Vector3.Dot(_rayDir, vel);
        float otherDirVel = Vector3.Dot(_rayDir, otherVel);

        float relVel = rayDirVel - otherDirVel;
        float currHeight = rayHit.distance - _rideHeight;
        float springForce = (currHeight * _rideSpringStrength) - (relVel * _rideSpringDamper);
        Vector3 maintainHeightForce = - _gravitationalForce + springForce * Vector3.down;
        Vector3 oscillationForce = springForce * Vector3.down;
        _rb.AddForce(maintainHeightForce);
        _squashAndStretchOcillator.ApplyForce(oscillationForce);
        Debug.DrawLine(transform.position, transform.position + (_rayDir * springForce), Color.yellow);

        // Apply force to objects beneath
        if (hitBody != null)
        {
            hitBody.AddForceAtPosition(_rayDir * -springForce, rayHit.point);
        }  
    }



    private void MaintainUpright(Vector3 yLookAt)
    {
        //if (Vector3.Magnitude(yLookAt) > 0.025f)
        if (yLookAt != Vector3.zero)
        {
            _uprightTargetRot = Quaternion.LookRotation(yLookAt, Vector3.up);
        }

        Quaternion currentRot = transform.rotation; 
        Quaternion toGoal = MathsUtils.ShortestRotation(_uprightTargetRot, currentRot);

        Vector3 rotAxis;
        float rotDegrees;

        toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;
        _rb.AddTorque((rotAxis * (rotRadians * _uprightSpringStrength)) - (_rb.angularVelocity * _uprightSpringDamper));
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
            _timeSinceJumpPressed = 0f;
        }
    }


    private Vector3 AdjustInputToCameraAngle(Vector3 moveInput) // Source: https://forum.unity.com/threads/move-character-relative-to-camera-angle.474375/
    {
        float facing = Camera.main.transform.eulerAngles.y; // Getting the angle the camera is facing
        return (Quaternion.Euler(0, facing, 0) * moveInput);
    }

    [SerializeField] private Transform platform;

    private void SetPlatform(RaycastHit rayHit) // NOT WORKING PROPERLY...!!
    {
        if (rayHit.transform != null)
        {
            platform = rayHit.transform.parent.GetComponentInChildren<RigidParent>().transform;
            if (platform != null)
            {
                this.transform.SetParent(platform);
            }

            else
            {
                this.transform.SetParent(null);
            }
        }

    }

    private void CharacterMove(Vector3 moveInput, RaycastHit rayHit) 
    {
        Vector3 m_UnitGoal = moveInput;
        Vector3 unitVel = _m_GoalVel.normalized;
        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
        float accel = _acceleration * _accelerationFactorFromDot.Evaluate(velDot);
        Vector3 goalVel = m_UnitGoal * _maxSpeed * _speedFactor;

        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = rayHit.rigidbody;
        if (hitBody != null)
        {
            //otherVel = hitBody.velocity;
        }

        _m_GoalVel = Vector3.MoveTowards(_m_GoalVel,
                                        goalVel + otherVel,
                                        accel * Time.fixedDeltaTime);

        // Actual force...
        Vector3 neededAccel = (_m_GoalVel - _rb.velocity) / Time.fixedDeltaTime;

        float maxAccel = _maxAccelForce * _maxAccelerationForceFactorFromDot.Evaluate(velDot) * _maxAccelForceFactor;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        // Using AddForceAtPosition in order to both move the player and cause the play to lean in the direction of input.
        
        _rb.AddForceAtPosition(Vector3.Scale(neededAccel * _rb.mass, _moveForceScale), transform.position + new Vector3(0f, transform.localScale.y * +0.25f, 0f));
    }




    private void CharacterJump(Vector3 jumpInput, bool grounded, RaycastHit rayHit)
    {
        _timeSinceJumpPressed += Time.fixedDeltaTime;
        _timeSinceJump += Time.fixedDeltaTime;

        if (_rb.velocity.y < 0)
        {
            _shouldMaintainHeight = true;
            _jumpReady = true;

            if (!grounded)
            {
                if (_isJumping)
                {
                    // Increase downforce for a sudden plummet.
                    _rb.AddForce(_gravitationalForce * (_fallGravityFactor - 1f)); // Hmm... this feels a bit weird. I want a reactive jump, but I don't want it to dive all the time...
                }
            }

        }

        else if (_rb.velocity.y > 0)
        {
            if (!grounded)
            {

                if (jumpInput == Vector3.zero)
                {
                    // Impede the jump height to achieve a low jump.
                    _rb.AddForce(_gravitationalForce * (_lowJumpFactor - 1f));
                }
            }

        }

        if (_timeSinceJumpPressed < _jumpBuffer)
        {
            if (_timeSinceUngrounded < _coyoteTime)
            {
                if (_jumpReady)
                {
                    _jumpReady = false;
                    _shouldMaintainHeight = false;
                    _isJumping = true;

                    // Cheat fix...
                    _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

                    if (rayHit.distance != 0) // i.e. if the ray has hit
                    {
                        _rb.position = new Vector3(_rb.position.x, _rb.position.y - (rayHit.distance - _rideHeight), _rb.position.z);
                    }

                    _rb.AddForce(Vector3.up * _jumpForceFactor, ForceMode.Impulse); // This does not work very consistently... Jump height is affected by initial y velocity and y position relative to RideHeight... Want to adopt a fancier approach (more like PlayerMovement). A cheat fix to ensure consistency has been issued above...

                    _timeSinceJumpPressed = _jumpBuffer; // So as to not activate further jumps, in the case that the player lands before the jump timer surpasses the buffer.
                    _timeSinceJump = 0f;
                }
            }
        }
    }
}

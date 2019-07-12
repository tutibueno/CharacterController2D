using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using System;

public class ControllerMetroid : MonoBehaviour
{
    // movement config
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;
    public float jumpForce = 0.2f;
    public float movingPlatformDump = 0.2f;


    private float normalizedHorizontalSpeed = 0;

    private CharacterController2D _controller;
    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;
    Vector3 movingPlatformVelocity;

    bool isAttacking;
    bool isFalling;
    bool jumpReleased;
    bool isJumping;
    float jumpForceCalc;
    bool wasJumping; //indicates the jump started some point and not finished yet, can prevent jump key forever pressed (infinite jumps after landing)
    bool landed; //want add some extra feature when landing?
    Vector3 velocityLastFrame;
    HealthController healthController;
    bool canControl;
    float invincibleTimer;
    bool isOnInvincebleTimer;
    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
    }

    private void Start()
    {
        canControl = true;
    }

    private void OnEnable()
    {
        healthController = GetComponent<HealthController>();
        healthController.OnHit += OnHit;
    }

    private void OnHit()
    {
        if (healthController.isInvincible)
            return;

        healthController.SetInvincible(true);
        canControl = false;
        _animator.SetTrigger("onHit");

    }

    public void SetHitFinished()
    {
        canControl = true;
        invincibleTimer = 3;
        isOnInvincebleTimer = true;

    }


    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (Mathf.Approximately(hit.normal.y, 1f))
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent(Collider2D col)
    {
        Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }


    void onTriggerExitEvent(Collider2D col)
    {
        Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion


    // the Update loop contains a very simple example of moving the character around and controlling the animation
    void Update()
    {


        if (_controller.isGrounded)
            _velocity.y = 0;

        if (isOnInvincebleTimer)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                healthController.SetInvincible(false);
                isOnInvincebleTimer = false;
                rend.material.color = Color.white;
            }
            else
            {
                rend.material.color = new Color(1, 1, 1, Time.frameCount % 2 == 0 ? 1 : 0);
            }
                
        }
            

        normalizedHorizontalSpeed = 0;
        bool grounded = CheckGrounded();

        if (canControl)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if ((grounded && !isAttacking) || !grounded)
                {
                    normalizedHorizontalSpeed = 1;
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if ((grounded && !isAttacking) || !grounded)
                {
                    normalizedHorizontalSpeed = -1;
                    transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }


            if (Input.GetKeyDown(KeyCode.UpArrow))
            {

                if (CheckGrounded())
                    isJumping = true;
            }

            if (Input.GetKey(KeyCode.UpArrow) && isJumping)
            {
                if (jumpForceCalc <= 0.0f)
                {
                    isJumping = false;
                }
                if (isJumping)
                {
                    jumpForceCalc -= Time.deltaTime;
                    _velocity.y = Mathf.Sqrt(3 * jumpHeight * -gravity);
                }
            }



            //Release character to jump again
            if (!Input.GetKey(KeyCode.UpArrow) && isJumping)
            {
                isJumping = false;
                jumpReleased = true;

            }

            if (Input.GetKeyDown(KeyCode.E))
            {

                if (!isAttacking)
                {
                    _animator.SetTrigger("attack");
                    isAttacking = true;
                }


            }
        }
                       

        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = normalizedHorizontalSpeed * runSpeed;//Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

        // apply gravity before moving
        if (!_controller.isGrounded)
            _velocity.y += gravity * Time.deltaTime;


        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (_controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                _velocity.y *= 3f;
                _controller.ignoreOneWayPlatformsThisFrame = true;
            }


            jumpForceCalc = jumpForce;
            jumpReleased = false;


        }

        movingPlatformVelocity = Vector3.zero;


        Vector3 v = _velocity * Time.deltaTime;

        if(_velocity.y <= 0)
        {
            movingPlatformVelocity = CheckMovingPlatforms(v);
        }


        _controller.move(v + movingPlatformVelocity);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;

        isFalling = _velocity.y < 0 && !CheckGrounded();

        if (!isFalling && velocityLastFrame.y < -1 && !landed)
        {
            landed = true;
            Debug.Log("OnLandaded");
        }
        else
            landed = false;

        _animator.SetFloat("velocityY", _velocity.y);

        _animator.SetBool("grounded", CheckGrounded());

        _animator.SetBool("falling", isFalling);

        _animator.SetBool("jumping", isJumping);

        _animator.SetFloat("speed", Mathf.Abs(normalizedHorizontalSpeed));

        velocityLastFrame = _controller.velocity;


    }

    bool CheckGrounded()
    {
        return _controller.collisionState.becameGroundedThisFrame || _controller.collisionState.wasGroundedLastFrame;
    }

    RaycastHit2D _raycastHit;
    Vector3 CheckMovingPlatforms(Vector3 deltaMovement)
    {
        var initialRayOrigin = _controller.raycastOrigins.bottomLeft;
        var rayDirection = Vector3.down;
        var rayDistance = _controller.skinWidth + Mathf.Abs(deltaMovement.y) + 0.3f;

        initialRayOrigin.x += deltaMovement.x;

        for (var i = 0; i < _controller.totalVerticalRays; i++)
        {
            var ray = new Vector2(initialRayOrigin.x + i * _controller.horizontalDistanceBetweenRays, initialRayOrigin.y);

            Debug.DrawRay(ray, rayDirection * rayDistance, Color.magenta);
            _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, _controller.platformMask);
            if (_raycastHit)
            {
                // set our new deltaMovement and recalculate the rayDistance taking it into account
                //deltaMovement.y = _raycastHit.point.y - ray.y;
                //rayDistance = Mathf.Abs(deltaMovement.y);

                var movingPlatform = _raycastHit.transform.GetComponent<Platform>();
                if (movingPlatform)
                {
                    movingPlatform.SetOnPlatform(Time.deltaTime);
                    return movingPlatform.Velocity;
                }


            }
        }
        return Vector3.zero;

    }

    public void SetAttackFinished()
    {
        isAttacking = false;
    }


}



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

    private CharacterController2D controller;
    private Animator animator;
    private RaycastHit2D lastControllerColliderHit;
    private Vector3 velocity;
    Vector3 movingPlatformVelocity;
    private bool isInDash;
    private float dashTime;

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
    bool isTakingHit;
    bool waitingToGrouded;
    float takingHitTimer;
    Vector2 hitSource;
    int hitDirection;
    SpriteRenderer spriteRenderer;
    List<SpriteRenderer> spritesCopy = new List<SpriteRenderer>();

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController2D>();

        // listen to some events for illustration purposes
        controller.onControllerCollidedEvent += onControllerCollider;
        controller.onTriggerEnterEvent += onTriggerEnterEvent;
        controller.onTriggerExitEvent += onTriggerExitEvent;

        PrepareSprites();


    }

    void PrepareSprites()
    {
        for (int i = 0; i < 6; i++)
        {
            var go = new GameObject("spriteCopy");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spriteRenderer.sprite;
            spritesCopy.Add(sr);
            sr.transform.position = transform.position;
            sr.transform.localScale = transform.localScale;
        }

        StartCoroutine(ControlSpritesCopy());

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

    private void OnHit(Vector2 _hitSource)
    {
        if (healthController.IsInvincible)
            return;

        healthController.SetInvincible(true);
        canControl = false;
        animator.SetTrigger("onHit");

        takingHitTimer = 0.5f;
        waitingToGrouded = true;
        hitSource = _hitSource;
        hitDirection = hitSource.x >= transform.position.x ? -1 : 1;


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


        if ((controller.isGrounded && !waitingToGrouded) || isInDash)
            velocity.y = 0;

        // apply gravity before moving
        if (!controller.isGrounded && !isInDash)
            velocity.y += gravity * Time.deltaTime;

        normalizedHorizontalSpeed = 0;


        if (healthController.IsDead)
            canControl = false;

        if (Input.GetKeyDown(KeyCode.H))
        {
            velocity.y =  -gravity  * 0.3f;
            waitingToGrouded = true;
            takingHitTimer = 0.5f;
        }

        //Control hit
        if(waitingToGrouded || takingHitTimer > 0)
        {
            if(takingHitTimer >= 0.5f)
                velocity.y = -gravity * 0.3f;

            takingHitTimer -= Time.deltaTime;
            //velocity.x = hitDirection * runSpeed;
            normalizedHorizontalSpeed = hitDirection;
            if (CheckGrounded() && takingHitTimer <= 0) {
                waitingToGrouded = false;
                if (!healthController.IsDead)
                {
                    canControl = true;
                    isOnInvincebleTimer = true;
                    invincibleTimer = 3;
                }
                else
                    animator.Play("Die");

            }

        }


        if (isOnInvincebleTimer)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer <= 0)
            {
                healthController.SetInvincible(false);
                isOnInvincebleTimer = false;
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = new Color(1, 1, 1, Time.frameCount % 2 == 0 ? 1 : 0);
            }

        }


        bool grounded = CheckGrounded();

        if (canControl && !isInDash)
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
                    velocity.y = Mathf.Sqrt(3 * jumpHeight * -gravity);
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
                    animator.SetTrigger("attack");
                    isAttacking = true;
                }


            }

            if (Input.GetKeyDown(KeyCode.Q) && !isAttacking && !CheckGrounded())
            {
                isInDash = true;
                dashTime = 0;
                animator.SetTrigger("startDash");
            }

        }

        float dashSpeed = runSpeed * 0.5f;

        //Control Dash
        if (isInDash)
        {
            dashTime += Time.deltaTime;
            if (dashTime >= 0.5f)
                isInDash = false;
            else
            {
                normalizedHorizontalSpeed = transform.rotation.eulerAngles.y > 0 ? -1 : 1;
            }
        }
        else
            dashSpeed = 1;



        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        velocity.x = normalizedHorizontalSpeed * runSpeed * dashSpeed;//Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);




        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                velocity.y *= 3f;
                controller.ignoreOneWayPlatformsThisFrame = true;
            }


            jumpForceCalc = jumpForce;
            jumpReleased = false;


        }

        movingPlatformVelocity = Vector3.zero;


        Vector3 v = velocity * Time.deltaTime;

        if(velocity.y <= 0)
        {
            movingPlatformVelocity = CheckMovingPlatforms(v);
        }


        controller.move(v + movingPlatformVelocity);

        // grab our current _velocity to use as a base for all calculations
        velocity = controller.velocity;

        isFalling = velocity.y < 0 && !CheckGrounded();

        if (!isFalling && velocityLastFrame.y < -1 && !landed)
        {
            landed = true;
            Debug.Log("OnLandaded");
        }
        else
            landed = false;

        animator.SetFloat("velocityY", velocity.y);

        animator.SetBool("grounded", CheckGrounded());

        animator.SetBool("falling", isFalling);

        animator.SetBool("jumping", isJumping);

        animator.SetFloat("speed", Mathf.Abs(normalizedHorizontalSpeed));

        animator.SetBool("canControl", canControl);

        animator.SetBool("isDash", isInDash);

        velocityLastFrame = controller.velocity;


    }

    bool CheckGrounded()
    {
        return controller.collisionState.becameGroundedThisFrame || controller.collisionState.wasGroundedLastFrame;
    }

    RaycastHit2D _raycastHit;
    Vector3 CheckMovingPlatforms(Vector3 deltaMovement)
    {
        var initialRayOrigin = controller.raycastOrigins.bottomLeft;
        var rayDirection = Vector3.down;
        var rayDistance = controller.skinWidth + Mathf.Abs(deltaMovement.y) + 0.3f;

        initialRayOrigin.x += deltaMovement.x;

        for (var i = 0; i < controller.totalVerticalRays; i++)
        {
            var ray = new Vector2(initialRayOrigin.x + i * controller.horizontalDistanceBetweenRays, initialRayOrigin.y);

            Debug.DrawRay(ray, rayDirection * rayDistance, Color.magenta);
            _raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, controller.platformMask);
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

    IEnumerator ControlSpritesCopy()
    {
        while (true)
        {
            foreach (var item in spritesCopy)
            {
                item.transform.position = transform.position;
                item.transform.rotation = transform.rotation;
                item.sprite = spriteRenderer.sprite;
                item.color = new Color(1, 1, 1, 0.4f);
                for (int i = 0; i < 5; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }
    }


}



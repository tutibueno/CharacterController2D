using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class ControllerMetroid : MonoBehaviour
{
    // movement config
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;
    public float jumpForce = 0.2f;


    private float normalizedHorizontalSpeed = 0;

    private CharacterController2D _controller;
    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    private Vector3 _velocity;

    bool isFalling;
    bool jumpReleased;
    bool isJumping;
    float jumpForceCalc;
    bool wasJumping; //indicates the jump started some point and not finished yet, can prevent jump key forever pressed (infinite jumps after landing)
    bool landed; //want add some extra feature when landing?
    Vector3 velocityLastFrame;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
    }


    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
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

        if( _controller.isGrounded )
          _velocity.y = 0;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            normalizedHorizontalSpeed = 1;
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
                _animator.Play(Animator.StringToHash("Run"));
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            normalizedHorizontalSpeed = -1;
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            if (_controller.isGrounded)
                _animator.Play(Animator.StringToHash("Run"));
        }
        else
        {
            normalizedHorizontalSpeed = 0;

            if (_controller.isGrounded)
                _animator.Play(Animator.StringToHash("Idle"));
        }



        if (Input.GetKeyDown(KeyCode.UpArrow))
        {

            if(CheckGrounded())
                isJumping = true;
        }

        if(Input.GetKey(KeyCode.UpArrow) && isJumping)
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



        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

        // apply gravity before moving
        if(!_controller.isGrounded)
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

        _controller.move(_velocity * Time.deltaTime);

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

        velocityLastFrame = _controller.velocity;


    }

    bool CheckGrounded()
    {
        return _controller.collisionState.becameGroundedThisFrame || _controller.collisionState.wasGroundedLastFrame;
    }

}



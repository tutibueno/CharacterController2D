using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using System;

[RequireComponent(typeof(HealthController))]
[RequireComponent(typeof(CharacterController2D))]
public class NPC : MonoBehaviour
{


    [SerializeField]
    float gravity = -10;

    [SerializeField]
    float speed = 2;

    [SerializeField]
    protected int direction = 1;

    [SerializeField]
    protected float distanceToAttack = 1;

    [SerializeField]
    protected float touchDamage = 10;

    [SerializeField]
    protected bool canAttack;

    [SerializeField]
    protected bool invertedSprite;

    protected GameObject player;

    protected Animator animator;
    protected CharacterController2D controller;
    Vector3 velocity;

    protected delegate void AiState();

    protected AiState currentState;

    protected float distanceFromPlayer;
    protected float currentSpeed;
    protected BoxCollider2D boxCollider;


    protected HealthController healthController;

    // Start is called before the first frame update
    public virtual void Start()
    {

        healthController = GetComponent<HealthController>();

        healthController.OnDie += OnDie;

        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController2D>();

        player = GameObject.FindGameObjectWithTag("Player");

        currentState = Patrol;

        boxCollider = GetComponent<BoxCollider2D>();

    }

    private void OnDisable()
    {
        healthController.OnDie -= OnDie;
    }

    private void OnDie()
    {
        Debug.Log("I am dead!");
        animator.SetTrigger("onDie");
        Invoke("StopAll", 3);

    }

    // Update is called once per frame
    public virtual void Update()
    {


        if (!controller.isGrounded)
            velocity.y = gravity;

        if (healthController.IsDead) 
            return;

        currentState();

        velocity.x = speed * direction;

        currentSpeed = speed * direction;
        
        controller.move(velocity * Time.deltaTime);

        animator.SetFloat("speed", Mathf.Abs(currentSpeed));

        distanceFromPlayer = player? Vector2.Distance(transform.position, player.transform.position) : 1000;



    }

    protected bool WillFall()
    {
        if (!controller.isGrounded)
            return false;

        var isGoingRight = velocity.x > 0;
        var rayDistance = controller.skinWidth * 2;
        var rayDirection = Vector3.down * 1.3f;
        var initialRayOrigin = isGoingRight ? controller.raycastOrigins.bottomRight : controller.raycastOrigins.bottomLeft;

        var ray = new Vector2(initialRayOrigin.x, initialRayOrigin.y);

        Debug.DrawRay(initialRayOrigin, rayDirection * rayDistance, Color.blue);

        var raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, controller.platformMask);

        if (raycastHit)
            return false;

        return true;


    }

    public virtual void Attack()
    {

        direction = 0;

        if(player)
            transform.rotation = Quaternion.Euler(0, player.transform.position.x >= transform.position.x ? 0 : 180, 0);

        if (distanceFromPlayer > distanceToAttack)
            currentState = Patrol;

        animator.SetBool("isAttacking", true);
    }

    public virtual void Patrol()
    {

        animator.SetBool("isAttacking", false);

        if (direction == 0 && controller.isGrounded)
        {
            direction = player.transform.position.x >= transform.position.x ? 1 : -1;
        }
            

        if (controller.collisionState.right)
            direction = -1;
        else
            if (controller.collisionState.left)
            direction = 1;

        if (!controller.isGrounded)
            direction = 0;

        if (WillFall())
            direction *= -1;

        if (distanceFromPlayer <= distanceToAttack && canAttack)
            currentState = Attack;

        //Adjust rotation
        if(!invertedSprite)
            transform.rotation = Quaternion.Euler(0, direction > 0 ? 0 : 180, 0);
        else
            transform.rotation = Quaternion.Euler(0, direction > 0 ? 180 : 0, 0);


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (healthController.IsDead)
            return;

        Debug.Log("Collided with: " + collision.transform.tag);
        if(collision.transform.tag.Equals("Player"))
        {
            var hc = collision.transform.GetComponent<HealthController>();
            hc.Hit(touchDamage, transform.position);
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (healthController.IsDead)
            return;

        Debug.Log("Collided with: " + collision.transform.tag);
        if (collision.transform.tag.Equals("Player"))
        {
            var hc = collision.transform.GetComponent<HealthController>();
            hc.Hit(touchDamage, transform.position);
        }

    }

    void StopAll()
    {
        animator.enabled = false;
        healthController.enabled = false;
        this.enabled = false;
        boxCollider.enabled = false;
    }

}

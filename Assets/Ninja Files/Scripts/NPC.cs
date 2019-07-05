using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

[RequireComponent(typeof(CharacterController2D))]
public class NPC : MonoBehaviour
{


    [SerializeField]
    float gravity = -10;

    [SerializeField]
    float speed = 2;

    [SerializeField]
    int direction = 1;

    Transform player;

    Animator animator;
    CharacterController2D controller;
    Vector3 velocity;

    delegate void State();

    State currentState;

    float distanceFromPlayer;
    float currentSpeed;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController2D>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        currentState = Patrol;
        
    }

    // Update is called once per frame
    void Update()
    {


        if (!controller.isGrounded)
            velocity.y = gravity;



        currentState();

        velocity.x = speed * direction;

        currentSpeed = speed * direction;
        
        controller.move(velocity * Time.deltaTime);

        animator.SetFloat("speed", Mathf.Abs(currentSpeed));

        distanceFromPlayer = Vector2.Distance(transform.position, player.position);


    }

    bool WillFall()
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

    void Attack()
    {

        direction = 0;

        transform.rotation = Quaternion.Euler(0, player.position.x >= transform.position.x ? 0 : 180, 0);

        if (distanceFromPlayer > 1)
            currentState = Patrol;

        animator.SetBool("isAttacking", true);
    }

    void Patrol()
    {

        animator.SetBool("isAttacking", false);

        if (direction == 0 && controller.isGrounded)
        {
            direction = player.position.x >= transform.position.x ? 1 : -1;
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

        if (distanceFromPlayer <= 1)
            currentState = Attack;

        //Adjust rotation
        transform.rotation = Quaternion.Euler(0, direction > 0 ? 0 : 180, 0);
    }
}

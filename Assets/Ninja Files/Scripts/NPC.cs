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

        if (direction == 0)
        {
            direction = player.position.x >= transform.position.x ? 1 : -1;
        }
            

        if (controller.collisionState.right)
            direction = -1;
        else
            if (controller.collisionState.left)
            direction = 1;

        if (distanceFromPlayer <= 1)
            currentState = Attack;

        //Adjust rotation
        transform.rotation = Quaternion.Euler(0, direction > 0 ? 0 : 180, 0);
    }
}

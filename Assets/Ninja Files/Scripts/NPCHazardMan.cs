using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHazardMan : NPC
{

    float timeToIdle;
    float timeInIdle;
    int storedDirection;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public override void Attack()
    {
        direction = 0;

        if (player)
            transform.rotation = Quaternion.Euler(0, player.transform.position.x >= transform.position.x ? 180 : 0, 0);

        if (distanceFromPlayer > distanceToAttack)
        {
            timeToIdle = timeToIdle = Random.Range(4, 6);
            currentState = Patrol;
        }


        animator.SetBool("isAttacking", true);
    }

    public override void Patrol()
    {
        timeToIdle -= Time.deltaTime;

        if (timeToIdle <= 0) {
            storedDirection = direction;
            animator.SetFloat("idle", Random.value);
            currentState = Idle;
        }


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

        if (distanceFromPlayer <= distanceToAttack)
            currentState = Attack;

        //Adjust rotation
        transform.rotation = Quaternion.Euler(0, direction > 0 ? 180 : 0, 0);

    }

    void Idle()
    {


        timeInIdle += Time.deltaTime;

        direction = 0;

        if (timeInIdle > 10)
        {
            timeToIdle = Random.Range(4, 6);
            timeInIdle = 0;
            direction = storedDirection;
            currentState = Patrol;
        }

        if (distanceFromPlayer <= distanceToAttack)
            currentState = Attack;


    }
}

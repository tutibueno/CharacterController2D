using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField]
    LayerMask platformMask;

    int totalVerticalRays = 3;
    float horizontalDistanceBetweenRays;
    BoxCollider2D boxCollider;
    float skinWidth = 0.02f;
    float timeToDisapear = 5;
    float timeGrounded;
    bool isGrounded;

    Vector3 raycastOrigin;

    [SerializeField]
    Vector3 velocity;

    RaycastHit2D raycastHit;
    Renderer _renderer;

    float timeAcum;

    // Start is called before the first frame update
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        recalculateDistanceBetweenRays();
        _renderer = GetComponent<Renderer>();
    }

    void primeRaycastOrigins()
    {
        var modifiedBounds = boxCollider.bounds;
        modifiedBounds.Expand(-2f * skinWidth);
        raycastOrigin = modifiedBounds.min;
    }

    // Update is called once per frame
    void Update()
    {

        primeRaycastOrigins();

        Move(new Vector3(velocity.x * Mathf.Sin(timeAcum * 3) , velocity.y, 0) * Time.deltaTime);

        if(isGrounded)
        {
            timeGrounded += Time.deltaTime;
            if(timeGrounded > timeToDisapear * 0.5f)
            {
                //Start Blinking
                _renderer.material.color = new Color(1, 1, 1, Time.frameCount % 3 == 0 ? 1 : 0);
            }
            if(timeGrounded > timeToDisapear)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            _renderer.material.color = Color.white;
            timeGrounded = 0;
        }

        isGrounded = false;
        timeAcum += Time.deltaTime;
    }


    /// <summary>
    /// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
    /// It is also used in the skinWidth setter in case it is changed at runtime.
    /// </summary>
    public void recalculateDistanceBetweenRays()
    {

        // vertical
        var colliderUseableWidth = boxCollider.size.x * Mathf.Abs(transform.localScale.x) - (2f * skinWidth);
        horizontalDistanceBetweenRays = colliderUseableWidth / (totalVerticalRays - 1);
    }

    void Move(Vector3 deltaMovement)
    {
        //move until hit the ground
        var initialRayOrigin = raycastOrigin;
        var rayDirection = Vector3.down;
        var rayDistance = skinWidth + Mathf.Abs(deltaMovement.y);

        for (var i = 0; i < totalVerticalRays; i++)
        {
            var ray = new Vector2(initialRayOrigin.x + i * horizontalDistanceBetweenRays, initialRayOrigin.y);

            Debug.DrawRay(ray, rayDirection * rayDistance, Color.magenta);
            raycastHit = Physics2D.Raycast(ray, rayDirection, rayDistance, platformMask);
            if(raycastHit)
            {
                isGrounded = true;
            }

        }

        if(!isGrounded)
        {
            transform.Translate(deltaMovement);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag.Equals("Player"))
        {
            Debug.Log("Ganhou Item!");
            Destroy(gameObject);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField]
    Vector3 toPosition;

    [SerializeField]
    float moveSpeed = 1;

    [SerializeField]
    bool isMovable;

    [SerializeField]
    bool isMoveDownWhenStep;

    [SerializeField]
    protected bool isOnPlatform;

    int directionX = 0;
    int directionY = 0;

    protected Vector3 initialPos;

    public Vector3 Velocity { get; private set; }

    Vector3 postionLastFrame;

    bool movingBackToInitial;

    public Vector3 deltaMoviment;

    bool willMoveVertically;
    bool willMoveHorizontally;

    protected Renderer _renderer;
    protected Collider2D _collider;


    void OnEnable()
    {
        Reset();
    }

    // Start is called before the first frame update
    public virtual void Awake()
    {
        initialPos = transform.position;
        postionLastFrame = transform.position;

        willMoveVertically = Mathf.Abs(transform.position.y - toPosition.y) > 0.1f;
        willMoveHorizontally = Mathf.Abs(transform.position.x - toPosition.x) > 0.1;

        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<Renderer>();

    }

    // Update is called once per frame
    public virtual void Update()
    {

        if (isMovable)
        {
            if (willMoveHorizontally)
            {
                if (transform.position.x >= toPosition.x)
                {
                    directionX = -1;
                    transform.position = new Vector3(toPosition.x, transform.position.y);
                }


                if (transform.position.x <= initialPos.x)
                {
                    directionX = 1;
                    transform.position = new Vector3(initialPos.x, transform.position.y);
                }
            }

            if (willMoveVertically)
            {
                if (transform.position.y >= toPosition.y)
                {
                    directionY = -1;
                    transform.position = new Vector3(transform.position.x, toPosition.y);
                }

                if (transform.position.y <= initialPos.y)
                {
                    directionY = 1;
                    transform.position = new Vector3(transform.position.x, initialPos.y);
                }
            }
        }


        if (isMoveDownWhenStep)
        {
            if (isOnPlatform)
            {
                directionY = -1;
                moveSpeed = 3;
            }
            else if (transform.position.y < initialPos.y && !isOnPlatform)
            {
                directionY = 1;
                moveSpeed = 1;
            }
            else directionY = 0;
        }

        deltaMoviment = new Vector2(moveSpeed * directionX * Time.deltaTime, moveSpeed * directionY * Time.deltaTime);

        Move(deltaMoviment);

        Velocity = transform.position - postionLastFrame;

        postionLastFrame = transform.position;

        isOnPlatform = false;

    }

    protected void Move(Vector3 _deltaMovement)
    {
        transform.Translate(new Vector2(_deltaMovement.x, _deltaMovement.y));
    }

    //called by character
    public void SetOnPlatform()
    {
        isOnPlatform = true;
    }

    public virtual void Reset()
    {

    }

    public void Deactivate()
    {
        _renderer.enabled = false;
        _collider.enabled = false;
        Invoke("Activate", 5);
    }

    public void Activate()
    {
        _renderer.enabled = true;
        _collider.enabled = true;
        _renderer.material.color = Color.white;
        Reset();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField]
    float damage = 50;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("collided Trigger");
        var health = collision.transform.GetComponent<HealthController>();
        if (health)
            health.OnHit(damage);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collided Collision");
        var health = collision.transform.GetComponent<HealthController>();
        if (health)
            health.OnHit(damage);
    }

}

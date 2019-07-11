using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{

    [SerializeField]
    float health = 100;

    public System.Action OnDie { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnHit(float amount)
    {
        health -= amount;
        if (health <= 0)
            Die();

    }

    void Die()
    {
        OnDie?.Invoke();
    }
}

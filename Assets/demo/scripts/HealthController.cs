using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{

    [SerializeField]
    float health = 100;

    [SerializeField]
    public bool isInvincible { get; private set; }

    public System.Action OnDie { get; set; }
    public System.Action OnHit { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Hit(float amount)
    {
        if (isInvincible)
            return;

        health -= amount;
        if (health <= 0)
        {
            Die();
            return;
        }

        OnHit?.Invoke();
            

    }

    void Die()
    {
        OnDie?.Invoke();
    }

    public void SetInvincible(bool enable)
    {
        isInvincible = enable;
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}

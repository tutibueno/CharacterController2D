using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{

    [SerializeField]
    float health = 10;

    [SerializeField]
    public bool IsInvincible { get; private set; }

    public bool IsDead { get { return health <= 0; } }

    public System.Action OnDie { get; set; }
    public System.Action<Vector2> OnHit { get; set; }

    public float HealthAmount { get { return health; } }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void Hit(float amount, Vector2 hitSource)
    {
        if (IsInvincible)
            return;

        if (IsDead)
            return;

        health -= amount;
        if (health <= 0)
        {
            Die();
        }

        OnHit?.Invoke(hitSource);
            

    }

    void Die()
    {
        OnDie?.Invoke();
    }

    public void SetInvincible(bool enable)
    {
        IsInvincible = enable;
    }

}

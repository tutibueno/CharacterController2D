using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformRotate : Platform
{

    [SerializeField]
    float rotationSpeed = 7;
    bool activateRotataion;
    float timer;
    float rotationTotal;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.25f;
    }

    // Update is called once per frame
    public override void Update()
    {
        if(isOnPlatform)
        {
            activateRotataion = true;
        }

        if(activateRotataion)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                _collider.enabled = false;
                rotationTotal += rotationSpeed;
                transform.rotation = Quaternion.Euler(0, 0, -rotationTotal);
                if (rotationTotal >= 360)
                    Reset();
            }
        }


        isOnPlatform = false;
    }

    public override void Reset()
    {
        timer = 0.25f;
        activateRotataion = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        _collider.enabled = true;
        rotationTotal = 0;
    }
}

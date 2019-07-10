using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDeteriorate : Platform
{
    [SerializeField]
    float deteriorateTime = 3;

    float deteriorateTimeStored;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();

        deteriorateTimeStored = deteriorateTime;
    }

    // Update is called once per frame
    public override void Update()
    {

        //Deteriorate platform
        if (isOnPlatform)
        {
            deteriorateTime -= Time.deltaTime;
            float normalizedTime = (deteriorateTime / deteriorateTimeStored);
            _renderer.material.color = new Color(1, 1, 1, normalizedTime);
            if (deteriorateTime <= 0)
                Deactivate();
        }
    

        isOnPlatform = false;
    }

    public override void Reset()
    {
        base.Reset();
        deteriorateTime = deteriorateTimeStored;
    }
}

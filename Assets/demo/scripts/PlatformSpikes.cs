using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpikes : Platform
{

    [SerializeField]
    float timeToSpawnSpikes = 2;

    [SerializeField]
    Transform spikes;

    bool isActivated;

    float timeToSpawnSpikesStored;

    Vector3 spikesInitialPos;


    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        timeToSpawnSpikesStored = timeToSpawnSpikes;
        spikesInitialPos = spikes.transform.localPosition;
    }

    // Update is called once per frame
    public override void Update()
    {

        if (timeToSpawnSpikes <= 0)
            return;

        if(isOnPlatform)
        {
            isActivated = true;
        }

        if(isActivated)
        {
            timeToSpawnSpikes -= Time.deltaTime;

            if(timeToSpawnSpikes <= 0)
            {
                spikes.transform.localPosition = new Vector3(0, 0.04f, 0);
                Invoke("Reset", 5);
            }

            else
                spikes.transform.localPosition = new Vector3(0, -0.03f, 0);



        }

        isOnPlatform = false;

    }

    public override void Reset()
    {
        timeToSpawnSpikes = timeToSpawnSpikesStored;
        spikes.transform.localPosition = spikesInitialPos;
        isActivated = false;
        isOnPlatform = false;
    }
}

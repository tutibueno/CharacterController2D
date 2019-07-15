using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{

    [SerializeField]
    Sprite[] lifeSpritesAnimation;

    int animationCurrentIndex;

    [SerializeField]
    int animationSpeed = 10;

    [SerializeField]
    Image lifeMeter;

    readonly int imageLifeWidth = 16;
    readonly int imageLifeHeight = 18;

    HealthController playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<HealthController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.frameCount % animationSpeed == 0)
        {
            animationCurrentIndex++;
            if (animationCurrentIndex >= lifeSpritesAnimation.Length)
                animationCurrentIndex = 0;

            lifeMeter.sprite = lifeSpritesAnimation[animationCurrentIndex];
        }

        //LifeSize
        lifeMeter.rectTransform.sizeDelta = new Vector2(imageLifeWidth * playerHealth.HealthAmount, imageLifeHeight);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampAnim : MonoBehaviour
{
    [SerializeField]
    Sprite[] sprites;

    [SerializeField]
    int frequency;

    SpriteRenderer spriteRenderer;

    int currentIndex;

    bool isRunning = true;
    [SerializeField]
    bool isLoop = true;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
            return;

        if(Time.frameCount % frequency == 0)
        {
            currentIndex++;
            if (currentIndex >= sprites.Length && isLoop)
                currentIndex = 0;
            else if(currentIndex >= sprites.Length && !isLoop)
            {
                return;
            }

            spriteRenderer.sprite = sprites[currentIndex];
        }

    }

    public void SetIsRunning(bool running)
    {
        isRunning = running;
    }

    public void SetRendererOnOff(bool enable)
    {
        spriteRenderer.enabled = enable;
    }

    public void SetNewSpriteSet(Sprite[] sprites)
    {
        this.sprites = sprites;
    }

    public void SetLoopEnable(bool enable)
    {
        isLoop = enable;
        currentIndex = 0;
    }
}

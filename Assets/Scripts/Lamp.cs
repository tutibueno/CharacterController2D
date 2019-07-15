using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    [SerializeField]
    DropItem itemToSpawn; //TODO colocar vários itens diferentes aqui
    [SerializeField]
    LampAnim mainAnim;
    [SerializeField]
    LampAnim[] lampAnims;
    [SerializeField]
    Sprite[] brakingSprites;
    [SerializeField]
    LayerMask mask;

    BoxCollider2D _collider;
    Rigidbody2D _rigid;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
        _rigid = GetComponent<Rigidbody2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("broken");
        foreach (var lampAnim in lampAnims)
        {
            lampAnim.SetIsRunning(false);
            lampAnim.SetRendererOnOff(false);
        }

        mainAnim.SetRendererOnOff(true);
        mainAnim.SetIsRunning(true);
        mainAnim.SetNewSpriteSet(brakingSprites);
        mainAnim.SetLoopEnable(false);

        //_collider.enabled = false;
        //_rigid.simulated = false;

        //Swpan object
        DropItem item = Instantiate<DropItem>(itemToSpawn, transform.position, Quaternion.identity);
    }
}

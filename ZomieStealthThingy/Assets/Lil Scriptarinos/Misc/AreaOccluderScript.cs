using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public class AreaOccluderScript : MonoBehaviour
{
    //this sprtie shape renderer, staryt color and color when no occlussion
    SpriteShapeRenderer SSR;
    Color SCol;
    [SerializeField] Color UnOccludedColor;
    //fetch variable ref
    private void Start()
    {
        SSR = GetComponent<SpriteShapeRenderer>();
        SCol = SSR.color;
    }
    //if player enters trigger, set unocluded color
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerMovementScript.player.gameObject)
            SSR.color = UnOccludedColor;
    }
    //if player leave trigger, set (start) occluded color
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == PlayerMovementScript.player.gameObject)
            SSR.color = SCol;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shootable : MonoBehaviour
{
    private Rigidbody2D R2D2;
    private float FORCE = 250;
    [SerializeField] protected Color DEAD = Color.grey;
    protected SpriteRenderer SR;
    protected virtual void Start()
    {
        R2D2 = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
        
    }

    public void OnGetShot(Vector2 V, float DVal)
    {
        DVal = Mathf.Clamp(DVal,0, 0.5f);
        V = (transform.position.ToVector2()-V).normalized;
        R2D2.AddForce(V * FORCE * DVal);
        SR.color = DEAD;
        //R2D2.constraints = RigidbodyConstraints2D.FreezeAll;
        OnDead();
    }


    protected abstract void OnDead();
}

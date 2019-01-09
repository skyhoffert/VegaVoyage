using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlasmaBall : MonoBehaviour
{

    private float damage = 0;

    private Rigidbody2D rb2D;

    private Vector2 newvel = new Vector2(0, 0);
    private bool newvel_wasset = true;

    // Start is called before the first frame update
    void Start()
    {
        this.rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!newvel_wasset){
            newvel_wasset = true;
            this.rb2D.velocity += newvel;
        }
    }

    public void SetVelocity(Vector2 v){
        this.newvel = v;
        this.newvel_wasset = false;
    }

    public void SetDamage(float f){
        this.damage = f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.SendMessage("ApplyRawDamage", this.damage);

        Destroy(this.gameObject);
    }
}

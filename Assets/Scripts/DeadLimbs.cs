using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadLimbs : MonoBehaviour
{

    private Rigidbody2D rb2d;

    private Vector2 newvel = new Vector2(0, 0);
    private bool newvel_wasset = true;

    private float newrot = 0.0f;
    private bool newrot_wasset = true;

    // Start is called before the first frame update
    void Start(){
        this.rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!newvel_wasset){
            this.newvel_wasset = true;
            this.rb2d.velocity += newvel;
        }

        if (!newrot_wasset){
            this.newrot_wasset = true;
            this.rb2d.angularVelocity = this.newrot;
        }
    }

    public void SetVelocity(Vector2 v){
        this.newvel = v;
        this.newvel_wasset = false;
    }
    public void SetRotationSpeed(float rot_speed){
        this.newrot = rot_speed;
        this.newrot_wasset = false;
    }
}

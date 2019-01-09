using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzPilot : MonoBehaviour
{
    // structure to hold state information about the player
    struct MuzzState{
        public bool thrust_enabled;
    }

    private MuzzState state;

    private GameObject player;
    private Rigidbody2D rb2d;

    private float detect_distance = 10.0f;

    private float thrust_force = 2.0f;
    
    private float max_velocity = 2.0f;

    private float collision_damage = 1.0f;

    private bool waslost = true;

    private float drag_whenlost = 0.8f;

    // Start is called before the first frame update
    void Start(){
        this.player = GameObject.FindWithTag("Player");

        this.rb2d = GetComponent<Rigidbody2D>();

        this.state.thrust_enabled = true;
    }

    // Update is called once per frame
    void Update(){
        if (this.state.thrust_enabled){
            // find distance to player
            float y_dist = player.transform.position.y - this.transform.position.y;
            float x_dist = player.transform.position.x - this.transform.position.x;
            Vector2 toplayer = new Vector2(x_dist, y_dist);

            if (toplayer.magnitude < this.detect_distance){
                Vector2 intercept = toplayer - rb2d.velocity;
                intercept.Normalize();
                this.rb2d.AddForce(intercept * this.thrust_force);
                
                // clamp velocity to below max velocity
                if (this.rb2d.velocity.magnitude > this.max_velocity){
                    this.rb2d.AddForce((this.max_velocity - this.rb2d.velocity.magnitude) * this.rb2d.velocity * this.thrust_force);
                }

                if (this.waslost){
                    this.rb2d.drag = 0.0f;
                }

                this.waslost = false;
            } else {
                if (!this.waslost){
                    this.rb2d.drag = this.drag_whenlost;
                }

                this.waslost = true;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.SendMessage("ApplyRawDamage", this.collision_damage);
    }
}

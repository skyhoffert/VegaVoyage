using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaulperPilot : MonoBehaviour
{
    private bool plasmaball_enabled;
    private bool thrust_enabled;
    private bool turn_enabled;
    private bool triggered;

    private bool paused = false;

    private Vector2 forward;
    private float rotation_speed = 10.0f;

    private GameObject player;

    private Rigidbody2D rb2d;

    private float thrust_force = 80.0f;
    private float thrust_maxangle = 35.0f; // makes sure sintzer is facing player before thrusting
    private float max_velocity = 3.0f;

    private float collision_damage = 10.0f;

    private float trigger_distance = 10.0f;

    private float aim_distance = 8.0f;
    private float aim_time = 0.0f;
    private float aim_duration = 0.5f;
    private float aim_cooldown = 2.0f;
    private bool aiming = false;
    private float aim_drag = 2.0f;

    private float projectile_offsetfactor = 1.0f;
    private float projectile_velocity_magnitude = 20.0f;
    private float projectile_damage = 10.1f;

    public GameObject projectile;

    public GameObject projectile_charge;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.FindWithTag("Player");

        this.plasmaball_enabled = true;
        this.thrust_enabled = true;
        this.turn_enabled = true;
        this.triggered = false;

        this.rb2d = GetComponent<Rigidbody2D>();

        this.forward = new Vector2(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.paused){ return; }

        // sit still until player comes within trigger range
        if (!this.triggered){
            float yt = player.transform.position.y - this.transform.position.y;
            float xt = player.transform.position.x - this.transform.position.x;
            
            if ( (new Vector2(xt, yt)).magnitude < this.trigger_distance){
                this.triggered = true;
            } else {
                return;
            }
        }

        // find distance to player
        float y_dist = player.transform.position.y - this.transform.position.y;
        float x_dist = player.transform.position.x - this.transform.position.x;
        
        // angle between forward and player
        float angle_to = 0;

        if (this.turn_enabled){
            // calculate forward direction
            this.forward.x = Mathf.Cos(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
            this.forward.y = Mathf.Sin(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
            this.forward.Normalize();
            
            // Lerp towards that direction
            Vector3 dir = new Vector3(0.0f, 0.0f, Mathf.Atan2(y_dist, x_dist) * Mathf.Rad2Deg - 90);
            Quaternion target = Quaternion.Euler(dir);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, target, Time.deltaTime * this.rotation_speed);

            angle_to = Vector2.Angle(forward, new Vector2(x_dist, y_dist));
        }

        // check the thrust_enabled state - act if it is active
        if (this.thrust_enabled && !this.aiming && angle_to < this.thrust_maxangle){
                Vector2 toplayer = new Vector2(x_dist, y_dist);
                Vector2 intercept = toplayer - rb2d.velocity;
                intercept.Normalize();
                this.rb2d.AddForce(intercept * this.thrust_force);
            
            // clamp velocity to below max velocity
            if (this.rb2d.velocity.magnitude > this.max_velocity){
                this.rb2d.AddForce((this.max_velocity - this.rb2d.velocity.magnitude) * this.rb2d.velocity * this.thrust_force);
            }
        }

        if (this.plasmaball_enabled){
            if (this.aiming){
                this.rb2d.drag = this.aim_drag;
                if (Time.time - this.aim_time > this.aim_duration){
                        Vector3 offset = new Vector3(this.forward.x * this.projectile_offsetfactor, this.forward.y * this.projectile_offsetfactor, 0);
                        GameObject g = Instantiate(this.projectile, this.transform.position + offset, transform.rotation) as GameObject;
                        g.SendMessage("SetVelocity", this.forward * this.projectile_velocity_magnitude);
                        g.SendMessage("SetDamage", this.projectile_damage);
                        this.aiming = false;
                        this.rb2d.drag = 0.0f;
                }
            } else {
                if ( (new Vector2(x_dist, y_dist)).magnitude < this.aim_distance){
                    if (Time.time - this.aim_time > this.aim_cooldown){
                        this.aiming = true;
                        this.aim_time = Time.time;
                        GameObject g = Instantiate(this.projectile_charge, this.transform.position + new Vector3(this.forward.x/2, this.forward.y/2, 0), this.transform.rotation) as GameObject;
                        g.transform.parent = this.transform;
                    }
                }
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.gameObject.SendMessage("ApplyRawDamage", this.collision_damage);
    }

    void Pause(bool p){
        this.paused = p;
    }
}

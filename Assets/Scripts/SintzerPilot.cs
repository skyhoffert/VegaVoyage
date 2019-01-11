using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SintzerPilot : MonoBehaviour
{
    private bool lunge_enabled;
    private bool thrust_enabled;
    private bool turn_enabled;
    private bool lunging;
    private bool lunge_ready;

    private bool paused = false;

    private Vector2 forward;
    private float rotation_speed = 10.0f;

    private GameObject player;

    private Rigidbody2D rb2d;

    private float lunge_range = 6.0f;
    private float lunge_time = 0.0f;
    private float lunge_force = 1200.0f;
    private float lunge_duration = 1.0f;
    private float lunge_maxangle = 4.0f; // makes sure sintzer is looking directly at player before lunging
    private float lunge_chargeduration = 0.5f;
    private float lunge_chargetime = 0.0f;
    private bool lunge_charging = false;

    private float thrust_force = 50.0f;
    private float thrust_maxangle = 35.0f; // makes sure sintzer is facing player before thrusting
    private float max_velocity = 4.0f;

    private float collision_damage = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.FindWithTag("Player");

        this.lunge_enabled = true;
        this.thrust_enabled = true;
        this.turn_enabled = true;
        this.lunging = false;
        this.lunge_ready = false;

        this.rb2d = GetComponent<Rigidbody2D>();

        this.forward = new Vector2(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.paused){ return; }

        if (this.lunging){
            if (Time.time > this.lunge_time + this.lunge_duration){
                this.lunging = false;
                this.turn_enabled = true;
                this.lunge_enabled = true;
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
            
            // calculate how much to turn
            float rotation_amount = Mathf.Atan2(y_dist, x_dist) * Mathf.Rad2Deg - 90;
            
            // Lerp towards that direction
            Vector3 dir = new Vector3(0.0f, 0.0f, rotation_amount);
            Quaternion target = Quaternion.Euler(dir);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, target, Time.deltaTime * this.rotation_speed);

            angle_to = Vector2.Angle(forward, new Vector2(x_dist, y_dist));
            this.lunge_ready = Mathf.Abs(angle_to) < this.lunge_maxangle;
        }

        if (this.lunge_enabled){
            if (this.lunge_ready && !this.lunge_charging){
                float dist = (new Vector2(x_dist, y_dist)).magnitude;
                if (dist <= lunge_range){
                    this.rb2d.velocity = Vector2.zero;
                    this.lunge_charging = true;
                    this.lunge_chargetime = Time.time;
                }
            }
        }

        if (this.lunge_charging){
            if (Time.time - this.lunge_chargetime > this.lunge_chargeduration){
                    this.lunge_charging = false;
                    
                    this.lunging = true;
                    this.turn_enabled = false;
                    this.lunge_enabled = false;
                    this.lunge_time = Time.time;

                    this.rb2d.AddForce(this.forward * this.lunge_force);
            }
        }

        // check the thrust_enabled state - act if it is active
        if (this.thrust_enabled && !this.lunging && angle_to < this.thrust_maxangle){
                Vector2 toplayer = new Vector2(x_dist, y_dist);
                Vector2 intercept = toplayer - rb2d.velocity;
                intercept.Normalize();
                this.rb2d.AddForce(intercept * this.thrust_force);
            
            // clamp velocity to below max velocity
            if (this.rb2d.velocity.magnitude > this.max_velocity){
                this.rb2d.AddForce((this.max_velocity - this.rb2d.velocity.magnitude) * this.rb2d.velocity * this.thrust_force);
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

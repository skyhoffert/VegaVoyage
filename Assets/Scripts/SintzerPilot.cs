using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SintzerPilot : MonoBehaviour
{
    // structure to hold state information about the player
    struct SintzerState{
        public bool lunge_enabled;
        public bool thrust_enabled;
        public bool turn_enabled;
        public bool lunging;
        public bool lunge_ready;
    }

    private SintzerState state;

    private Vector2 forward;
    private float rotation_speed = 10.0f;

    private GameObject player;

    private Rigidbody2D rb2d;

    private float lunge_range = 8.0f;
    private float lunge_time = 0.0f;
    private float lunge_force = 1200.0f;
    private float lunge_duration = 1.0f;
    private float lunge_maxangle = 4.0f; // makes sure sintzer is looking directly at player before lunging

    private float thrust_force = 50.0f;
    private float thrust_maxangle = 35.0f; // makes sure sintzer is facing player before thrusting
    private float max_velocity = 4.0f;

    private float collision_damage = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");

        this.state.lunge_enabled = true;
        this.state.thrust_enabled = true;
        this.state.turn_enabled = true;
        this.state.lunging = false;
        this.state.lunge_ready = false;

        this.rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.state.lunging){
            if (Time.time > this.lunge_time + this.lunge_duration){
                this.state.lunging = false;
                this.state.turn_enabled = true;
                this.state.lunge_enabled = true;
            }
        }

        // find distance to player
        float y_dist = player.transform.position.y - this.transform.position.y;
        float x_dist = player.transform.position.x - this.transform.position.x;
        
        // angle between forward and player
        float angle_to = 0;

        if (this.state.turn_enabled){
            // calculate forward direction
            forward.x = Mathf.Sin(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
            forward.y = -Mathf.Cos(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
            forward.Normalize();
            
            // calculate how much to turn
            float rotation_amount = Mathf.Atan2(y_dist, x_dist) * Mathf.Rad2Deg + Mathf.PI/2;
            
            // Lerp towards that direction
            Vector3 dir = new Vector3(0.0f, 0.0f, rotation_amount);
            Quaternion target = Quaternion.Euler(dir);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, target, Time.deltaTime * this.rotation_speed);

            angle_to = Vector2.Angle(forward, new Vector2(x_dist, y_dist));
            this.state.lunge_ready = Mathf.Abs(angle_to) < this.lunge_maxangle;
        }

        if (this.state.lunge_enabled){
            if (this.state.lunge_ready){
                float dist = (new Vector2(x_dist, y_dist)).magnitude;
                
                if (dist <= lunge_range){
                    this.state.lunging = true;
                    this.state.turn_enabled = false;
                    this.state.lunge_enabled = false;
                    this.lunge_time = Time.time;

                    this.rb2d.AddForce(this.forward * this.lunge_force);
                }
            }
        }

        // check the thrust_enabled state - act if it is active
        if (this.state.thrust_enabled && !this.state.lunging && angle_to < this.thrust_maxangle){
            this.rb2d.AddForce(this.forward * this.thrust_force);
            
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // structure to hold state information about the player
    struct PlayerState{
        public bool thrust_enabled;
        public bool turn_enabled;
        public bool plasmaball_enabled;
        public bool can_damage;
        public bool can_heal;
        public bool iframes_enabled;
    }

    private PlayerState state;

    public GameObject exhaust;
    public ParticleSystem particle_system;

    public GameObject plasmaball;

    private Rigidbody2D rb2d;

    private float thrust_force = 100.0f;
    private float rotation_speed = 5.0f;

    private float max_velocity = 8.0f;

    private float plasmaball_velocity_magnitude = 20.0f;
    private float plasmaball_damage = 10.1f;
    
    private Vector2 forward;
    
    private float shield_points;
    private float max_shield_points;

    private float iframes_starttime = 0.0f;
    private float iframes_duration = 1.0f;
    private float iframes_minalpha = 0.4f;

    public SpriteRenderer sr;
    public PolygonCollider2D coll;
    public GameObject damage_particle_system;

    // Start is called before the first frame update
    void Start()
    {
        // start off being able to move
        this.state.thrust_enabled = true;
        this.state.turn_enabled = true;
        this.state.plasmaball_enabled = true;
        this.state.can_damage = true;
        this.state.can_heal = true;
        this.state.iframes_enabled = false;

        this.forward = new Vector2(1, 0);

        this.exhaust.SetActive(false);

        this.rb2d = GetComponent<Rigidbody2D>();

        this.max_shield_points = 1;
        this.shield_points = this.max_shield_points;
        
        // TODO - broken
        //this.particle_system.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        this.particle_system.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // handle iframes
        if (this.state.iframes_enabled){
            if (Time.time - this.iframes_starttime > this.iframes_duration){
                this.state.iframes_enabled = false;
                
                Color tmp = this.sr.color;
                tmp.a = 1.0f;
                this.sr.color = tmp;

                this.coll.enabled = true;
            }
        }

        // handle turning the player
        if (this.state.turn_enabled){    
            float x_thumb = Input.GetAxis("axx");
            float y_thumb = Input.GetAxis("axy");

            if (Mathf.Abs(x_thumb) > 0.1 || Mathf.Abs(y_thumb) > 0.1){
                // calculate forward direction
                forward.x = Mathf.Sin(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
                forward.y = -Mathf.Cos(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
                forward.Normalize();
                
                // calculate how much to turn
                float rotation_amount = -Mathf.Atan2(y_thumb, x_thumb) * Mathf.Rad2Deg;
                
                // Lerp towards that direction
                Vector3 dir = new Vector3(0.0f, 0.0f, rotation_amount);
                Quaternion target = Quaternion.Euler(dir);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, target, Time.deltaTime * this.rotation_speed);
            }
        }

        // check the thrust_enabled state - act if it is active
        if (this.state.thrust_enabled){
            float ax3 = Input.GetAxis("ax3");

            // set a lower limit for the exhaust to be enabled
            if (ax3 > 0.1){
                this.exhaust.SetActive(true);

                this.rb2d.AddForce(this.forward * this.thrust_force);

                // TODO - broken
                //this.particle_system.Play(true);
                this.particle_system.gameObject.SetActive(true);
            } else {
                if (this.exhaust.activeSelf){
                    this.exhaust.SetActive(false);

                    // TODO - broken
                    //this.particle_system.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    this.particle_system.gameObject.SetActive(false);
                }
            }

            if (ax3 < -0.1){
                this.rb2d.AddForce(-this.forward * this.thrust_force / 2);
            }
        }
        
        // clamp velocity to below max velocity
        if (this.rb2d.velocity.magnitude > this.max_velocity){
            this.rb2d.AddForce((this.max_velocity - this.rb2d.velocity.magnitude) * this.rb2d.velocity * this.thrust_force);
        }

        // handle firing plasma balls
        if (this.state.plasmaball_enabled){
            if (Input.GetButtonDown("b1")){
                GameObject g = Instantiate(plasmaball, transform.position + new Vector3(this.forward.x, this.forward.y, 0), transform.rotation) as GameObject;
                g.SendMessage("SetVelocity", this.forward * this.plasmaball_velocity_magnitude);
                g.SendMessage("SetDamage", this.plasmaball_damage);
            }
        }
    }
    
    public void ApplyRawDamage(float damage){
        if (!this.state.iframes_enabled && this.state.can_damage){
            // create a damage particle system
            GameObject dmg = Instantiate(damage_particle_system, transform.position, Quaternion.identity);

            // health for player goes by shield points
            if (this.shield_points > 0){
                this.shield_points--;

                this.iframes_starttime = Time.time;
                this.state.iframes_enabled = true;

                Color tmp = this.sr.color;
                tmp.a = iframes_minalpha;
                this.sr.color = tmp;

                this.coll.enabled = false;
            } else if (this.shield_points == 0){
                Destroy(this.gameObject);
                
                Debug.Log(this.gameObject + " has been destroyed");

            }
        }
    }

}

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
        public bool damage_enabled;
        public bool heal_enabled;
        public bool iframes_enabled;
        public bool shield_rechage_enabled;
    }

    private PlayerState state;

    public GameObject exhaust;
    public ParticleSystem particle_system;

    public GameObject plasmaball;
    
    public SpriteRenderer sr;
    public PolygonCollider2D coll;
    public GameObject damage_particle_system;

    public GameObject plasmaball_charge;

    public GameObject sp1;
    public GameObject sp1_missing;

    private Rigidbody2D rb2d;

    private float thrust_force = 100.0f;
    private float rotation_speed = 5.0f;

    private float max_velocity = 8.0f;

    private float plasmaball_velocity_magnitude = 20.0f;
    private float plasmaball_damage = 10.1f;
    private float plasmaball_chargetime = 0.1f;
    private float plasmaball_firetime = 0.0f;
    private float plasmaball_cooldown = 0.7f;
    private bool plasmaball_ready = true;
    private float plasmaball_spawnoffsetfactor = 0.8f;
    
    private Vector2 forward;
    
    private float shield_points;
    private float max_shield_points;

    // variables handling invulnerability frames
    private float iframes_starttime = 0.0f;
    private float iframes_duration = 1.0f;
    private float iframes_minalpha = 0.4f;

    private float shield_recharge_time = 5.0f;
    private float latest_damage_time = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // start off being able to move
        this.state.thrust_enabled = true;
        this.state.turn_enabled = true;
        this.state.plasmaball_enabled = true;
        this.state.damage_enabled = true;
        this.state.heal_enabled = true;
        this.state.iframes_enabled = false;
        this.state.shield_rechage_enabled = true;

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
                
                ChangeAlphaOfImage(1.0f);

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
            } else if (ax3 < -0.1){
                // if left trigger is being pressed
                // TODO - this probably won't be in the game
                this.rb2d.AddForce(-this.forward * this.thrust_force / 2);
            } else {
                if (this.exhaust.activeSelf){
                    this.exhaust.SetActive(false);

                    // TODO - broken
                    //this.particle_system.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    this.particle_system.gameObject.SetActive(false);
                }
            }
        }
        
        // clamp velocity to below max velocity
        if (this.rb2d.velocity.magnitude > this.max_velocity){
            this.rb2d.AddForce((this.max_velocity - this.rb2d.velocity.magnitude) * this.rb2d.velocity * this.thrust_force);
        }

        // handle firing plasma balls
        if (this.state.plasmaball_enabled){
            if (this.plasmaball_ready){
                if (Input.GetButtonDown("b1")){
                    if (Time.time - this.plasmaball_firetime > this.plasmaball_cooldown){
                        this.plasmaball_firetime = Time.time;
                        this.plasmaball_ready = false;

                        GameObject g = Instantiate(plasmaball_charge, this.transform.position + new Vector3(this.forward.x/2, this.forward.y/2, 0), this.transform.rotation) as GameObject;
                        g.transform.parent = this.transform;
                    }
                }
            }
            
            if (!this.plasmaball_ready){
                if (Time.time - this.plasmaball_firetime > this.plasmaball_chargetime){
                    Vector3 offset = new Vector3(this.forward.x * this.plasmaball_spawnoffsetfactor, this.forward.y * this.plasmaball_spawnoffsetfactor, 0);
                    GameObject g = Instantiate(plasmaball, this.transform.position + offset, transform.rotation) as GameObject;
                    g.SendMessage("SetVelocity", this.forward * this.plasmaball_velocity_magnitude);
                    g.SendMessage("SetDamage", this.plasmaball_damage);
                    this.plasmaball_ready = true;
                }
            }
        }

        // recharge shield points if possible
        if (this.state.shield_rechage_enabled){
            if (this.shield_points < this.max_shield_points){
                if (Time.time - this.latest_damage_time > this.shield_recharge_time){
                    this.latest_damage_time = Time.time;
                    this.shield_points++;
                    this.sp1.SetActive(true);
                }
            }
        }
    }
    
    public void ApplyRawDamage(float damage){
        // don't do anything if damage is less than 0
        if (damage < 0){ return; }

        if (!this.state.iframes_enabled && this.state.damage_enabled){
            // create a damage particle system
            GameObject dmg = Instantiate(damage_particle_system, transform.position, Quaternion.identity);

            // health for player goes by shield points
            if (this.shield_points > 0){
                this.shield_points--;

                this.iframes_starttime = Time.time;
                this.state.iframes_enabled = true;

                this.coll.enabled = false;

                this.sp1.SetActive(false);

                this.latest_damage_time = Time.time;

                ChangeAlphaOfImage(this.iframes_minalpha);
            } else if (this.shield_points == 0){
                this.gameObject.SetActive(false);

                Debug.Log("Player died.");
            }
        }
    }

    // modify the alpha value of the player's ship image
    // changing this value is used to represent when player has iframes or not
    private void ChangeAlphaOfImage(float a){
        Color tmp = this.sr.color;
        tmp.a = a;
        this.sr.color = tmp;
    }

}

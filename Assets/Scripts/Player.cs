using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // hold state information about player
    private bool thrust_enabled;
    private bool turn_enabled;
    private bool plasmaball_enabled;
    private bool damage_enabled;
    private bool heal_enabled;
    private bool iframes_enabled;
    private bool shield_rechage_enabled;
    private bool dash_enabled;
    private bool laser_enabled;

    public GameObject exhaust;
    public ParticleSystem particle_system;

    public GameObject plasmaball;
    
    public SpriteRenderer sr;
    public PolygonCollider2D coll;
    public GameObject damage_particle_system;

    public GameObject plasmaball_charge;

    public GameObject laser_charge;

    public GameObject laser_renderer;

    public GameObject sp1;
    public GameObject sp1_missing;

    private Rigidbody2D rb2d;

    private static float max_thrust_force = 100.0f;
    private float thrust_force = max_thrust_force;

    private static float max_rotation_speed = 4.0f;
    private float rotation_speed = max_rotation_speed;

    private static float max_velocity_cap = 8.0f;
    private float max_velocity = max_velocity_cap;

    private float plasmaball_velocity_magnitude = 20.0f;
    private float plasmaball_damage = 10.1f;
    private float plasmaball_chargetime = 0.1f;
    private float plasmaball_firetime = 0.0f;
    private float plasmaball_cooldown = 0.7f;
    private bool plasmaball_ready = true;
    private float plasmaball_spawnoffsetfactor = 0.8f;

    private float laser_damage = 3.8f;
    private bool laser_firing = false;
    private bool laser_charging = false;
    private float laser_range = 5.0f;
    private float laser_renderer_increasefactor = 1.2f;
    private float laser_chargetime = 0.5f;
    private float laser_duration = 2.5f;
    private float laser_firetime = 0.0f;
    private float laser_maxvelocity = 0.3f * max_velocity_cap;
    private float laser_maxthrust = 0.3f * max_thrust_force;
    private float laser_maxrotation = 0.25f * max_rotation_speed;

    private float dash_magnitude = 2.0f;
    
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
        this.thrust_enabled = true;
        this.turn_enabled = true;
        this.plasmaball_enabled = true;
        this.damage_enabled = true;
        this.heal_enabled = true;
        this.iframes_enabled = false;
        this.shield_rechage_enabled = true;
        this.dash_enabled = true;
        this.laser_enabled = true;

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
        if (this.iframes_enabled){
            if (Time.time - this.iframes_starttime > this.iframes_duration){
                this.iframes_enabled = false;
                
                ChangeAlphaOfImage(1.0f);

                this.coll.enabled = true;
            }
        }

        // handle turning the player
        if (this.turn_enabled){    
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
        if (this.thrust_enabled){
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
        if (this.plasmaball_enabled){
            if (this.plasmaball_ready){
                if (Input.GetButtonDown("b0")){
                    if (Time.time - this.plasmaball_firetime > this.plasmaball_cooldown){
                        this.plasmaball_firetime = Time.time;
                        this.plasmaball_ready = false;

                        GameObject g = Instantiate(plasmaball_charge, this.transform.position + new Vector3(this.forward.x/2, this.forward.y/2, 0), this.transform.rotation) as GameObject;
                        g.transform.parent = this.transform;
                    }
                }
            } else {
                if (Time.time - this.plasmaball_firetime > this.plasmaball_chargetime){
                    Vector3 offset = new Vector3(this.forward.x * this.plasmaball_spawnoffsetfactor, this.forward.y * this.plasmaball_spawnoffsetfactor, 0);
                    GameObject g = Instantiate(plasmaball, this.transform.position + offset, transform.rotation) as GameObject;
                    g.SendMessage("SetVelocity", this.forward * this.plasmaball_velocity_magnitude);
                    g.SendMessage("SetDamage", this.plasmaball_damage);
                    this.plasmaball_ready = true;
                }
            }
        }

        if (this.laser_enabled){
            if (Input.GetButtonDown("b1")){
                if (!this.laser_firing && !this.laser_charging){
                    this.laser_charging = true;
                    this.laser_firetime = Time.time;
                    
                    GameObject g = Instantiate(laser_charge, this.transform.position + new Vector3(this.forward.x/2, this.forward.y/2, 0), this.transform.rotation) as GameObject;
                    g.transform.parent = this.transform;
                    
                    this.max_velocity = this.laser_maxvelocity;
                    this.thrust_force = this.laser_maxthrust;
                    this.rotation_speed = this.laser_maxrotation;
                }
            }

            if (Input.GetButtonUp("b1")){
                if (this.laser_firing){
                    this.laser_firing = false;
                    
                    this.laser_renderer.SetActive(false);

                    this.max_velocity = max_velocity_cap;
                    this.thrust_force = max_thrust_force;
                    this.rotation_speed = max_rotation_speed;
                }
                if (this.laser_charging){
                    this.laser_charging = false;
                    
                    this.max_velocity = max_velocity_cap;
                    this.thrust_force = max_thrust_force;
                    this.rotation_speed = max_rotation_speed;
                }
            }
        }

        if (this.laser_charging){
            if (Time.time - this.laser_firetime > this.laser_chargetime){
                    this.laser_renderer.transform.localScale = new Vector3(this.laser_range * this.laser_renderer_increasefactor, 1, 1);
                    this.laser_renderer.SetActive(true);
                    this.laser_firing = true;
                    this.laser_charging = false;
            }
        }

        if (this.laser_firing){
            RaycastHit2D[] hits = Physics2D.RaycastAll(this.laser_renderer.transform.position, this.forward, this.laser_range);
            
            for (int i = 0; i < hits.Length; i++){
                if (hits[i]){
                    hits[i].collider.gameObject.SendMessage("ApplyRawDamage", this.laser_damage);
                }
            }

            if (Time.time - this.laser_firetime > this.laser_duration){
                this.laser_firing = false;

                this.laser_renderer.SetActive(false);

                this.max_velocity = max_velocity_cap;
                this.thrust_force = max_thrust_force;
                this.rotation_speed = max_rotation_speed;
            }
        }

        if (this.dash_enabled){
            if (Input.GetButtonDown("b2")){
                this.transform.position += new Vector3(this.forward.x, this.forward.y, 0) * this.dash_magnitude;
                this.rb2d.velocity = this.rb2d.velocity.magnitude * this.forward;
            }
        }

        // recharge shield points if possible
        if (this.shield_rechage_enabled){
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

        if (!this.iframes_enabled && this.damage_enabled){
            // create a damage particle system
            GameObject dmg = Instantiate(damage_particle_system, transform.position, Quaternion.identity);

            // health for player goes by shield points
            if (this.shield_points > 0){
                this.shield_points--;

                this.iframes_starttime = Time.time;
                this.iframes_enabled = true;

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

    public void ApplyRawHealing(float healing){
        if (this.heal_enabled){
            this.shield_points++;

            if (this.shield_points > this.max_shield_points){
                this.shield_points = this.max_shield_points;
            }

            // TODO - more stuff
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

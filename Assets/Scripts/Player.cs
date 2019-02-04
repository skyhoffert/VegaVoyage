using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // hold state information about player
    private bool thrust_enabled = true;
    private bool turn_enabled = true;
    private bool plasmaball_enabled = true;
    private bool damage_enabled = true;
    private bool heal_enabled = true;
    private bool iframes_enabled = true;
    private bool shield_rechage_enabled = true;
    private bool dash_enabled = true;
    private bool laser_enabled = true;
    private bool paused = false;
    private bool slow_enabled = true;

    public TrailRenderer trail;

    public GameObject exhaust;
    public ParticleSystem particle_system;

    public GameObject plasmaball;
    
    public SpriteRenderer sr;
    public GameObject damage_particle_system;

    public GameObject plasmaball_charge;

    public GameObject laser_charge;

    public LineRenderer laser_renderer;

    public GameObject sp1;
    public GameObject sp1_missing;

    public Text upgrade_text;

    private Rigidbody2D rb2d;

    private static float max_thrust_force = 100.0f;
    private float thrust_force = max_thrust_force;

    private static float max_rotation_speed = 4.0f;
    private float rotation_speed = max_rotation_speed;

    private static float max_velocity_cap = 10.0f;
    private float max_velocity = max_velocity_cap;

    private float plasmaball_velocity_magnitude = 20.0f;
    private float plasmaball_damage = 10.1f;
    private float plasmaball_chargetime = 0.1f;
    private float plasmaball_firetime = 0.0f;
    private float plasmaball_cooldown = 0.7f;
    private bool plasmaball_ready = true;
    private float plasmaball_spawnoffsetfactor = 1.0f;

    private float laser_damage = 3.8f;
    private bool laser_firing = false;
    private bool laser_charging = false;
    private float laser_range = 7.0f;
    private float laser_renderer_increasefactor = 1.2f;
    private float laser_chargetime = 0.5f;
    private float laser_duration = 2.5f;
    private float laser_firetime = 0.0f;
    private float laser_maxvelocity = 0.3f * max_velocity_cap;
    private float laser_maxthrust = 0.3f * max_thrust_force;
    private float laser_maxrotation = 0.25f * max_rotation_speed;

    private float dash_magnitude = 2.0f;

    private float slow_factor = 0.1f;

    private float thrust_emitrate = 20.0f;
    
    private Vector2 forward;
    
    private float shield_points = 1;
    private float max_shield_points = 1;

    private int currency_upgrade_amt = 0;

    // variables handling invulnerability frames
    private float iframes_starttime = 0.0f;
    private float iframes_duration = 1.0f;
    private float iframes_minalpha = 0.4f;

    private float shield_recharge_time = 5.0f;
    private float latest_damage_time = 0.0f;

    // handles frame after unpausing to prevent unwanted behavior
    private bool just_unpaused = false;

    // Start is called before the first frame update
    void Start()
    {
        // calculate forward direction
        this.forward.x = Mathf.Sin(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
        this.forward.y = -Mathf.Cos(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
        this.forward.Normalize();

        this.exhaust.SetActive(false);

        this.rb2d = GetComponent<Rigidbody2D>();
        
        var emission = this.particle_system.emission;
        emission.rateOverTime = 0;

        this.upgrade_text.text = "" + this.currency_upgrade_amt;

        this.currency_upgrade_amt = PlayerPrefs.GetInt("upgrade_currency_amt");
        
        this.upgrade_text.text = "" + this.currency_upgrade_amt;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.paused){
            this.just_unpaused = true;
            return;
        } else if (this.just_unpaused){
            this.just_unpaused = false;
            return;
        }

        // handle iframes
        if (this.iframes_enabled){
            if (Time.time - this.iframes_starttime > this.iframes_duration){
                this.iframes_enabled = false;
                
                ChangeAlphaOfImage(1.0f);
            }
        }

        // handle turning the player
        if (this.turn_enabled){    
            float x_thumb = Input.GetAxis("axx");
            float y_thumb = Input.GetAxis("axy");

            if (Mathf.Abs(x_thumb) > 0.1 || Mathf.Abs(y_thumb) > 0.1){
                this.forward = this.transform.right;
                
                // calculate how much to turn
                float rotation_amount = -Mathf.Atan2(y_thumb, x_thumb) * Mathf.Rad2Deg;
                
                // Lerp towards that direction
                Vector3 dir = new Vector3(0.0f, 0.0f, rotation_amount);
                Quaternion target = Quaternion.Euler(dir);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, target, Time.deltaTime * this.rotation_speed);
            } else {
                this.forward = this.transform.right;
            }
        }

        // check the thrust_enabled state - act if it is active
        if (this.thrust_enabled){
            float ax3 = Input.GetAxis("ax3");

            // set a lower limit for the exhaust to be enabled
            if (ax3 > 0.1){
                this.exhaust.SetActive(true);

                this.rb2d.AddForce(this.forward * this.thrust_force);

                var emission = this.particle_system.emission;
                emission.rateOverTime = this.thrust_emitrate;
            } else if (ax3 < -0.1){
                // if left trigger is being pressed
                if (this.slow_enabled){
                    this.rb2d.AddForce(-this.rb2d.velocity * this.thrust_force * this.slow_factor);
                    
                    var emission = this.particle_system.emission;
                    emission.rateOverTime = 0;
                }
            } else {
                if (this.exhaust.activeSelf){
                    this.exhaust.SetActive(false);
                }

                var emission = this.particle_system.emission;
                emission.rateOverTime = 0;
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
                    
                    this.laser_renderer.gameObject.SetActive(false);

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
                    this.laser_renderer.gameObject.SetActive(true);
                    this.laser_firing = true;
                    this.laser_charging = false;
                    Vector3 scale = this.laser_renderer.gameObject.transform.localScale;
                    scale.x = this.laser_range;
                    this.laser_renderer.gameObject.transform.localScale = scale;
            }
        }

        if (this.laser_firing){
            LayerMask mask = LayerMask.GetMask("EnemySpace", "FarFG");
            RaycastHit2D[] hits = Physics2D.RaycastAll(this.laser_renderer.transform.position, this.forward, this.laser_range, mask);
            
            for (int i = 0; i < hits.Length; i++){
                if (hits[i]){
                    hits[i].collider.gameObject.SendMessage("ApplyRawDamage", this.laser_damage);
                }
            }

            if (Time.time - this.laser_firetime > this.laser_duration){
                this.laser_firing = false;

                this.laser_renderer.gameObject.SetActive(false);

                this.max_velocity = max_velocity_cap;
                this.thrust_force = max_thrust_force;
                this.rotation_speed = max_rotation_speed;
            }
        }

        if (this.dash_enabled){
            if (Input.GetButtonDown("b2")){
                LayerMask mask = LayerMask.GetMask("FarFG");
                RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, this.forward, this.dash_magnitude, mask);

                bool fail = false;
                Vector3 topoint = this.transform.position + new Vector3(this.forward.x, this.forward.y, 0) * this.dash_magnitude;
                for (int i = 0; i < hits.Length; i++){
                    if (hits[i].collider.isTrigger){
                        continue;
                    }
                    
                    if (hits[i].collider.OverlapPoint(new Vector2(topoint.x, topoint.y))){
                        fail = true;
                    }
                }

                if (!fail){
                    this.transform.position = topoint;
                    this.rb2d.velocity = this.rb2d.velocity.magnitude * this.forward;
                }
            }
        }

        if (this.shield_rechage_enabled){
            // must prevent adding too many shield points
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

    public void Pause(bool p){
        this.paused = p;
    }

    public void AddUpgradeCurrency(int num){
        this.currency_upgrade_amt += num;
        
        this.upgrade_text.text = "" + this.currency_upgrade_amt;

        PlayerPrefs.SetInt("upgrade_currency_amt", this.currency_upgrade_amt);
        PlayerPrefs.Save();
    }

    public void ClearTrail(){
        this.trail.Clear();
    }

    // modify the alpha value of the player's ship image
    // changing this value is used to represent when player has iframes or not
    private void ChangeAlphaOfImage(float a){
        Color tmp = this.sr.color;
        tmp.a = a;
        this.sr.color = tmp;
    }

}

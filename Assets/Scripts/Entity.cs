using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
    
    public bool can_damage = true;
    public bool can_heal = true;
    private bool paused = false;

    public float health;
    public float maxhp;
    private bool iframes_enabled = false;

    private float iframes_starttime = 0.0f;
    private float iframes_duration = 10.0f;
    public float iframes_duration_perdamagepoint = 0.1f;
    public float iframes_duration_max = 0.5f;
    public float iframes_minalpha = 0.2f;

    private SpriteRenderer sr;

    public GameObject damage_particle_system;

    void Start(){
        this.sr = GetComponent<SpriteRenderer>();
    }

    void Update(){
        if (this.paused){ return; }

        if (iframes_enabled){
            if (Time.time - this.iframes_starttime > this.iframes_duration){
                this.iframes_enabled = false;
                
                Color tmp = this.sr.color;
                tmp.a = 1.0f;
                this.sr.color = tmp;
            }
        }
    }
    
    public void ApplyRawDamage(float damage){
        if (!this.iframes_enabled && this.can_damage){
            // create a damage particle system
            GameObject dmg = Instantiate(damage_particle_system, transform.position, Quaternion.identity);

            // subtract damage and handle the damage
            this.health -= damage;

            if (this.health < 0){ this.health = 0; }

            if (this.health <= 0){
                Destroy(this.gameObject);
                
                Debug.Log(this.gameObject + " has been destroyed");
            } else {
                this.iframes_starttime = Time.time;
                this.iframes_enabled = true;

                // handle iframe duration - changes with damage amount up to a max
                this.iframes_duration = damage * this.iframes_duration_perdamagepoint;
                if (this.iframes_duration > this.iframes_duration_max){
                    this.iframes_duration = this.iframes_duration_max;
                }

                Color tmp = this.sr.color;
                tmp.a = iframes_minalpha;
                this.sr.color = tmp;
            }
        }
    }
    
    public void Heal(float hp){
        if (can_heal){
            this.health += hp;
            
            if (this.health > maxhp){ this.health = maxhp; }
        }
    }

    public void Pause(bool p){
        this.paused = p;
    }
}

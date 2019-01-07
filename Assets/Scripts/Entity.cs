using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour {
    
    public float health;
    public float maxhp;
    public bool can_damage = true;
    public bool can_heal = true;
    private bool iframes_enabled = false;

    private float iframes_starttime = 0.0f;
    public float iframes_duration = 1.0f;
    public float iframes_minalpha = 0.2f;

    public SpriteRenderer sr;
    public PolygonCollider2D coll;
    public GameObject damage_particle_system;

    void Start(){
    }

    void Update(){
        if (iframes_enabled){
            if (Time.time - this.iframes_starttime > this.iframes_duration){
                this.iframes_enabled = false;
                
                Color tmp = this.sr.color;
                tmp.a = 1.0f;
                this.sr.color = tmp;

                this.coll.enabled = true;
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

                Color tmp = this.sr.color;
                tmp.a = iframes_minalpha;
                this.sr.color = tmp;

                this.coll.enabled = false;
            }
        }
    }
    
    public void Heal(float hp){
        if (can_heal){
            this.health += hp;
            
            if (this.health > maxhp){ this.health = maxhp; }
        }
    }
}

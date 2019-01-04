using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // structure to hold state information about the player
    struct PlayerState{
        public bool thrust_enabled;
        public bool turn_enabled;
    }

    private PlayerState state;

    public GameObject exhaust;
    private ParticleSystem particle_system;
    private ParticleSystem.EmissionModule particle_emission;

    private Rigidbody2D rb2D;

    private float thrust_force = 100.0f;
    private float rotation_speed = 5.0f;

    private float max_velocity = 8.0f;
    
    private Vector2 forward;

    // Start is called before the first frame update
    void Start()
    {
        // start off being able to move
        this.state.thrust_enabled = true;
        this.state.turn_enabled = true;

        this.forward = new Vector2(0, 0);

        this.exhaust.SetActive(false);

        this.rb2D = GetComponent<Rigidbody2D>();
        
        // TODO - broken
        //this.particle_system = GetComponent<ParticleSystem>();
        //this.particle_emission = particle_system.emission;
        //this.particle_emission.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
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

                this.rb2D.AddForce(this.forward * this.thrust_force);

                //if (!this.particle_emission.enabled){
                //    this.particle_emission.enabled = true;
                //}
            } else {
                if (this.exhaust.activeSelf){
                    this.exhaust.SetActive(false);
                    
                    //this.particle_emission.enabled = false;
                }
            }

            if (ax3 < -0.1){
                this.rb2D.AddForce(-this.forward * this.thrust_force / 2);
            }
        }
        
        // clamp velocity to below max velocity
        if (this.rb2D.velocity.magnitude > this.max_velocity){
            this.rb2D.AddForce((this.max_velocity - this.rb2D.velocity.magnitude) * this.rb2D.velocity * this.thrust_force);
        }

    }
}

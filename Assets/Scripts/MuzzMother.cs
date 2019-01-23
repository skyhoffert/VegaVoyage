using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzMother : MonoBehaviour
{

    private bool spawn_enabled = true;

    private float spawn_timer_max = 2.5f;
    private float spawn_timer = 0.0f;

    private float detect_distance = 12.0f;

    public GameObject muzz;

    private GameObject player;

    void Start() {
        this.player = GameObject.FindWithTag("Player");

        this.spawn_timer = this.spawn_timer_max;
    }

    void Update() {
        if (this.spawn_enabled){
            if (this.spawn_timer <= 0){
                float distance = (this.player.transform.position - this.transform.position).magnitude;

                if (distance < this.detect_distance){
                    GameObject g = Instantiate(muzz, this.transform.position, this.transform.rotation) as GameObject;
                    this.spawn_timer = this.spawn_timer_max;
                }
            } else {
                this.spawn_timer -= Time.deltaTime;
            }
        }
    }

    void Die(){
        
    }
}

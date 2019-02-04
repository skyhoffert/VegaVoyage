using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{

    private float increase = 0.06f;

    private float start_time = 0.0f;

    private float increase_duration = 3.0f;
    private float alpha_duration = 0.5f;

    private int state = 0;

    private SpriteRenderer sr;

    private bool allow_skipping = true;

    void Start(){
        this.start_time = Time.time;
        this.sr = GetComponent<SpriteRenderer>();
        
        Color tmp = this.sr.color;
        tmp.a = 0;
        this.sr.color = tmp;
    }

    // Update is called once per frame
    void Update(){
        float elapsed = Time.time - this.start_time;

        if (this.state == 0){
            if (elapsed >= this.alpha_duration){
                this.state = 1;

                Color tmp = this.sr.color;
                tmp.a = 1.0f;
                this.sr.color = tmp;
            } else {
                Color tmp = this.sr.color;
                tmp.a = elapsed / this.alpha_duration;
                this.sr.color = tmp;
            }
        } else if (this.state == 1){
            if (elapsed < this.increase_duration + this.alpha_duration){
                this.transform.localScale += new Vector3(this.increase * Time.deltaTime, this.increase * Time.deltaTime, 0);
            } else {
                this.state = 2;
            }
        } else {
            if (elapsed < 2 * this.alpha_duration + this.increase_duration){
                Color tmp = this.sr.color;
                tmp.a = 1- (elapsed - (this.alpha_duration + this.increase_duration)) / this.alpha_duration;
                this.sr.color = tmp;
            } else {
                this.state = 3;
                SceneManager.LoadScene("MainMenu");
            }
        }

        if (this.allow_skipping){
            if (Input.anyKeyDown){
                this.increase_duration = 0.0f;
            }
        }
    }
}

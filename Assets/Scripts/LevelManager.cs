using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{

    private bool pause_enabled = true;

    public GameObject pause_menu;
    public Text txt_0;
    public Text txt_1;

    private GameObject player;
    private GameObject cam;

    public GameObject blackout_sr;

    public GameObject spawn_0;
    public GameObject spawn_1;

    private bool paused = false;

    private float prev_tm;
    private int num_updates;

    private int pause_cursor_pos = 0;

    private float down_thresh = -0.8f;
    private float up_thresh = 0.8f;
    
    private float max_rgb = 0.6f;
    private float min_rgb = 0.0f;

    private bool debouncing_dpad = true;
    private bool debouncing_lstick = true;

    private const bool DEBUGGING = false;

    // Start is called before the first frame update
    void Start(){
        int playerid = LayerMask.NameToLayer("GameSpace");
        int enemies = LayerMask.NameToLayer("EnemySpace");
        int walls = LayerMask.NameToLayer("FarFG");
        int pieces = LayerMask.NameToLayer("NearBG");
        Physics2D.IgnoreLayerCollision(enemies, enemies, true);
        Physics2D.IgnoreLayerCollision(pieces, playerid, true);
        Physics2D.IgnoreLayerCollision(pieces, enemies, true);
        Physics2D.IgnoreLayerCollision(pieces, pieces, true);

        this.prev_tm = Time.time;

        this.player = GameObject.FindWithTag("Player");
        this.cam = GameObject.FindWithTag("MainCamera");

        // move the player to the spawn position
        if (PlayerPrefs.GetInt("spawn_area") == 0){
            this.player.transform.position = this.spawn_0.transform.position;
            this.player.transform.rotation = this.spawn_0.transform.rotation;
        } else if (PlayerPrefs.GetInt("spawn_area") == 1){
            this.player.transform.position = this.spawn_1.transform.position;
            this.player.transform.rotation = this.spawn_1.transform.rotation;
        }

        // tell player to clear trail
        this.player.SendMessage("ClearTrail");

        // move the camera over the player so it doesn't have to lerp there
        this.cam.transform.position = new Vector3(this.player.transform.position.x, this.player.transform.position.y, this.cam.transform.position.z);

        this.pause_menu.SetActive(false);
    }

    // Update is called once per frame
    void Update(){
        if (this.paused){
            if (PauseKeyPressed()){
                TogglePause();
            } else if (DownKeyPressed() || UpKeyPressed()){
                this.pause_cursor_pos = (this.pause_cursor_pos + 1) % 2;
                HandlePauseCursor();
            } else if (GoKeyPressed()){
                if (this.pause_cursor_pos == 0){
                    TogglePause();
                } else if (this.pause_cursor_pos == 1){
                    TogglePause();
                    SceneManager.LoadScene("MainMenu");
                }
            }
        } else {
            // input handling
            if (QuitKeyPressed()){
                TogglePause();
            } else if (ResetKeyPressed()){
                // load current scene name that has been saved
                string scene_name = PlayerPrefs.GetString("current_scene", "");
                SceneManager.LoadScene(scene_name);
            } else if (PauseKeyPressed()){
                TogglePause();
            }
        }

        // frame rate handling
        if (Time.time - this.prev_tm >= 1.0){
            if (DEBUGGING){
                //Debug.Log(this.num_updates);
            }
            this.prev_tm = Time.time;
            this.num_updates = 0;
        } else {
            this.num_updates++;
        }

        if (this.debouncing_dpad){
            if (Input.GetAxis("Vertical") < this.up_thresh && Input.GetAxis("Vertical") > this.down_thresh){
                if (Input.GetAxis("Horizontal") < this.up_thresh && Input.GetAxis("Horizontal") > this.down_thresh){
                    this.debouncing_dpad = false;
                }
            }
        }
        
        if (this.debouncing_lstick){
            if (Input.GetAxis("axy") < this.up_thresh && Input.GetAxis("axy") > this.down_thresh){
                if (Input.GetAxis("axx") < this.up_thresh && Input.GetAxis("axx") > this.down_thresh){
                    this.debouncing_lstick = false;
                }
            }
        }
    }

    private void TogglePause(){
        if (this.pause_enabled){
            if (Time.timeScale < 0.1){
                Time.timeScale = 1.0f;
                this.paused = false;
                blackout_sr.SetActive(false);
            } else {
                Time.timeScale = 0.0f;
                this.paused = true;
                blackout_sr.SetActive(true);
            }

            GameObject.FindWithTag("Player").SendMessage("Pause", this.paused);

            GameObject[] sintzers = GameObject.FindGameObjectsWithTag("Sintzer");
            for (int i = 0; i < sintzers.Length; i++){
                sintzers[i].SendMessage("Pause", this.paused);
            }
            
            GameObject[] muzz = GameObject.FindGameObjectsWithTag("Muzz");
            for (int i = 0; i < muzz.Length; i++){
                muzz[i].SendMessage("Pause", this.paused);
            }
            
            GameObject[] caulpers = GameObject.FindGameObjectsWithTag("Caulper");
            for (int i = 0; i < caulpers.Length; i++){
                caulpers[i].SendMessage("Pause", this.paused);
            }
            
            this.pause_menu.SetActive(this.paused);

            Color c = this.txt_0.color;
            c.r = this.min_rgb;
            c.g = this.min_rgb;
            c.b = this.min_rgb;
            this.txt_0.color = c;
            this.txt_1.color = c;

            HandlePauseCursor();
        }
    }

    private void HandlePauseCursor(){
        Color c = this.txt_0.color;
        c.r = this.min_rgb;
        c.g = this.min_rgb;
        c.b = this.min_rgb;
        this.txt_0.color = c;
        this.txt_1.color = c;

        if (this.pause_cursor_pos == 0){
            c.r = this.max_rgb;
            c.g = this.max_rgb;
            c.b = this.max_rgb;
            this.txt_0.color = c;
        } else if (this.pause_cursor_pos == 1){
            c.r = this.max_rgb;
            c.g = this.max_rgb;
            c.b = this.max_rgb;
            this.txt_1.color = c;
        }
    }

    bool QuitKeyPressed(){
        return Input.GetKeyDown(KeyCode.Escape);
    }

    bool ResetKeyPressed(){
        return Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetButtonDown("b6");
    }

    bool PauseKeyPressed(){
        return Input.GetButtonDown("b7");
    }
    
    bool GoKeyPressed(){
        return Input.GetButtonDown("b0") || Input.GetKeyDown(KeyCode.Return);
    }

    bool DownKeyPressed(){
        if (!this.debouncing_dpad && (Input.GetAxis("Vertical") < this.down_thresh || Input.GetAxis("Horizontal") > this.up_thresh)){
            this.debouncing_dpad = true;
            return true;
        }
        if (!this.debouncing_lstick && (Input.GetAxis("axy") > this.up_thresh || Input.GetAxis("axx") > this.up_thresh)){
            this.debouncing_lstick = true;
            return true;
        }
        return Input.GetKeyDown(KeyCode.DownArrow);
    }
    
    bool UpKeyPressed(){
        if (!this.debouncing_dpad && (Input.GetAxis("Vertical") > this.up_thresh || Input.GetAxis("Horizontal") < this.down_thresh)){
            this.debouncing_dpad = true;
            return true;
        }
        if (!this.debouncing_lstick && (Input.GetAxis("axy") < this.down_thresh || Input.GetAxis("axx") < this.down_thresh)){
            this.debouncing_lstick = true;
            return true;
        }
        return Input.GetKeyDown(KeyCode.UpArrow);
    }
}

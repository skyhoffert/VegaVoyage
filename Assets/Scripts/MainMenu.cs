using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Text txt_0;
    public Text txt_1;
    public Text txt_2;
    public Text txt_3;

    public GameObject newgame_menu;
    public Text newgame_0;
    public Text newgame_1;

    public GameObject exhausts;
    public GameObject center_of_scout_ships;

    public Image blackout;

    private int newgame_cursor_pos = 1;

    private int cursor_pos = 0;

    private float max_alpha = 1.0f;
    private float min_alpha = 0.6f;

    private float max_rgb = 0.6f;
    private float min_rgb = 0.0f;

    private float down_thresh = -0.8f;
    private float up_thresh = 0.8f;

    private bool has_game = true;

    private string continue_scene;

    private bool main_input = true;
    private bool newgame_input = false;
    private bool newgame_pressed = false;

    private float newgame_pressed_tm = 0.0f;
    private float exhaust_startup_duration = 1.0f;
    private float newgame_pressed_duration = 2.5f;
    private float stf = 5.0f;

    private bool debouncing_dpad = false;
    private bool debouncing_lstick = false;

    void Start(){
        //PlayerPrefs.SetString("current_scene", "GameStartArea");
        //PlayerPrefs.SetInt("spawn_area", 0);

        this.continue_scene = PlayerPrefs.GetString("current_scene", "");

        if (this.continue_scene == ""){
            this.cursor_pos = 1;
            this.has_game = false;

            Color c1 = this.txt_0.color;
            c1.a = 0.0f;
            this.txt_0.color = c1;
        } else {
            this.center_of_scout_ships.SetActive(false);
        }

        this.newgame_menu.SetActive(false);
        Color c2 = this.newgame_1.color;
        c2.r = this.max_rgb;
        c2.g = this.max_rgb;
        c2.b = this.max_rgb;
        this.newgame_1.color = c2;

        HandleSelection();

        this.exhausts.SetActive(false);
    }

    void Update(){
        if (this.main_input){
            // handle the "go" key, whatever it may be
            if (GoKeyPressed()){
                if (this.cursor_pos == 0){
                    SceneManager.LoadScene(this.continue_scene);
                } else if (this.cursor_pos == 1){
                    if (!this.has_game){
                        PlayerPrefs.DeleteAll();
                        this.newgame_pressed = true;
                        this.main_input = false;
                        this.newgame_input = false;
                        this.exhausts.SetActive(true);
                    } else {
                        this.newgame_menu.SetActive(true);
                        this.main_input = false;
                        this.newgame_input = true;
                    }
                } else if (this.cursor_pos == 2){
                    
                } else if (this.cursor_pos == 3){
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else
                        Application.Quit();
                    #endif
                }
            }

            // handle navigational input
            if (DownKeyPressed()){
                if (this.has_game){
                    this.cursor_pos = (this.cursor_pos + 1) % 4;
                } else {
                    this.cursor_pos = (this.cursor_pos % 3) + 1;
                }
                HandleSelection();
            }
            if (UpKeyPressed()){
                if (this.has_game){
                    this.cursor_pos = (4 + this.cursor_pos - 1) % 4;
                } else {
                    this.cursor_pos--;
                    if (this.cursor_pos == 0){ this.cursor_pos = 3; }
                }
                HandleSelection();
            }
        } else if (this.newgame_input){
            if (GoKeyPressed()){
                if (this.newgame_cursor_pos == 0){
                    PlayerPrefs.DeleteAll();
                    this.center_of_scout_ships.SetActive(true);
                    this.newgame_pressed = true;
                    this.main_input = false;
                    this.newgame_input = false;
                    this.exhausts.SetActive(true);
                    this.newgame_menu.SetActive(false);
                } else if (this.newgame_cursor_pos == 1){
                    this.main_input = true;
                    this.newgame_input = false;
                    this.newgame_menu.SetActive(false);
                    this.newgame_cursor_pos = 1;
                }
            }
            
            if (DownKeyPressed() || UpKeyPressed()){
                this.newgame_cursor_pos = (this.newgame_cursor_pos + 1) % 2;
                HandleSelection();
            }
        } else if (this.newgame_pressed){
            this.newgame_pressed_tm += Time.deltaTime;
            if (this.newgame_pressed_tm < this.exhaust_startup_duration){
            } else if (this.newgame_pressed_tm < this.exhaust_startup_duration + this.newgame_pressed_duration){
                this.center_of_scout_ships.transform.Translate(Time.deltaTime * this.stf, 0, 0);
            } else {
                SceneManager.LoadScene("Cutscene_Intro");
            }

            Color c = this.blackout.color;
            c.a = Mathf.Pow(this.newgame_pressed_tm / (this.newgame_pressed_duration + this.exhaust_startup_duration), 2);
            this.blackout.color = c;
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
    
    void HandleSelection(){
        if (this.main_input){
            Color c1 = this.txt_0.color;
            c1.a = this.min_alpha;
            this.txt_1.color = c1;
            this.txt_2.color = c1;
            this.txt_3.color = c1;

            if (this.has_game){
                this.txt_0.color = c1;
            }

            if (this.cursor_pos == 0){
                Color c = this.txt_0.color;
                c.a = this.max_alpha;
                this.txt_0.color = c;
            } else if (this.cursor_pos == 1){
                Color c = this.txt_1.color;
                c.a = this.max_alpha;
                this.txt_1.color = c;
            } else if (this.cursor_pos == 2){
                Color c = this.txt_2.color;
                c.a = this.max_alpha;
                this.txt_2.color = c;
            } else if (this.cursor_pos == 3){
                Color c = this.txt_3.color;
                c.a = this.max_alpha;
                this.txt_3.color = c;
            }
        } else if (this.newgame_input){
            Color c2 = this.newgame_0.color;
            c2.r = this.min_rgb;
            c2.g = this.min_rgb;
            c2.b = this.min_rgb;
            this.newgame_0.color = c2;
            this.newgame_1.color = c2;

            if (this.newgame_cursor_pos == 0){
                Color c3 = this.newgame_0.color;
                c3.r = this.max_rgb;
                c3.g = this.max_rgb;
                c3.b = this.max_rgb;
                this.newgame_0.color = c3;
            } else if (this.newgame_cursor_pos == 1){
                Color c3 = this.newgame_1.color;
                c3.r = this.max_rgb;
                c3.g = this.max_rgb;
                c3.b = this.max_rgb;
                this.newgame_1.color = c3;
            }
        }
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

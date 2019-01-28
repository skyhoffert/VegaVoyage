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

    private int newgame_cursor_pos = 1;

    private int cursor_pos = 0;

    private float max_alpha = 1.0f;
    private float min_alpha = 0.6f;

    private float max_rgb = 0.6f;
    private float min_rgb = 0.0f;

    private bool has_game = true;

    private string continue_scene;

    private bool main_input = true;
    private bool newgame_input = false;

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
        }

        this.newgame_menu.SetActive(false);
        Color c2 = this.newgame_1.color;
        c2.r = this.max_rgb;
        c2.g = this.max_rgb;
        c2.b = this.max_rgb;
        this.newgame_1.color = c2;

        HandleSelection();
    }

    void Update(){
        if (this.main_input){
            // handle the "go" key, whatever it may be
            if (PauseKeyPressed()){
                if (this.cursor_pos == 0){
                    SceneManager.LoadScene(this.continue_scene);
                } else if (this.cursor_pos == 1){
                    this.newgame_menu.SetActive(true);
                    this.main_input = false;
                    this.newgame_input = true;
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
            if (PauseKeyPressed()){
                if (this.newgame_cursor_pos == 0){
                    PlayerPrefs.SetString("current_scene", "GameStartArea");
                    SceneManager.LoadScene("GameStartArea");
                } else if (this.newgame_cursor_pos == 1){
                    this.main_input = true;
                    this.newgame_input = false;
                    this.newgame_menu.SetActive(false);
                    this.newgame_cursor_pos = 1;
                }
            }
            
            if (DownKeyPressed() || UpKeyPressed()){
                this.newgame_cursor_pos = (this.newgame_cursor_pos + 1) % 2;
                Debug.Log(this.newgame_cursor_pos);
                HandleSelection();
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

    bool PauseKeyPressed(){
        return Input.GetButtonDown("b7") || Input.GetKeyDown(KeyCode.Return);
    }

    bool DownKeyPressed(){
        return Input.GetKeyDown(KeyCode.DownArrow);
    }
    
    bool UpKeyPressed(){
        return Input.GetKeyDown(KeyCode.UpArrow);
    }
}

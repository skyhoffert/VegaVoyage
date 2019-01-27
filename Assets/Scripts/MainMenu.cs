using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Text start_txt;
    public Text options_txt;
    public Text credits_txt;
    public Text exit_txt;

    private int cursor_pos = 0;

    private float max_alpha = 1.0f;
    private float min_alpha = 0.6f;

    void Start(){
        PlayerPrefs.SetString("current_scene", "GameStartArea");
        PlayerPrefs.SetInt("spawn_area", 0);

        HandleSelection();
    }

    void Update(){
        if (PauseKeyPressed()){
            if (this.cursor_pos == 0){
                SceneManager.LoadScene("GameStartArea");
            } else if (this.cursor_pos == 1){

            } else if (this.cursor_pos == 2){
                
            } else if (this.cursor_pos == 3){
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }
        if (DownKeyPressed()){
            this.cursor_pos = (this.cursor_pos + 1) % 4;
            HandleSelection();
        }
        if (UpKeyPressed()){
            this.cursor_pos = (4 + this.cursor_pos - 1) % 4;
            HandleSelection();
        }
    }
    
    void HandleSelection(){
        Color c1 = this.start_txt.color;
        c1.a = this.min_alpha;
        this.start_txt.color = c1;
        this.options_txt.color = c1;
        this.credits_txt.color = c1;
        this.exit_txt.color = c1;

        if (this.cursor_pos == 0){
            Color c = this.start_txt.color;
            c.a = this.max_alpha;
            this.start_txt.color = c;
        } else if (this.cursor_pos == 1){
            Color c = this.options_txt.color;
            c.a = this.max_alpha;
            this.options_txt.color = c;
        } else if (this.cursor_pos == 2){
            Color c = this.credits_txt.color;
            c.a = this.max_alpha;
            this.credits_txt.color = c;
        } else if (this.cursor_pos == 3){
            Color c = this.exit_txt.color;
            c.a = this.max_alpha;
            this.exit_txt.color = c;
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

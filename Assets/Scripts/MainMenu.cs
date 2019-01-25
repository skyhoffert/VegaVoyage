using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start(){
        PlayerPrefs.SetString("current_scene", "GameStartArea");
        PlayerPrefs.SetInt("spawn_area", 0);
    }

    void Update(){
        if (PauseKeyPressed()){
            SceneManager.LoadScene("GameStartArea");
        }
    }
    
    bool PauseKeyPressed(){
        return Input.GetButtonDown("b7");
    }
}

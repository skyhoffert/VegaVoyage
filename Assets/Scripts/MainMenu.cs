using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Start(){
        
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

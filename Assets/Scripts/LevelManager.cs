using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    // Start is called before the first frame update
    void Start(){
        int layer = LayerMask.NameToLayer("EnemySpace");
        Physics2D.IgnoreLayerCollision(layer, layer, true);
    }

    // Update is called once per frame
    void Update(){
        if (QuitKeyPressed()){
            Application.Quit();
        } else if (ResetKeyPressed()){
            SceneManager.LoadScene("Test");
        } else if (PauseKeyPressed()){
            if (Time.timeScale < 0.1){
                Time.timeScale = 1.0f;
            } else {
                Time.timeScale = 0.0f;
            }
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
}

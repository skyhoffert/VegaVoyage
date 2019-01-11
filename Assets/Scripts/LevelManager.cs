using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private float prev_tm;
    private int num_updates;

    // Start is called before the first frame update
    void Start(){
        int layer = LayerMask.NameToLayer("EnemySpace");
        Physics2D.IgnoreLayerCollision(layer, layer, true);

        this.prev_tm = Time.time;
    }

    // Update is called once per frame
    void Update(){
        // input handling
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

        // frame rate handling
        if (Time.time - this.prev_tm >= 1.0){
            Debug.Log(this.num_updates);
            this.prev_tm = Time.time;
            this.num_updates = 0;
        } else {
            this.num_updates++;
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

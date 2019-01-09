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
        if (Input.GetKeyDown(KeyCode.Escape)){
            // kill game
            Application.Quit();
        } else if (Input.GetKeyDown(KeyCode.LeftBracket)){
            // reset level
            SceneManager.LoadScene("Test");
        }
    }
}

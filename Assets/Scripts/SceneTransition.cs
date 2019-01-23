using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{

    public string scene_name = "Test";
    
    void OnTriggerEnter2D(Collider2D collision){
        SceneManager.LoadScene(this.scene_name);
    }

}

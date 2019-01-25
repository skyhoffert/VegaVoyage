using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{

    public string scene_name = "Test";
    public int spawn_index = 0;
    
    void OnTriggerEnter2D(Collider2D collision){
        PlayerPrefs.SetString("current_scene", this.scene_name);
        PlayerPrefs.SetInt("spawn_area", this.spawn_index);
        SceneManager.LoadScene(this.scene_name);
    }

}

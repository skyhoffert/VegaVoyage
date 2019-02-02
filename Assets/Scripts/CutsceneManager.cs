using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{

    public bool allow_skipping = true;

    public string next_scene_name = "GameStartArea";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.allow_skipping){
            if (GoKeyPressed()){
                SceneManager.LoadScene(this.next_scene_name);
                PlayerPrefs.SetString("current_scene", this.next_scene_name);
            }
        }   
    }

    bool GoKeyPressed(){
        return Input.GetButtonDown("b0") || Input.GetKeyDown(KeyCode.Return);
    }
}

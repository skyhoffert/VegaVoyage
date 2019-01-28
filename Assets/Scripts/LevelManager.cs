using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private bool pause_enabled = true;

    private GameObject player;
    private GameObject cam;

    public GameObject blackout_sr;

    public GameObject spawn_0;
    public GameObject spawn_1;

    private bool paused = false;

    private float prev_tm;
    private int num_updates;

    private const bool DEBUGGING = false;

    // Start is called before the first frame update
    void Start(){
        int playerid = LayerMask.NameToLayer("GameSpace");
        int enemies = LayerMask.NameToLayer("EnemySpace");
        int walls = LayerMask.NameToLayer("FarFG");
        int pieces = LayerMask.NameToLayer("NearBG");
        Physics2D.IgnoreLayerCollision(enemies, enemies, true);
        Physics2D.IgnoreLayerCollision(pieces, playerid, true);
        Physics2D.IgnoreLayerCollision(pieces, enemies, true);
        Physics2D.IgnoreLayerCollision(pieces, pieces, true);

        this.prev_tm = Time.time;

        this.player = GameObject.FindWithTag("Player");
        this.cam = GameObject.FindWithTag("MainCamera");

        // move the player to the spawn position
        if (PlayerPrefs.GetInt("spawn_area") == 0){
            this.player.transform.position = this.spawn_0.transform.position;
            this.player.transform.rotation = this.spawn_0.transform.rotation;
        } else if (PlayerPrefs.GetInt("spawn_area") == 1){
            this.player.transform.position = this.spawn_1.transform.position;
            this.player.transform.rotation = this.spawn_1.transform.rotation;
        }

        // tell player to clear trail
        this.player.SendMessage("ClearTrail");

        // move the camera over the player so it doesn't have to lerp there
        this.cam.transform.position = new Vector3(this.player.transform.position.x, this.player.transform.position.y, this.cam.transform.position.z);
    }

    // Update is called once per frame
    void Update(){
        // input handling
        if (QuitKeyPressed()){
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        } else if (ResetKeyPressed()){
            // load current scene name that has been saved
            string scene_name = PlayerPrefs.GetString("current_scene", "");
            SceneManager.LoadScene(scene_name);
        } else if (PauseKeyPressed()){
            if (this.pause_enabled){
                if (Time.timeScale < 0.1){
                    Time.timeScale = 1.0f;
                    this.paused = false;
                    blackout_sr.SetActive(false);
                } else {
                    Time.timeScale = 0.0f;
                    this.paused = true;
                    blackout_sr.SetActive(true);
                }

                GameObject.FindWithTag("Player").SendMessage("Pause", this.paused);

                GameObject[] sintzers = GameObject.FindGameObjectsWithTag("Sintzer");
                for (int i = 0; i < sintzers.Length; i++){
                    sintzers[i].SendMessage("Pause", this.paused);
                }
                
                GameObject[] muzz = GameObject.FindGameObjectsWithTag("Muzz");
                for (int i = 0; i < muzz.Length; i++){
                    muzz[i].SendMessage("Pause", this.paused);
                }
                
                GameObject[] caulpers = GameObject.FindGameObjectsWithTag("Caulper");
                for (int i = 0; i < caulpers.Length; i++){
                    caulpers[i].SendMessage("Pause", this.paused);
                }
            }
        }

        // frame rate handling
        if (Time.time - this.prev_tm >= 1.0){
            if (DEBUGGING){
                //Debug.Log(this.num_updates);
            }
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

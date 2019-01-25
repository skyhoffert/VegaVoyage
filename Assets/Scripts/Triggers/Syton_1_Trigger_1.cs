using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syton_1_Trigger_1 : MonoBehaviour
{

    public GameObject ship_exterior;
    
    void OnTriggerEnter2D(Collider2D collision){
        this.ship_exterior.SetActive(false);
    }
    
    void OnTriggerExit2D(Collider2D collision){
        this.ship_exterior.SetActive(true);
    }
}

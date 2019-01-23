using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syton_2_Trigg_1 : MonoBehaviour
{
    public GameObject muzz_wall;
    
    void OnTriggerEnter2D(Collider2D collision){
        Destroy(muzz_wall);
    }
}

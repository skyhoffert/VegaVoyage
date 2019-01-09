using System.Collections;
using System.Collections.Generic;
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
        
    }
}

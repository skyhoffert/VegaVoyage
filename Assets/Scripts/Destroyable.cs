using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{

    public float health = 10.0f;

    public GameObject destroy_particles;

    // Start is called before the first frame update
    void Start(){
        
    }

    void ApplyRawDamage(float dmg){
        this.health -= dmg;

        if (this.health < 0){
            GameObject g = Instantiate(this.destroy_particles, this.transform.position, this.transform.rotation);

            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour
{

    public float health = 10.0f;

    public GameObject destroy_particles;

    // 0 is upgrade, TODO
    public int currency_type = 0;
    public int currency_amount = 1;

    private GameObject player;

    // Start is called before the first frame update
    void Start(){
        this.player = GameObject.FindWithTag("Player");
    }

    void ApplyRawDamage(float dmg){
        this.health -= dmg;

        if (this.health < 0){
            GameObject g = Instantiate(this.destroy_particles, this.transform.position, this.transform.rotation);

            Destroy(this.gameObject);

            if (this.currency_type == 0){
                this.player.SendMessage("AddUpgradeCurrency", this.currency_amount);
            }
        }
    }
}

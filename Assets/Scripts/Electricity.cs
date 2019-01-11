using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    private float collision_damage = 10.2f;

    void OnTriggerEnter2D(Collider2D collision){
        collision.gameObject.SendMessage("ApplyRawDamage", this.collision_damage);
    }

    void OnTriggerStay2D(Collider2D collision){
        collision.gameObject.SendMessage("ApplyRawDamage", this.collision_damage);
    }
}

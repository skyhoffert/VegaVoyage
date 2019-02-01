using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowlyRotate : MonoBehaviour
{

    private float rot_amount = 18.0f;

    // Update is called once per frame
    void Update(){
        this.transform.Rotate(Vector3.up * Time.deltaTime * this.rot_amount);
    }

}

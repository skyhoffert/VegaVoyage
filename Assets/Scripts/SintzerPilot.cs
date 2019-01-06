using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SintzerPilot : MonoBehaviour
{

    private Vector2 forward;
    private float rotation_speed = 5.0f;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float y_dist = player.transform.position.y - this.transform.position.y;
        float x_dist = player.transform.position.x - this.transform.position.x;

        // calculate forward direction
        forward.x = Mathf.Sin(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
        forward.y = -Mathf.Cos(this.transform.rotation.eulerAngles.z * Mathf.Deg2Rad + Mathf.PI/2);
        forward.Normalize();
        
        // calculate how much to turn
        float rotation_amount = Mathf.Atan2(y_dist, x_dist) * Mathf.Rad2Deg + Mathf.PI/2;
        
        // Lerp towards that direction
        Vector3 dir = new Vector3(0.0f, 0.0f, rotation_amount);
        Quaternion target = Quaternion.Euler(dir);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, target, Time.deltaTime * this.rotation_speed);

        Debug.Log(this.forward);
    }
}

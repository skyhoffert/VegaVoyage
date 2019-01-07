using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDuration : MonoBehaviour
{
    public float duration = 1.0f;

    private float start_time;

    // Start is called before the first frame update
    void Start()
    {
        this.start_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= this.start_time + this.duration){
            Destroy(this.gameObject);
        }
    }
}

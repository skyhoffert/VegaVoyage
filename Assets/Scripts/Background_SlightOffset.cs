using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_SlightOffset : MonoBehaviour
{

    private float ratio = 0.98f;

    // Update is called once per frame
    void Update()
    {
        Vector3 target = new Vector3(transform.parent.transform.position.x * this.ratio, transform.parent.transform.position.y * this.ratio, transform.position.z);

        transform.position = target;
    }
}

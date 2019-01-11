using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public GameObject objectToFollow;

    private float speed = 0.08f;
	
	// Update is called once per frame
	void FixedUpdate () {
        if (objectToFollow){
            Vector3 target = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, this.transform.position.z);

            this.transform.position = Vector3.Lerp(this.transform.position, target, speed);
        }
	}

}

using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public GameObject objectToFollow;

    private float speed = 0.06f;
	
	// Update is called once per frame
	void FixedUpdate () {
        if (objectToFollow){
            /* 1
            //float interpolation = speed * Time.deltaTime;
            
            //Vector3 position = this.transform.position;
            //position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y, speed);
            //position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, speed);
            
            //this.transform.position = position;
            /* */

            /* 2 */
            Vector3 target = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, this.transform.position.z);

            this.transform.position = Vector3.Lerp(this.transform.position, target, speed);
            /* */

            /* 3
            Vector3 target = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, this.transform.position.z);

            transform.position -= (transform.position - target) * movespeed * Time.deltaTime;
            /* */
        }
	}

}

using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public GameObject objectToFollow;

    private float speed = 5.0f;
	
	// Update is called once per frame
	void Update () {
        if (objectToFollow){
            float interpolation = speed * Time.deltaTime;
            
            Vector3 position = this.transform.position;
            position.y = Mathf.Lerp(this.transform.position.y, objectToFollow.transform.position.y, interpolation);
            position.x = Mathf.Lerp(this.transform.position.x, objectToFollow.transform.position.x, interpolation);
            
            this.transform.position = position;
        }
	}

}

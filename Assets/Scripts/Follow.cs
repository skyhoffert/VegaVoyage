using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour {

    public GameObject objectToFollow;

    private float speed = 0.08f;

    private float zoom_initial = 6.0f;
    private float zoom_factor = 0.5f;

    private Camera cam;

    void Start(){
        this.cam = GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (objectToFollow){
            Vector3 target = new Vector3(objectToFollow.transform.position.x, objectToFollow.transform.position.y, this.transform.position.z);
            float dist = (target - this.transform.position).magnitude;

            this.transform.position = Vector3.Lerp(this.transform.position, target, speed);

            this.cam.orthographicSize = zoom_initial + dist * zoom_factor;
        }
	}

}

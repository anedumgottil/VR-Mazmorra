using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollZ : MonoBehaviour {

    public float scrollspeed = 15;
    //public int counter = 0;

	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        Vector3 localVectorUp = transform.TransformDirection(0, 1, 0);
        pos += localVectorUp * scrollspeed * Time.deltaTime;
        transform.position = pos;
        //counter++;
	}
}

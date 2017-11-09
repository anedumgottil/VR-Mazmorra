using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: set this to stationary...
public class Technoshard : MobileEntity {

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains ("Controller")) {
            Destroy (this.gameObject);
        }
        //SetCollidingObject(other);
    }
}

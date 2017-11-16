using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: set this to stationary...
public class Technoshard : StationaryEntity {

    public Technoshard(GridSpace gs) : base(gs) {
        //new Technoshard
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains ("Controller")) {
            this.setGridSpace (null);
            Destroy (this.gameObject);
        }
        //SetCollidingObject(other);
    }
}

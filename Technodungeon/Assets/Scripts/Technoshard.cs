using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//TODO: set this to stationary...
public class Technoshard : StationaryEntity {
    public AudioSource gemNoise;
    public Technoshard(GridSpace gs) : base(gs) {
        //new Technoshard
        gemNoise = GetComponent<AudioSource>();
    }

    public void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.name.Contains ("Controller")) {
            gemNoise.Play();
            this.setGridSpace (null);
            Destroy (this.gameObject);
        }
        //SetCollidingObject(other);
    }
}

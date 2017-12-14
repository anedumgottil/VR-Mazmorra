using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
//TODO: set this to stationary...
public class Technoshard : StationaryEntity {
    public AudioSource gemNoise;
    public AudioClip gemClip;
    private VRTK_InteractableObject myobj = null;
    public Technoshard(GridSpace gs) : base(gs) {
        //new Technoshard
        gemNoise = GetComponent<AudioSource>();
    }

    public void OnTriggerEnter(Collider other)
    {
        /*
        if (other.gameObject.name.Contains ("Controller")) {
            gemNoise.Play();
            this.setGridSpace (null);
            Destroy (this.gameObject);
        }*/
        //SetCollidingObject(other);
    }

//TODO: please implement touch-destroy for Technoshard.

    public void Start() {
        myobj = this.gameObject.GetComponent<VRTK_InteractableObject> ();
    }
}

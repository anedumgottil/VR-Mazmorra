using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Grenade : MobileEntity {

    public AudioClip grenadeBeep = null;
    public AudioClip grabNoise = null;
    public AudioClip ungrabNoise = null;
    public AudioClip grenadeExplosion = null;
    private static WaitForSeconds wait1s;
    private AudioSource audioSource = null;
    public GameObject deathParticleSystem;
    public Light grenadeLight = null;
   
   

	// Use this for initialization
	void Start () {
        audioSource = this.gameObject.GetComponent<AudioSource> ();
        wait1s = new WaitForSeconds (1f);
        this.setHealth (10);//TODO: why is this necessary???
	}

    public void startTimer() {
        audioSource.PlayOneShot (grenadeBeep);
        StartCoroutine(grenadeTimer ());
    }

    public override void die() {
        this.alive = false; 

        audioSource.PlayOneShot (grenadeExplosion);
        Renderer rend = this.GetComponentInChildren<Renderer> ();
        rend.material.color = Color.black;

        Block newBlock = MapLoader.getBlockInstance (0);
        newBlock.setPosition (this.gameObject.transform.position);
        newBlock.setColor (Color.white);
        newBlock.addTechnofog ();

        if (deathParticleSystem != null) {
            deathParticleSystem.SetActive (true);
        }

        Destroy (this.gameObject, 5f);
    }

    IEnumerator grenadeTimer()
    {
        while (isAlive ()) {
            yield return wait1s;
            this.damage (this.gameObject, 2);
            Debug.Log ("Health: " + this.getHealth ());
            grenadeLight.enabled = !grenadeLight.enabled;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Grenade : MobileEntity {

    public AudioClip grenadeBeep = null;
    public AudioClip grabNoise = null;
    public AudioClip ungrabNoise = null;
    public AudioClip grenadeExplosion = null;
    private static WaitForSeconds wait5s;
    private AudioSource audioSource = null;
    public GameObject deathParticleSystem;
   
   

	// Use this for initialization
	void Start () {
        audioSource = this.gameObject.GetComponent<AudioSource> ();

	}

    public void startTimer() {
        audioSource.PlayOneShot (grenadeBeep);
        grenadeTimer ();
    }

    public override void die() {
        this.alive = false; 

        audioSource.PlayOneShot (grenadeExplosion);
        Renderer rend = this.GetComponent<Renderer> ();
        rend.material.color = Color.black;

        Block newBlock = MapLoader.getBlockInstance (0);
        newBlock.setPosition (this.gameObject.transform.position);
        newBlock.setColor (Color.white);

        if (deathParticleSystem != null) {
            deathParticleSystem.SetActive (true);
        }

        Destroy (this.gameObject, 5f);
    }

    IEnumerator grenadeTimer()
    {

        yield return wait5s;
        this.die ();

    }
}

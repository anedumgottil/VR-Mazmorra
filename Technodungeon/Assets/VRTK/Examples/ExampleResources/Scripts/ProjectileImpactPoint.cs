using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is used to produce noises and particles for a bullet impact. We can't use the bullet since bullets get deleted on impact. Now they will produce this Prefab, which will do the good stuff for us, then delete itself.
//This could also be used to render bullet holes.
public class ProjectileImpactPoint : MonoBehaviour {
    public AudioClip[] impactNoises;
    public float pitchModRange = 0.05f;

    public AudioSource audioSource = null;
    public ParticleSystem particleSys = null;


	// Use this for initialization
    void Start () {
        if (audioSource == null)
            audioSource = this.gameObject.GetComponent<AudioSource> ();
        if (particleSys == null)
            particleSys = this.gameObject.GetComponent<ParticleSystem> ();
    }


    //makes the impact point do its thing, then die.
    public void triggerImpactPoint() {
        spitParticles ();
        playSound ();
        Destroy (this.gameObject, 30f);
    }

    private void spitParticles() {
        this.particleSys.Play ();
    }

    private void playSound() {
        if (!audioSource.isActiveAndEnabled) {
            audioSource.enabled = true;
        }
        audioSource.pitch += ((float) (Random.Range (0,11)*(pitchModRange*2))) + (-pitchModRange);//tweak pitch from range (0.05 to -0.05)
        if (impactNoises != null && impactNoises.Length > 0) {
            int selectedsound = Random.Range (0, impactNoises.Length);
            audioSource.PlayOneShot (impactNoises[selectedsound]);
        }
    }

}

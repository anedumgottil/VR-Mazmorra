using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is used to produce noises and particles for a bullet impact. We can't use the bullet since bullets get deleted on impact. Now they will produce this Prefab, which will do the good stuff for us, then delete itself.
//This could also be used to render bullet holes.
public class ProjectileImpactPoint : MonoBehaviour {
    public AudioClip[] metalImpactClips;
    public AudioClip[] fleshImpactClips;
    public float pitchModRange = 0.05f;

    public enum ImpactType : byte {
        NONE = 0, METAL, FLESH };

    public AudioSource audioSource = null;
    public ParticleSystem particleSys = null;

    private ImpactType impactType = ImpactType.NONE;


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
        playSound (impactType);
        Destroy (this.gameObject, 30f);
    }

    private void spitParticles() {
        this.particleSys.Play ();
    }

    private void playSound(ImpactType type) {
        if (!audioSource.isActiveAndEnabled) {
            audioSource.enabled = true;
        }
        audioSource.pitch += ((float) (Random.Range (0,11)*(pitchModRange*2))) + (-pitchModRange);//tweak pitch from range (0.05 to -0.05)
        switch (type) {
        case ImpactType.NONE:
            return;
        case ImpactType.METAL:
            if (metalImpactClips != null && metalImpactClips.Length > 0) {
                int selectedsound = Random.Range (0, metalImpactClips.Length);
                audioSource.PlayOneShot (metalImpactClips [selectedsound]);
            }
            break;
        case ImpactType.FLESH:
            if (fleshImpactClips != null && fleshImpactClips.Length > 0) {
                int selectedsound = Random.Range (0, fleshImpactClips.Length);
                audioSource.PlayOneShot (fleshImpactClips [selectedsound]);
            }
            break;
        default:
            Debug.LogWarning ("ProjectileImpactPoint: playSound: specified unknown ImpactType");
            break;
        }
        pitchModRange = 1.0f;
    }

    public void setImpactType(ImpactType type) {
        this.impactType = type;
    }

}

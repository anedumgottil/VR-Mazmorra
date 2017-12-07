using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoolantLine : StationaryEntity {

    public AudioSource coolantMainAudioSource = null;
    public AudioClip steamClip = null;
    public GameObject grabbablePlugEnd = null;
    public Renderer coreRenderer = null;
    public Material criticalMaterial = null;

    public GameObject ventParticles = null;
    public GameObject chimneyParticles = null;

    private bool isCriticalState = false;

    public CoolantLine(GridSpace gs) : base(gs) {
        //empty constructor
    }

    // Use this for initialization
    void Start () {
        alive = false; 
        if (grabbablePlugEnd == null) {
            Debug.LogError ("CoolantLine: Was not given Grabbable Plug End! Cannot function!");
            return;
        }
        if (coreRenderer == null) {
            Debug.LogError ("CoolantLine: Was not given core Renderer! Cannot function!");
            return;
        }
        if (coolantMainAudioSource == null) {
            Debug.LogError ("CoolantLine: Was not given main audio source! Cannot function!");
            return;
        }
        alive = true;
    }

    public void goCritical() {
        isCriticalState = true;
        if (criticalMaterial != null) {
            coreRenderer.material = criticalMaterial;
        }
        if (ventParticles != null) {
            ventParticles.SetActive (true);
        }
        if (chimneyParticles != null) {
            chimneyParticles.SetActive (true);
            Destroy (chimneyParticles, 10f); 
        }
        coolantMainAudioSource.Stop ();
        if (steamClip != null) {
            coolantMainAudioSource.volume = 0.3f;
            coolantMainAudioSource.PlayOneShot (steamClip);
        }
    }

    public bool isCritical() {
        return isCriticalState;
    }
}

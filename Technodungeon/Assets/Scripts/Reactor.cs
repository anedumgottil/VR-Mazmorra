using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : StationaryEntity {

    public GameObject grabbablePlugEnd = null;
    public Renderer coreRenderer = null;
    public Material criticalMaterial = null;

    public GameObject ventParticles = null;
    public GameObject coreParticles = null;
    public float coreParticlesDuration = 1f;

    private float lastReactorBurstTime = 0.0f;
    private float nextReactorBurstWaitTime = 3f;
    private bool isCriticalState = false;

    public Reactor(GridSpace gs) : base(gs) {
    }

    void Update () {
        if (isCriticalState && alive) {
            if (lastReactorBurstTime + nextReactorBurstWaitTime <= Time.time && !coreParticles.activeSelf) {
                lastReactorBurstTime = Time.time;
                nextReactorBurstWaitTime = (float)Random.Range (1, 6);

                //cause reactor burst effect
                coreParticles.SetActive (true);
            } else if ((lastReactorBurstTime + coreParticlesDuration) <= Time.time && coreParticles.activeSelf) {
                //stop reactor burst effect
                coreParticles.SetActive (false);
            }
        }
    }

	// Use this for initialization
	void Start () {
        alive = false; 
        if (grabbablePlugEnd == null) {
            Debug.LogError ("Reactor: Was not given Grabbable Plug End! Cannot function!");
            return;
        }
        if (coreRenderer == null) {
            Debug.LogError ("Reactor: Was not given core Renderer! Cannot function!");
            return;
        }
        if (ventParticles == null) {
            Debug.LogError ("Reactor: Was not given ventParticles! Cannot function!");
            return;
        }
        if (coreParticles == null) {
            Debug.LogError ("Reactor: Was not given coreParticles! Cannot function!");
            return;
        }
        alive = true;
	}

    public void goCritical() {
        isCriticalState = true;
        if (criticalMaterial != null) {
            coreRenderer.material = criticalMaterial;
        }
        ventParticles.SetActive (true);
    }

    public bool isCritical() {
        return isCriticalState;
    }
}

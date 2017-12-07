using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GrabbablePlugEnd : VRTK_InteractableObject {

    public Reactor reactorScript = null;
    public CoolantLine coolantScript = null;
    public GameObject plugParticleEffect = null;

    void Start() {
        if (plugParticleEffect == null) {
            Debug.LogError ("Grabbable Plug End not given a plugParticleEffect!");
        }
    }

    void OnJointBreak() {
        if (reactorScript != null) {
            reactorScript.goCritical ();
        }
        if (coolantScript != null) {
            coolantScript.goCritical ();
        }
        plugParticleEffect.SetActive (true);
    }
}

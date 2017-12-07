using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GrabbablePlugEnd : VRTK_InteractableObject {

    public Reactor reactorScript = null;
    public GameObject plugParticleEffect = null;

    void Start() {
        if (reactorScript == null) {
            Debug.LogError ("Grabbable Plug End not given a ReactorScript!");
        }
        if (plugParticleEffect == null) {
            Debug.LogError ("Grabbable Plug End not given a plugParticleEffect!");
        }
    }

    void OnJointBreak() {
        reactorScript.goCritical ();
        plugParticleEffect.SetActive (true);
    }
}

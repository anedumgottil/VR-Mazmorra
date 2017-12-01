using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class GrenadeInteractableObject : VRTK_InteractableObject {


    private Grenade grenade;

    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);
        grenade.startTimer();
    }

	// Use this for initialization
	void Start () {
        grenade = this.gameObject.GetComponent<Grenade> ();
	}
	
}

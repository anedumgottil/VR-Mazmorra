using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for a controller that uses the Touchpad as a Dpad to teleport the player rig around.
public class PlayerMovementController : DPadController {

    protected override void doRight() {
        //Debug.Log ("Right Touchpad Press");
        Player.getInstance().transform.Translate (new Vector3(-1,0,0));
    }
    protected override void doLeft() {
        //Debug.Log ("Left Touchpad Press");
        Player.getInstance().transform.Translate (new Vector3(1,0,0));
    }
    protected override void doUp() {
        //Debug.Log ("Up Touchpad Press");
        Player.getInstance().transform.Translate (new Vector3(0,0,1));
    }
    protected override void doDown() {
        //Debug.Log ("Down Touchpad Press");
        Player.getInstance().transform.Translate (new Vector3(0,0,-1));
    }

    protected override void doCenter() {
        //Debug.Log ("Touchpad Deadzone Press");
    }

}

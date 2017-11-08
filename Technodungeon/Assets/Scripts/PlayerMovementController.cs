using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for a controller that uses the Touchpad as a Dpad to teleport the player rig around.
public class PlayerMovementController : DPadController {

    protected override void doRight() {
        //Debug.Log ("Right Touchpad Press");
        Player.getInstance().teleportRight();
    }
    protected override void doLeft() {
        //Debug.Log ("Left Touchpad Press");
        Player.getInstance().teleportLeft();
    }
    protected override void doUp() {
        //Debug.Log ("Up Touchpad Press");
        Player.getInstance().teleportForward();
    }
    protected override void doDown() {
        //Debug.Log ("Down Touchpad Press");
        Player.getInstance().teleportBack();
    }

    protected override void doCenter() {
        //Debug.Log ("Touchpad Deadzone Press");
    }

}

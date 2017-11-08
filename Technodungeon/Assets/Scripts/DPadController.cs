using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//Base class for the DPad-type Controller in-game. Uses VRTK Events to detect controller Touchpad input, and maps it to four axes. Can be extended.
public class DPadController : MonoBehaviour {
    private Vector2 touchAxis = Vector2.zero;

    private ControllerInteractionEventHandler axisChange;
    private ControllerInteractionEventHandler touchEnd;
    private ControllerInteractionEventHandler touchPress;

    public float threshold = 0.3f;//deadzone threshold

    private void OnEnable()
    {
        axisChange = new ControllerInteractionEventHandler (DoTouchpadAxisChanged);
        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += axisChange;
        touchEnd = new ControllerInteractionEventHandler (DoTouchpadTouchEnd);
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += touchEnd;

        touchPress = new ControllerInteractionEventHandler (DoTouchpadPressed); 
        GetComponent<VRTK_ControllerEvents> ().TouchpadPressed += touchPress;
    }

    private void OnDisable()
    {
        GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged -= axisChange;
        GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd -= touchEnd;

        GetComponent<VRTK_ControllerEvents> ().TouchpadPressed -= touchPress;
    }

    private void DoTouchpadPressed(object sender, ControllerInteractionEventArgs e) {
        if (this.yAxisIsDeadzone () && this.xAxisIsDeadzone ()) {
            this.doCenter ();
            return;
        }

        if (!this.yAxisIsDeadzone () && this.xAxisIsDeadzone ()) {
            //y-axis press
            if (touchAxis.y >= (1f - threshold)) {
                this.doUp ();
            } else if (touchAxis.y <= (-1f + threshold)) {
                this.doDown ();
            } else {
                Debug.LogError ("touchAxis not pressed either way within vertical non-deadzone condition");
            }
        } else if (this.yAxisIsDeadzone () && !this.xAxisIsDeadzone ()) {
            //x-axis press
            if (touchAxis.x >= (1f - threshold)) {
                this.doRight ();
            } else if (touchAxis.x <= (-1f + threshold)) {
                this.doLeft ();
            } else {
                Debug.LogError ("touchAxis not pressed either way within horizontal non-deadzone condition");
            }
        }
    }

    private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
        this.touchAxis = e.touchpadAxis;
    }

    private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
    {
        this.touchAxis = Vector2.zero;
    }

    protected virtual void doRight() {
        //Debug.Log ("Right Touchpad Press");
    }
    protected virtual void doLeft() {
        //Debug.Log ("Left Touchpad Press");
    }
    protected virtual void doUp() {
        //Debug.Log ("Up Touchpad Press");
    }
    protected virtual void doDown() {
        //Debug.Log ("Down Touchpad Press");
    }

    protected virtual void doCenter() {
        //Debug.Log ("Touchpad Deadzone Press");
    }

    //returns true if the touchAxis y-axis is within the set threshold deadzone
    protected bool yAxisIsDeadzone() {
        if (touchAxis.y <= (1f - threshold) && touchAxis.y >= (-1f + threshold)) {
            return true;
        }
        return false;
    }

    //returns true if the touchAxis x-axis is within the set threshold deadzone
    protected bool xAxisIsDeadzone() {
        if (touchAxis.x <= (1f - threshold) && touchAxis.x >= (-1f + threshold)) {
            return true;
        }
        return false;
    }
}

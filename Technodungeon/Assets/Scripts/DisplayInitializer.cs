using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInitializer : MonoBehaviour {

    static bool activated = false;
    static int twoDDisplay = -1;

	// Use this for initialization
	void Start () {
        Debug.Log ("Displays connected: " + Display.displays.Length);
        if (Display.displays.Length > 1) {
            //Display.displays [1].Activate ();
        }
        twoDDisplay = 0;

        #if UNITY_EDITOR
        //we're in the unity editor, set everything to use the second display to get the GUI text off of the game window.
        Camera TwoDCam = this.gameObject.GetComponent<Camera>();
        TwoDCam.targetDisplay = 1;//Display 2
        Canvas TwoDCanvas = this.gameObject.GetComponentInChildren<Canvas>();
        TwoDCanvas.targetDisplay = 1;
        twoDDisplay = 1;
        #endif

        activated = true;
	}

    public static bool isActivated() {
        return activated;
    }

    public static int getTwoDDisplay() {
        return twoDDisplay;
    }
}

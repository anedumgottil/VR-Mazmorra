using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIUIManager : MonoBehaviour {

    public float updateRate = 0.5f;

    public bool droneAvailable = true;
    public bool treadbotAvailable = true;
    public bool turretAvailable = false;

    public static bool droneSelected = false;
    public static bool treadbotSelected = false;
    public static bool turretSelected = false;

    public int lowEnergyThreshold = 0;
    public Color lowEnergyWarningColor;

    bool shouldUpdate = false;
    private WaitForSeconds waitASecond;

    public Text energyLevel = null;
    public Text selectedUnit = null;
    public GameObject droneUnitGroup = null;
    public GameObject treadbotUnitGroup = null;
    public GameObject turretUnitGroup = null;
    public AIPlayer aiPlayer = null;

    Color defaultEnergyColor;

	// Use this for initialization
	void Start () {
        if (energyLevel == null) {
            Debug.LogError ("AIUIManager: could not find energyLevel!");
        }
        if (selectedUnit == null) {
            Debug.LogError ("AIUIManager: could not find selectedUnit!");
        }
        if (droneUnitGroup == null) {
            Debug.LogError ("AIUIManager: could not find droneUnitGroup!");
        }
        if (treadbotUnitGroup == null) {
            Debug.LogError ("AIUIManager: could not find treadbotUnitGroup!");
        }
        if (turretUnitGroup == null) {
            Debug.LogError ("AIUIManager: could not find turretUnitGroup!");
        }
        if (aiPlayer == null) {
            Debug.LogError ("AIUIManager: could not find aiPlayer!");
        }

        waitASecond = new WaitForSeconds (updateRate);
        shouldUpdate = true;
        StartCoroutine (updateUI ());

        defaultEnergyColor = energyLevel.color;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void droneButtonSelected() {
        if (droneAvailable) {
            droneSelected = true;
            treadbotSelected = false;
            turretSelected = false;
        }

        //perform action
    }

    public void treadbotButtonSelected() {
        if (treadbotAvailable) {
            
            droneSelected = false;
            treadbotSelected = true;
            turretSelected = false;
        }

        //perform action
    }

    public void turretButtonSelected() {
        if (turretAvailable) {
            droneSelected = false;
            treadbotSelected = false;
            turretSelected = true;
        }

        //perform action
    }

    IEnumerator updateUI() {

        while (Player.getInstance () != null && shouldUpdate) {
            if (droneSelected) {
                selectedUnit.text = "Drone";
            } else if (treadbotSelected) {
                selectedUnit.text = "TreadBot";
            } else if (turretSelected) {
                selectedUnit.text = "Turret";
            }

            if (droneAvailable && !droneUnitGroup.activeSelf) {
                droneUnitGroup.SetActive (true);
            } else if (!droneAvailable && droneUnitGroup.activeSelf) {
                droneUnitGroup.SetActive (false);
            }
            if (turretAvailable && !turretUnitGroup.activeSelf) {
                turretUnitGroup.SetActive (true);
            } else if (!turretAvailable && turretUnitGroup.activeSelf) {
                turretUnitGroup.SetActive (false);
            }
            if (treadbotAvailable && !treadbotUnitGroup.activeSelf) {
                treadbotUnitGroup.SetActive (true);
            } else if (!treadbotAvailable && treadbotUnitGroup.activeSelf) {
                treadbotUnitGroup.SetActive (false);
            }
                
            //TODO: this color stuff is called every UI update, even when it doesn't need to tween since we're already tweened. not sure if it's worth making an edge trigger with state here.
            energyLevel.text = aiPlayer.getEnergy ().ToString ();
            if (aiPlayer.getEnergy () < lowEnergyThreshold) {
                energyLevel.CrossFadeColor (lowEnergyWarningColor, 0.5f, false, false);
            } else {
                energyLevel.CrossFadeColor (defaultEnergyColor, 0.5f, false, false);
            }



            yield return waitASecond;

        }
    }

    public static string getSelection() {
        if (droneSelected) {
            return "Drone";
        }
        if (treadbotSelected) {
            return "TreadBot";
        }
        if (turretSelected) {
            return "Turret";
        }
        return "None";
    }
}

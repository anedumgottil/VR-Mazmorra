using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIUIManager : MonoBehaviour {

    public bool droneAvailable = true;
    public bool treadbotAvailable = true;
    public bool turretAvailable = false;

    public bool droneSelected = false;
    public bool treadbotSelected = false;
    public bool turretSelected = false;

    bool shouldUpdate = false;
    private WaitForSeconds waitASecond;

    public Text energyLevel = null;
    public Text selectedUnit = null;
    public GameObject droneUnitGroup = null;
    public GameObject treadbotUnitGroup = null;
    public GameObject turretUnitGroup = null;

	// Use this for initialization
	void Start () {
        if (energyLevel == null) {
            Debug.LogError ("AIUIManager: could not find button required!");
        }
        if (selectedUnit == null) {
            Debug.LogError ("AIUIManager: could not find button required!");
        }
        if (droneUnitGroup == null) {
            Debug.LogError ("AIUIManager: could not find button required!");
        }
        if (treadbotUnitGroup == null) {
            Debug.LogError ("AIUIManager: could not find button required!");
        }
        if (turretUnitGroup == null) {
            Debug.LogError ("AIUIManager: could not find button required!");
        }

        waitASecond = new WaitForSeconds (1f);
        shouldUpdate = true;
        StartCoroutine (updateUI ());
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

            yield return waitASecond;

        }
    }
}

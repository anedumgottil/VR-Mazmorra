using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : MonoBehaviour {

    public int energy = 100;
    public int reactorsAvailable = 0;
    public int reactorsConnected = 0;
    public float energyRegenerationRate = 5.0f;
    public float energyRegenerationMultiplyer = 1.0f;

    private WaitForSeconds waitASecond;

    void Start() {
        
        waitASecond = new WaitForSeconds (1f);
        StartCoroutine (updateUI ());
    }

    public void Update() {
        
    }

    public bool spendEnergy(int energyCost) {
        if (energyCost >= energy) {
            return false;
        }
        energy -= energyCost;
        return true;
    }

    public int getEnergy() {

        return energy;
    }

    IEnumerator updateUI() {

        while (Player.getInstance () != null) {
            energy += (int) ((energyRegenerationMultiplyer * reactorsConnected + 1) + energyRegenerationRate);

            yield return waitASecond;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class allows you to do player related classes, like move him/her around the map or access his items which we might need to add in the future
public class Player : MobileEntity {
    private static Player instance = null;//now with 100% more singleton!

    public GameObject rController;
    public GameObject lController;

    void OnEnable() {

        Debug.Log ("Player exists");
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Player.
            Destroy (gameObject); 
    }


    public static Player getInstance() {
        return instance;
    }
}

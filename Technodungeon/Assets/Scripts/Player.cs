using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class allows you to do player related classes, like move him/her around the map or access his items which we might need to add in the future
public class Player : MonoBehaviour {
    private static Player instance = null;//now with 100% more singleton!

    private int gridPosX = -1;
    private int gridPosY = -1;

    void Awake() {
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Grid.
            Destroy (gameObject); 
    }

    //translates the SteamVR Rig entirely to a specified Grid Coordinate
    public void teleportToGridCoords(int x, int y) {
        this.gameObject.transform.Translate (getWorldCoordsFromGridCoords (x, y), Space.World);
        this.gridPosX = x;
        this.gridPosY = y;
    }


    //scales the grid coords accordingly to present world coords according to the Grid transform scale setting
    //WARNING this is broken right now because Grid scale does not inherit to it's objects... s it's useless until we can scale the map up
    public Vector3 getWorldCoordsFromGridCoords(int x, int y) {
        /*Vector3 scale = Grid.getInstance().gameObject.transform.scale;
        return new Vector3 (x * scale.x, this.gameObject.transform.position.y, y * scale.z);*/
        return new Vector3 (x, this.gameObject.transform.position.y, y);
    }


    public static Player getInstance() {
        return instance;
    }
}

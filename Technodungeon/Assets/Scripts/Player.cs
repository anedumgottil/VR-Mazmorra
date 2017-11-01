using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class allows you to do player related classes, like move him/her around the map or access his items which we might need to add in the future
public class Player : MonoBehaviour {
    private static Player instance = null;//now with 100% more singleton!

    private int gridPosX = -1;
    private int gridPosY = -1;

    public float clickThresh = 0.3f;//30% of trackpad distance
    public GameObject rController;
    public GameObject lController;

    private SteamVR_TrackedController rightController;
    private SteamVR_TrackedController leftController;

    private void OnEnable()
    {
        rightController = rController.gameObject.GetComponent<SteamVR_TrackedController>(); 
        leftController = lController.gameObject.GetComponent<SteamVR_TrackedController>();
        rightController.PadClicked += HandleRightPadClicked;
        leftController.PadClicked += HandleLeftPadClicked;
    }

    private void OnDisable()
    {
        rightController.PadClicked -= HandleRightPadClicked;
        leftController.PadClicked -= HandleLeftPadClicked;
    }

    private void HandleRightPadClicked(object sender, ClickedEventArgs e)
    {
        if (e.padX < -1 + clickThresh && (e.padY > -1 + clickThresh && e.padY < 1 - clickThresh)) {
            //left pad click
        } else if (e.padX > 1 - clickThresh && (e.padY > -1 + clickThresh && e.padY < 1 - clickThresh)) {
            //right pad click
        } else if (e.padY < -1 + clickThresh && (e.padX > -1 + clickThresh && e.padX < 1 - clickThresh)) {
            //down pad click
        } else if (e.padY > 1 - clickThresh && (e.padX > -1 + clickThresh && e.padX < 1 - clickThresh)) {
            //up pad click
        }
            
    }

    private void HandleLeftPadClicked(object sender, ClickedEventArgs e)
    {
        if (e.padX < -1 + clickThresh && (e.padY > -1 + clickThresh && e.padY < 1 - clickThresh)) {
            //left pad click
            teleportToGridCoords(gridPosX-1, gridPosY);
        } else if (e.padX > 1 - clickThresh && (e.padY > -1 + clickThresh && e.padY < 1 - clickThresh)) {
            //right pad click
            teleportToGridCoords(gridPosX+1, gridPosY);
        } else if (e.padY < -1 + clickThresh && (e.padX > -1 + clickThresh && e.padX < 1 - clickThresh)) {
            //down pad click
            teleportToGridCoords(gridPosX, gridPosY-1);
        } else if (e.padY > 1 - clickThresh && (e.padX > -1 + clickThresh && e.padX < 1 - clickThresh)) {
            //up pad click
            teleportToGridCoords(gridPosX, gridPosY+1);
        }

    }

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

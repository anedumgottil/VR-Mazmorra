using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

//this class allows you to do player related classes, like move him/her around the map or access his items which we might need to add in the future
public class Player : MobileEntity {
    private static Player instance = null;//now with 100% more singleton!

    private int gridX = -1;//the current grid position of the player
    private int gridY = -1;
    private bool isInit = false;//if false, do nothing.

    private Transform head = null;
    private VRTK_BasicTeleport teleporter = null;

    public GameObject rController = null;//VRTK Right Controller Alias
    public GameObject lController = null;//VRTK Left Controller Alias
    public GameObject playArea = null;//VRTK PlayArea Alias GameObject.

    void OnEnable() {
        //Check if instance already exists
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Player.
            Debug.Log("current Player GameObject: "+this.gameObject.ToString ());
            Debug.Log("old Player GameObject: "+instance.gameObject.ToString ()+" at pos: "+instance.gameObject.transform.position.ToString ()+" named: "+instance.gameObject.name+" as: "+instance.ToString ());
            Debug.LogError ("Player: Destroying multiple instance of Player Object! Do not re-instantiate Player!");
            Destroy (gameObject); 
        }
    }

    public void initialize() {
        head = VRTK_DeviceFinder.HeadsetTransform ();
        if (playArea != null) {
            teleporter = playArea.GetComponent<VRTK_BasicTeleport> ();
            if (teleporter == null) {
                Debug.LogError ("Player: unable to find teleporter!");
                return;
            }
        } else {
            Debug.LogError ("Player: unable to find play area!");
            return;
        }
        if (head == null) {
            Debug.LogError ("Player: unable to find the headset!");
            return;
        }
        isInit = true;
        Debug.Log ("Player initialized");
    }

    //teleport to the grid location, but only if there exists a GridSpace there. Update our gridX and Y accordingly.
    public override void teleportToGridLocation(int x, int y){
        if (this.gameObject == null || !isInit)
            return;
        if (MapGrid.getInstance ().getGridSpace (x, y) != null) {
            Vector3 worldGridPos = MapGrid.getInstance ().getWorldCoordsFromGridCoords (x, y);
            //adjust the x,z coord by MapGrid.getSize()/2 to correctly position the rig in the GridSpace... otherwise it is not aligned right, probably has to do with the transform location relative to play area
            //TODO: this needs to have dynamic adjustment based on the scaling factor of our PlaySpace to GridSpace ratio... it needs to have edges of playspace calculated to lock to the edges of GridSpace tiles. not this terrible pos crap
            worldGridPos.x += MapGrid.getSize()/2;
            worldGridPos.z += MapGrid.getSize()/2;
            teleporter.ForceTeleport (worldGridPos);
            this.gridX = x;
            this.gridY = y;
        }
    }

    //moves 1 GridSpace forward, according to the current look direction
    public void teleportForward() {
        if (this.isInit) {
            Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (head.forward);
            teleportToGridLocation (gridX + forward.x, gridY + forward.y);
        }
    }
    //moves 1 GridSpace left, according to the current look direction
    public void teleportLeft() {
        if (this.isInit) {
            Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (-head.right);
            teleportToGridLocation (gridX + forward.x, gridY + forward.y);
        }
    }
    //moves 1 GridSpace right, according to the current look direction
    public void teleportRight() {
        if (this.isInit) {
            Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (head.right);
            teleportToGridLocation (gridX + forward.x, gridY + forward.y);
        }
    }
    //moves 1 GridSpace back, according to the current look direction
    public void teleportBack() {
        if (this.isInit) {
            Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (-head.forward);
            teleportToGridLocation (gridX + forward.x, gridY + forward.y);
        }
    }

    public Vector2Int getGridLocation() {
        return new Vector2Int(gridX, gridY);
    }

    public Transform getHead() {
        return head;
    }

    public static Player getInstance() {
        return instance;
    }

    public bool isInitialized() {
        return this.isInit;
    }
}
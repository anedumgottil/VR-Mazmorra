using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class allows you to do player related classes, like move him/her around the map or access his items which we might need to add in the future
public class Player : MobileEntity {
    private static Player instance = null;//now with 100% more singleton!

    private int gridX = -1;//the current grid position of the player
    private int gridY = -1;

    private GameObject head = null;

    public GameObject rController;
    public GameObject lController;

    void OnEnable() {
        Debug.Log ("Player exists");
        Camera headCam = this.gameObject.GetComponentInChildren<Camera> ();
        if (headCam != null) {
            head = headCam.transform.gameObject;
        } else {
            Debug.LogError ("Error: Player - unable to find the head camera");
        }
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a Player.
            Destroy (gameObject); 
    }

    //teleport to the grid location, but only if there exists a GridSpace there. Update our gridX and Y accordingly.
    public override void teleportToGridLocation(int x, int y){
        if (this.gameObject == null)
            return;
        if (MapGrid.getInstance ().getGridSpace (x, y) != null) {
            Vector3 worldGridPos = MapGrid.getInstance ().getWorldCoordsFromGridCoords (x, y);
            //adjust the x,z coord by MapGrid.getSize()/2 to correctly position the rig in the GridSpace... otherwise it is not aligned right, probably has to do with the transform location relative to play area
            //TODO: this needs to have dynamic adjustment based on the scaling factor of our PlaySpace to GridSpace ratio... it needs to have edges of playspace calculated to lock to the edges of GridSpace tiles. not this terrible pos crap
            worldGridPos.x += MapGrid.getSize()/2;
            worldGridPos.z += MapGrid.getSize()/2;
            this.gameObject.transform.position = worldGridPos;
            this.gridX = x;
            this.gridY = y;
        }
    }

    //moves 1 GridSpace forward, according to the current look direction
    public void teleportForward() {
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (head.transform.forward);
        teleportToGridLocation (gridX + forward.x, gridY + forward.y);
    }
    //moves 1 GridSpace left, according to the current look direction
    public void teleportLeft() {
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (-head.transform.right);
        teleportToGridLocation (gridX + forward.x, gridY + forward.y);
    }
    //moves 1 GridSpace right, according to the current look direction
    public void teleportRight() {
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (head.transform.right);
        teleportToGridLocation (gridX + forward.x, gridY + forward.y);
    }
    //moves 1 GridSpace back, according to the current look direction
    public void teleportBack() {
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (-head.transform.forward);
        teleportToGridLocation (gridX + forward.x, gridY + forward.y);
    }

    public Vector2Int getGridLocation() {
        return new Vector2Int(gridX, gridY);
    }

    public GameObject getHead() {
        return head;
    }

    public static Player getInstance() {
        return instance;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wrapper Parent class for Tile and Block objects
//Could be referenced by 2D Gridview when that is complete.
public class GridObject {

    protected static int gridObjectCount = 0;
    private int gridObjectID;
    protected Vector3 position; //position relative to grid origin
    private static float gridObjectSize = Grid.getSize()/2;//meters
    private GridSpace parent;
    protected GameObject gameObj;//this is either an actual gameObject parent for a Block or Tile if it was just generated, OR a prefab, if it was cloned
    public static readonly Vector3 DEFAULT_POSITION = new Vector3 (-5, -5, -5);

    public GridObject(GridSpace parent, Vector3 position) {
        this.gridObjectID = GridObject.gridObjectCount;//always set ID then increment GridObjectCount, this means we'll begin indexing at zero and end at GridObjectCount-1
        GridObject.gridObjectCount++;
        this.parent = parent;
        this.position = position;//set once, never change; defined by its position in the grid...
    }
        
    public GridObject(GridSpace parent) {
        //create empty object, fill in position later. 
        //Note: Please don't use a object without a position like this - it'll break stuff. It defaults to a bad location.
        //So if you do, set these values later.
        this.parent = parent;
        this.position = DEFAULT_POSITION;//store the prefab parents under the map and out of view, I don't know of an easy way to hide them yet.
        this.gridObjectID = GridObject.gridObjectCount;//always set ID then increment GridObjectCount, this means we'll begin indexing at zero and end at GridObjectCount-1
        GridObject.gridObjectCount++;
    }

    //clone constructor (inefficient deep copy because unity serialization is literally pain)
    //clone constructor sets parent to null and ID's correctly, but otherwise most everything else is copied
    // DO NOT FORGET to set parent if you use this clone constructor
    public GridObject(GridObject toClone) {
        this.gridObjectID = GridObject.gridObjectCount;
        GridObject.gridObjectCount++;
        this.parent = null;
        this.position = toClone.getPosition ();
        this.gameObj = null;//TODO: copy the gameObj here? Or in child class? we can only instantiate once...
    }
        


    public Vector3 getPosition() {
        return position;
    }
        
    public void setPosition(Vector3 position) {
        this.position = position;
        //synchronize actual game object position
        this.gameObj.transform.localPosition = position;
    }

    //returns computed worldspace position
    public Vector3 getGlobalPosition() {
        Vector3 ret = position;
        //add Grid global position to our relative-to-grid local position
        ret.x += Grid.getInstance().transform.position.x;
        ret.y += Grid.getInstance().transform.position.y;
        ret.z += Grid.getInstance().transform.position.z;
        return ret;
    }

    public void setParent(GridSpace parent) {
        this.parent = parent;
    }

    public GridSpace getParent() {
        return parent;
    }

    public GameObject getGameObj() {
        return gameObj;
    }

    public static float getSize() {
        return gridObjectSize;
    }

    public virtual int getID() {
        return gridObjectID;
    }
}

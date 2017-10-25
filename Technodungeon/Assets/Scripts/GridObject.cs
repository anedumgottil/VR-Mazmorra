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

    public GridObject(GridSpace parent, Vector3 position) {
        GridObject.gridObjectCount++;
        this.parent = parent;
        this.position = position;//set once, never change; defined by its position in the grid...
    }
        
    public GridObject(GridSpace parent) {
        //create empty object, fill in position later. 
        //Note: Please don't use a object without a position like this - it'll break stuff. It defaults to a bad location.
        //So if you do, set these values later.
        this.parent = parent;
        this.position = new Vector3(-5,-5,-5);//store the prefab parents under the map and out of view, I don't know of an easy way to hide them yet.
        GridObject.gridObjectCount++;
        this.gridObjectID = gridObjectCount;
    }


    public Vector3 getPosition() {
        return position;
    }
        
    public void setPosition(Vector3 position) {
        this.position = position;
        //synchronize actual game object position
        this.gameObj.transform.Translate (this.position);
    }

    //returns computed worldspace position
    public Vector3 getGlobalPosition() {
        Vector3 ret = position;
        //add Grid global position to our relative-to-grid local position
        ret.x += parent.getGrid().transform.position.x;
        ret.y += parent.getGrid().transform.position.y;
        ret.z += parent.getGrid().transform.position.z;
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

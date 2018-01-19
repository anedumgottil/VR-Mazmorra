using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Wrapper Parent class for Tile and Block objects
//Could be referenced by 2D Gridview when that is complete.
public class GridObject {

    protected static int gridObjectCount = 0;
    private int gridObjectID;
    protected GridObjectConfiguration gridObjectConfiguration;
    protected Vector3 position; //position relative to grid origin
    private static float gridObjectSize = MapGrid.getSize()/2;//meters
    private GridSpace parent;
    protected GameObject gameObj;//this is either an actual gameObject parent for a Block or Tile if it was just generated, OR a prefab, if it was cloned
    public static readonly Vector3 DEFAULT_POSITION = new Vector3 (-5, -5, -5);

    //These are the definitions for the possible GridObjectConfigurations previously defined by the `blocks.key.txt` file.
    public enum GridObjectConfiguration : short {
        FloorMaster = 0,        //Master Prefab for Floor-type GridObjects (contains all possible children walls, floors, etc)
        CeilingMaster = 1,      //Master Prefab for Ceiling-type GridObjects (may be identical to the 0th ID, if none exists or n/a)
        //Full-Sized GridSpace pieces:
        //Basic Floor:
        Floor = 2,              //Floor
        FloorWest = 3,          //Floor w/ West Wall
        FloorEast = 4,          //Floor w/ East Wall
        FloorSouth = 5,         //Floor w/ South Wall
        FloorNorth = 6,         //Floor w/ North Wall
        //Basic Ceiling:
        Ceiling = 7,            //Ceiling
        CeilingWest = 8,        //Ceiling w/ West Wall
        CeilingEast = 9,        //Ceiling w/ East Wall
        CeilingSouth = 10,      //Ceiling w/ South Wall
        CeilingNorth = 11,      //Ceiling w/ North Wall
        //Corners Ceiling:
        CeilingWestSouth = 12,  //Ceiling w/ West & South Walls (Corner)
        CeilingWestNorth = 13,  //Ceiling w/ West & North Walls (Corner)
        CeilingEastSouth = 14,  //Ceiling w/ East & South Walls (Corner)
        CeilingEastNorth = 15,  //Ceiling w/ East & North Walls (Corner)
        //Corners Floor:
        FloorWestSouth = 16,    //Floor w/ West & South Walls (Corner)
        FloorWestNorth = 17,    //Floor w/ West & North Walls (Corner)
        FloorEastSouth = 18,    //Floor w/ East & South Walls (Corner)
        FloorEastNorth = 19,    //Floor w/ East & North Walls (Corner)
        //Half-Sized GridSpace pieces:
        //Basic Half:
        Half = 20,              //Floor & Ceiling
        HalfWest = 21,          //Floor & Ceiling w/ West Wall
        HalfEast = 22,          //Floor & Ceiling w/ East Wall
        HalfSouth = 23,         //Floor & Ceiling w/ South Wall
        HalfNorth = 24,         //Floor & Ceiling w/ North Wall
        //Corners Half:
        HalfWestSouth = 25,     //Floor & Ceiling w/ West & South Walls (Corner)
        HalfWestNorth = 26,     //Floor & Ceiling w/ West & North Walls (Corner)
        HalfEastSouth = 27,     //Floor & Ceiling w/ East & South Walls (Corner)
        HalfEastNorth = 28      //Floor & Ceiling w/ East & North Walls (Corner)
    };
    //Each of these refer to a specified layout of walls and floors/ceilings present in a GridObject so the MapGenerator knows what to place in the map.
    //Their indexes are used and referred to by the `tilegridspaces.xml` file and the soon to be `blockgridspaces.xml` file.
    //This is now the master reference for these definitions, and as of now, all imported tile banks must support every possible GridSpace configuration.
    //TODO: These may not be super-applicable to Block-type GridObjects. I'm not sure a Block needs references to walls and floors, but it could be useful descriptor information, I suppose. May need to factor out in future.

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
        this.gameObj = null;
    }
        


    public Vector3 getPosition() {
        return position;
    }
        
    public void setPosition(Vector3 position) {
        this.position = position;
        if (gameObj != null) {
            //synchronize actual game object position
            this.gameObj.transform.localPosition = position;
        }
    }

    //returns computed worldspace position
    public Vector3 getGlobalPosition() {
        Vector3 ret = position;
        //add MapGrid global position to our relative-to-grid local position
        ret.x += MapGrid.getInstance().transform.position.x;
        ret.y += MapGrid.getInstance().transform.position.y;
        ret.z += MapGrid.getInstance().transform.position.z;
        return ret;
    }

    public void setParent(GridSpace parent) {
        this.parent = parent;
    }

    public GridSpace getParent() {
        return parent;
    }

    public GridObjectConfiguration getConfiguration() {
        return gridObjectConfiguration;
    }

    public void setConfiguration(GridObjectConfiguration goc) {
        gridObjectConfiguration = goc;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Grid Tile GridObject. This tile is used to represent a tile in the Map, along with all of it's GameObjects and properties. 
//These will be things like scenery, for example, walls, floors, lights, and other GameObjects.
//They can coexist simultaneously with Block GridObjects for extra versatility in map design/generation.

public class Tile : GridObject {
    private static int tileCount = 0;
    private int tileID;
    private static float tileSize = Grid.getSize()/2;//meters
    public const string PARENT_TILE_NAME_PREFIX = "Tile #";
    public const string CLONE_TILE_NAME_POSTFIX = " (cc)";//cloner constructor postfix

    //creates an empty tile assigned to GridSpace 'parent' at location 'position':
    public Tile(GridSpace parent, Vector3 position) : base(parent, position) {
        this.tileID = Tile.tileCount;
        Tile.tileCount++;
        gameObj = new GameObject(PARENT_TILE_NAME_PREFIX+this.tileID);//the parent GameObject used to hold all of the Tile parts
        this.gameObj.transform.Translate (position, Space.World);//TODO: is this necessary?
    }

    //creates an empty tile at the default location, assigned to GridSpace 'parent'
    public Tile(GridSpace parent) : base(parent) {
        //create empty tile, fill in position later. 
        //Note: Please don't use a Tile without a position - it'll break stuff. We assume these are set at instantiation.
        this.position = DEFAULT_POSITION;//store under the map and out of view
        this.tileID = Tile.tileCount;
        Tile.tileCount++;//always set ID then increment BlockCount, this means we'll begin indexing at zero and end at blockCount-1
        gameObj = new GameObject(PARENT_TILE_NAME_PREFIX+this.tileID);//the parent GameObject used to hold all of the Tile parts
        this.gameObj.transform.Translate (this.position, Space.World);
    }
        
    //clone constructor sets parent to null, but otherwise most everything else is copied
    // DO NOT FORGET to set parent if you use this clone constructor
    public Tile(Tile b) : base((GridObject)b) {
        if (b == null) {
            Debug.LogWarning ("Warning: Tried to create copy of null Tile");
        }
        this.position = new Vector3 (b.getPosition ().x, b.getPosition ().y, b.getPosition ().z);
        this.setParent (null);  
        this.tileID = Tile.tileCount;
        Tile.tileCount++; //even though this is a copy, we increment our ID, needs to be unique.
        this.gameObj = MonoBehaviour.Instantiate(b.getGameObj().gameObject);
        this.gameObj.name = (PARENT_TILE_NAME_PREFIX + b.getID () + CLONE_TILE_NAME_POSTFIX);
    }

    public void setGameObj(GameObject go) {
        gameObj = go;
    }

    public new static float getSize() {
        return tileSize;
    }

    public override int getID() {
        return tileID;
    }

}
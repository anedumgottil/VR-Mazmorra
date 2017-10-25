using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block {
    private static int blockCount = 0;
    private int blockID;
    private Vector3 position; //position relative to grid origin
    private static float blockSize = Grid.getSize()/2;//meters
    public const int mbDivision = 4;
    private MicroBlock[,,] mbs;
    private Grid parent;
    private GameObject gameObj;

    public Block(Grid parent, Vector3 position) {
        Block.blockCount++;
        this.blockID = blockCount;
        this.parent = parent;
        this.position = position;//set once, never change; defined by its position in the grid...
        mbs = new MicroBlock[mbDivision, mbDivision, mbDivision];
        gameObj = new GameObject("Block ["+position.x+","+position.y+","+position.z+"]");
        //gameObj.transform.Translate (this.position); TODO: CHECK THIS! Might be a bug, this may be necessary to set proper positions...
    }
    //Probably shouldn't initiate a game object in this empty constructor: 
    //it tends to spawn em at 0,0,0, because you cant have a non-value vector position
    public Block() {
        //create empty block, fill in parent and position later. 
        //Note: Please don't use a block without a position or parent - it'll break stuff. We assume these are set at instantiation.
        this.position = new Vector3(-5,-5,-5);//store the prefab parents under the map and out of view, I don't know of an easy way to hide them yet.
        Block.blockCount++;
        this.blockID = blockCount;
        mbs = new MicroBlock[mbDivision, mbDivision, mbDivision];
        gameObj = new GameObject("Block Prefab Parent");
        this.gameObj.transform.Translate (this.position);
    }


    //sets the microblock at specified position
    public void setMicroBlock(MicroBlock obj, int x, int y, int z) {
        if (x >= mbDivision || x < 0 || y >= mbDivision || y < 0 || z >= mbDivision || z < 0) {
            Debug.LogError ("tried to set a microblock that doesn't exist (out of bounds)");
            return;
        }
        mbs [x, y, z] = obj;
    }

    //gets the microblock at this position, if it exists
    //if it does not, creates a new one and registers it
    public MicroBlock getMicroBlock(int x, int y, int z) {
        if (mbs [x, y, z] == null) {
            return newMicroBlock (x, y, z);
        }
        return mbs [x, y, z];
    }

    //makes a new microblock at this position and registers it into the array
    public MicroBlock newMicroBlock(int x, int y, int z) {
        Vector3 offset = calculateMicroBlockPosOffset (x, y, z);
        //we don't have to add any relative offsets here? I think?
        mbs [x, y, z] = new MicroBlock (this, offset);
        return mbs [x, y, z];//TODO: make a temp var here so we dont have to do this array access, but i'm leaving it here to make sure the array gets generated for debug purposes
    }

    //returns a position offset for a MicroBlock relative to the origin of it's parent Block, based on it's array coordinates.
    //similar to the Block array, for the MicroBlock array we'll have Y be up, Z be forward, and X be lateral.
    //this function returns the origin position for a MicroBlock, NOT it's midpoint position/center-of-mass.
    private static Vector3 calculateMicroBlockPosOffset(int x, int y, int z) {
        return new Vector3 (x * MicroBlock.getSize (), y * MicroBlock.getSize (), z * MicroBlock.getSize ());
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
        ret.x += parent.transform.position.x;
        ret.y += parent.transform.position.y;
        ret.z += parent.transform.position.z;
        return ret;
    }

    public void setParent(Grid parent) {
        this.parent = parent;
    }

    public Grid getParent() {
        return parent;
    }

    public GameObject getGameObj() {
        return gameObj;
    }

    public static float getSize() {
        return blockSize;
    }

    public int getBlockID() {
        return blockID;
    }
}

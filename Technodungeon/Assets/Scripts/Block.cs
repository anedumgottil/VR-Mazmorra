using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Factor out the GameObj and move the prefab to the GridSpace class which will hold this GridObject
public class Block : GridObject {
    private static int blockCount = 0;
    private int blockID;
    private static float blockSize = Grid.getSize()/2;//meters
    public const int mbDivision = 4;
    private MicroBlock[,,] mbs;
    public const string PARENT_BLOCK_NAME_PREFIX = "Blockfab #";

    //generate a Block right now, creating all primitives and microblocks on the spot, at a specific position:
    //NOTE: DO NOT USE IN-GAME, INEFFICIENT, ONLY USED BY MAP PRE-GENERATION
    public Block(GridSpace parent, Vector3 position) : base(parent, position) {
        this.blockID = Block.blockCount;
        Block.blockCount++;
        mbs = new MicroBlock[mbDivision, mbDivision, mbDivision];
        gameObj = new GameObject("Generated Block ["+position.x+","+position.y+","+position.z+"]");
        Debug.LogWarning ("Warn: Deprecated: old autogeneration block constructor called, attempt to remove this if necessary");
        //this.gameObj.transform.Translate (this.position); //TODO: is this necessary?
    }
    //generate a Block right now, creating all primitives and microblocks on the spot - used for creating parents to clone from by MapLoader:
    //NOTE: DO NOT USE IN-GAME, INEFFICIENT, ONLY USED BY MAP PRE-GENERATION
    //Probably shouldn't initiate a game object in this empty constructor: 
    //it tends to spawn em at 0,0,0, because you cant have a non-value vector position
    public Block(GridSpace parent) : base(parent) {
        //create empty block, fill in position later. 
        //Note: Please don't use a block without a position - it'll break stuff. We assume these are set at instantiation.
        this.position = DEFAULT_POSITION;//store the prefab parents under the map and out of view
        this.blockID = Block.blockCount;
        Block.blockCount++;//always set ID then increment BlockCount, this means we'll begin indexing at zero and end at blockCount-1
        mbs = new MicroBlock[mbDivision, mbDivision, mbDivision];
        gameObj = new GameObject(PARENT_BLOCK_NAME_PREFIX+this.blockID);//PARENT block, used for cloning, needs an ID to make sure we don't store duplicates. (edit: this might be alleviated by serialization)
        this.gameObj.transform.Translate (this.position);
        //this is a Parent Block used to generate other blocks, so it need not be shown in the map. turn off it's renderer.
        this.gameObj.GetComponent<Renderer>().enabled = false;
    }

    //sets the microblock at specified position (in integer coordinate indexes, not worldspace positional coordinates. confusing, I know...)
    public void setMicroBlock(MicroBlock obj, int x, int y, int z) {
        if (x >= mbDivision || x < 0 || y >= mbDivision || y < 0 || z >= mbDivision || z < 0) {
            Debug.LogError ("tried to set a microblock that cannot exist (out of bounds)");
            return;
        }
        mbs [x, y, z] = obj;
    }

    //gets the microblock at this position, if it exists, o/w returns null
    public MicroBlock getMicroBlock(int x, int y, int z) {
        if (x >= mbDivision || x < 0 || y >= mbDivision || y < 0 || z >= mbDivision || z < 0) {
            Debug.LogError ("tried to get a microblock that cannot exist (out of bounds)");
            return null;
        }
        return mbs [x, y, z];
    }

    //makes a new microblock at this position and registers it into the array
    public MicroBlock newMicroBlock(int x, int y, int z) {
        if (x >= mbDivision || x < 0 || y >= mbDivision || y < 0 || z >= mbDivision || z < 0) {
            Debug.LogError ("tried to create a microblock that doesn't exist (out of bounds)");
            return null;
        }
        Vector3 offset = calculateMicroBlockPosOffset (x, y, z);
        //we don't have to add any relative offsets, as they are children to a Block and inherit it's coordinate system
        MicroBlock temp = new MicroBlock (this, offset);
        mbs [x, y, z] = temp;
        return temp;
    }

    //returns a position offset for a MicroBlock relative to the origin of it's parent Block, based on it's array coordinates.
    //similar to the Block array, for the MicroBlock array we'll have Y be up, Z be forward, and X be lateral.
    //this function returns the origin position for a MicroBlock, NOT it's midpoint position/center-of-mass.
    private static Vector3 calculateMicroBlockPosOffset(int x, int y, int z) {
        return new Vector3 (x * MicroBlock.getSize (), y * MicroBlock.getSize (), z * MicroBlock.getSize ());
    }
        
    public new static float getSize() {
        return blockSize;
    }

    public override int getID() {
        return blockID;
    }
}

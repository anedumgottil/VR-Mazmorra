using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
	
    public bool shouldRender = true;//TODO:
    public bool generateBlocksOnStart = false;//debug setting to pregenerate blocks, wastes memory
    public static int gridSize = 2;//meters
	public int xDimension = 20;
	public int yDimension = 20;
	public bool drawGizmos = true;
	public bool drawIconGizmos = true;
    public bool drawBlockGizmos = false; //gizmos will be drawn for instantiated blocks that exist in memory (thusly, only during game run)
	public Texture2D icon;
    private GameObject[,,] grid;

    public enum BlockPos : byte {LoNW = 0, LoSW = 1, LoNE = 2, LoSE = 3, HiNW = 4, HiSW = 5, HiNE = 6, HiSE = 7};//8 blocks per grid, with cardinal positions defined here
    //you'll notice: index >= 4 , guaranteed to be higher block
    //               index % 2 == 0, guaranteed to be northern block
    //               and vice versa.

    public Grid() {
        //for our 8-block array, we'll have Y be up, Z be forward, and X be lateral.
        //forward direction is northern/southern movement, lateral is east/west.
        grid = new GameObject[xDimension, yDimension, 8];
    }
	
    void Awake() {
        //some code to help debug blocks:
        if (generateBlocksOnStart) {
            pregenerateBlocks ();
        }
    }

    //sets the specified Block
    public void setBlock(Block obj, int x, int y, BlockPos bpos) {
        if (obj.getParent () == null) {
            obj.setParent (this);
        }
            Vector3 offset = calculateBlockPosOffset (bpos);
            offset.x += x;
            //it's weird, but we want the x and y dimensions of our grid to be laid over the x and z dimensions in Unity.
            offset.z += y;
        if (x >= xDimension || y >= yDimension) {
            return;//TODO: SOLVE THIS PROBLEM!
        }
            Debug.Log ("setblock offset: "+offset.ToString ());
            grid [x, y,(byte)bpos] = BlockPrefabGenerator.getPrefab(obj, offset);
	}

    /*
    //returns the specified Block, if it exists
    //if it does not exist, makes a new one and registers it to the grid <-- why?
    public Block getBlock(int x, int y, BlockPos position) {
        Block ret = grid [x, y,(byte)position];
        if (ret == null) {
            ret = newBlock (x, y, position);
        }
        return ret;
	}*/

    //makes a new Block and registers it to the grid
    //if the block already exists, does nothing
    public Block newBlock(int x, int y, BlockPos bpos) {
        if (grid [x, y,(byte)bpos] == null) {
            Vector3 offset = calculateBlockPosOffset (bpos);
            offset.x += x;
            //it's weird, but we want the x and y dimensions of our grid to be laid over the x and z dimensions in Unity.
            offset.z += y;
            Block newBlock = new Block (this, offset);
            setBlock (newBlock, x, y, bpos);
            return newBlock;
        }
        return null;
    }
        
    public static int getSize() {
        return gridSize;
    }

    //calculates the position offset of a block from its gridpoint origin based on it's position in the array
    private static Vector3 calculateBlockPosOffset(BlockPos bpos) {
        Vector3 ret = new Vector3 (0, 0, 0);
        byte adjustedPosition = (byte)bpos;
        if ((byte)bpos >= 4) {
            ret.y = Block.getSize ();
            adjustedPosition -= 4;
        } else {
            ret.y = 0;
        }

        if ((byte)bpos % 2 == 0) {
            //TODO: check that this is the proper forward Unity direction
            ret.z = Block.getSize ();

            if ((byte)adjustedPosition == 0) {
                //NW
                ret.x = 0;
            } else {
                //NE
                ret.x = Block.getSize ();
            }
        } else {
            ret.z = 0;

            if ((byte)adjustedPosition == 1) {
                //SW
                ret.x = 0;
            } else {
                //SE
                ret.x = Block.getSize ();
            }
        }

        return ret;
    }

    //loads all blocks in for testing purposes
    private void pregenerateBlocks() {
        Debug.LogWarning ("WARNING! You've left the debug setting ' Pregenerate Blocks ' on. This is only used for testing the grid block generation and storing algorithm, and is essentially useless at this point, so turn it off as it severely wastes memory.");
        for (int i = 0; i < xDimension; i++) {
            for (int j = 0; j < yDimension; j++) {
                for (int p = 0; p < 8; p++) {
                    newBlock (i, j, (BlockPos)p);
                    //Debug.Log ("Created new block at [" + i + ", " + j + ", " + p + "]: " + blk);
                }
            }
        }
    }


	void OnDrawGizmos() {
        if (drawGizmos) {
            for (int x = xDimension-1; x >= 0; x--) {
                for (int y = yDimension-1; y >= 0; y--) {
                    Func<float, float, float> dimensionPos = (float offset, float distance) => (offset + distance * gridSize) + (gridSize / 2);
                    //it's weird, but we want the x and y dimensions of our grid to be laid over the x and z dimensions in Unity.
                    if (drawIconGizmos) {
                        Gizmos.DrawIcon (new Vector3 (dimensionPos (transform.position.x, x), transform.position.y, dimensionPos (transform.position.z, y)), icon.name, false);
                    } else {
                        Gizmos.color = Color.white;
                        Gizmos.DrawSphere (new Vector3 (dimensionPos (transform.position.x, x), transform.position.y, dimensionPos (transform.position.z, y)), 0.025f);
                    }
                }
            }
            if (drawBlockGizmos) {
                for (int x = xDimension-1; x >= 0; x--) {
                    for (int y = yDimension-1; y >= 0; y--) {
                        for (int i = 0; i < 8; i++) {
                            GameObject gobj = grid [x, y, i];
                            if (gobj != null) {
//                                Vector3 offset = new Vector3 (this.transform.position.x + blk.getPosition ().x + bsize, this.transform.position.y + blk.getPosition ().y + bsize, this.transform.position.z + blk.getPosition ().z + bsize);
                                Vector3 offset = gobj.transform.position;
                                Gizmos.color = Color.white;
                                Gizmos.DrawSphere (offset, 0.025f);
                            }
                        }
                    }
                }
            }
        }
    }

}

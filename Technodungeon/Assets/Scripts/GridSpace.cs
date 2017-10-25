using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// holds two arrays worth of Tiles and Blocks, so that we can easily manipulate an entire GridSpace with code
// useful for the 2D Gridview
public class GridSpace {

    //Reperesent relationship between Space (Block class or Tile class of type GridObject) and prefabs using two arrays
    //we need two arrays because sometimes a Block or Tile will exist on the same block
    private GridObject[] blocks = new GridObject[8];
    private GridObject[] tiles = new GridObject[8];
    private Grid parentGrid;
    protected Vector3 position; //position relative to grid origin

    //create empty GridSpace, fill up later.
    public GridSpace(Vector3 position) {
        this.position = position;
    }

    public enum GridPos : byte {LoNW = 0, LoSW = 1, LoNE = 2, LoSE = 3, HiNW = 4, HiSW = 5, HiNE = 6, HiSE = 7};//8 blocks per grid, with cardinal positions defined here
    //you'll notice: index >= 4 , guaranteed to be higher block
    //               index % 2 == 0, guaranteed to be northern block
    //               and vice versa.


    //gets the parent grid of a given GridSpace
    public Grid getGrid() {
        return parentGrid;
    }

    /*//sets the specified Block
    public void setBlock(Block obj, int x, int y, GridPos bpos) {
        if (obj.getParent () == null) {
            obj.setParent (this.parent);
        }
        Vector3 offset = calculateBlockPosOffset (bpos);
        offset.x += x;
        //it's weird, but we want the x and y dimensions of our grid to be laid over the x and z dimensions in Unity.
        offset.z += y;
        if (x >= Grid.xDimension || y >= Grid.yDimension) {
            return;//TODO: SOLVE THIS PROBLEM!
        }
        Debug.Log ("setblock offset: "+offset.ToString ());
        grid [(byte)bpos] = BlockPrefabGenerator.getPrefab(obj, offset);
    }

    //returns the specified Block, if it exists
    //if it does not exist, makes a new one and registers it to the grid <-- why?
    public Block getBlock(int x, int y, BlockPos position) {
        Block ret = grid [x, y,(byte)position];
        if (ret == null) {
            ret = newBlock (x, y, position);
        }
        return ret;
    }

    //makes a new Block and registers it to the grid
    //if the block already exists, does nothing, returns NULL
    public Block newBlock(int x, int y, GridPos bpos) {
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
    }*/

    //calculates the position offset of a block from its gridpoint origin based on it's position in the array
    private static Vector3 calculateBlockPosOffset(GridPos bpos) {
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
}

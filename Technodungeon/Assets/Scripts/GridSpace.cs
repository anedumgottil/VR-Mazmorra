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
    private Vector2 gridPosition; //integer positions for grid
    private Vector3 worldPosition; //world space (scaled by grid size)

    //create empty GridSpace, fill up later.
    public GridSpace(Vector2 gridcoords) {
        this.gridPosition = gridcoords;
    }

    public GridSpace() {
        //do nothing?
        this.gridPosition = (Vector2)Block.DEFAULT_POSITION;
    }

    public enum GridPos : byte {LoNW = 0, LoSW = 1, LoNE = 2, LoSE = 3, HiNW = 4, HiSW = 5, HiNE = 6, HiSE = 7};//8 blocks per grid, with cardinal positions defined here
    //you'll notice: index >= 4 , guaranteed to be higher block
    //               index % 2 == 0, guaranteed to be northern block
    //               and vice versa.

    //sets the specified Block to this GridSpace in the appropriate position
    public void setBlock(Block obj, GridPos bpos) {
        if (obj == null) {
            Debug.LogError ("Error: setBlock passed null block");
            return;
        }
        if ((int)bpos >= 8 || (int)bpos < 0) {
            Debug.LogError ("Error: setBlock was passed an incorrectly formed GridPos: "+bpos);
            return;
        }
        if (obj.getParent () == null) {
            obj.setParent (this);
        }
        Vector3 offset = calculateBlockPosOffset (bpos);
        offset.x += this.worldPosition.x;
        //it's weird, but we want the x and y dimensions of our grid to be laid over the x and z dimensions in Unity.
        offset.z += this.worldPosition.y;
        if (offset.x >= Grid.getInstance().xDimension || offset.z >= Grid.getInstance().yDimension) {
            Debug.LogError ("Error: setBlock() generated out of bounds range offset");
            return;//apparently sometimes we get out of bounds ranges for setBlock?
        }
        obj.setPosition (offset);//set offset
        blocks [(int)bpos] = obj;
    }

    //returns the specified Block, if it exists
    public Block getBlock(GridPos bpos) {
        if ((int)bpos >= 8 || (int)bpos < 0) {
            Debug.LogError ("Error: getBlock was passed an incorrectly formed GridPos: "+bpos);
            return null;
        }
        return (Block) blocks[(int)bpos];
    }

    public void setParents(Grid g) {
        foreach (Block b in blocks) {
            if (b != null) {
                b.getGameObj ().gameObject.transform.SetParent (Grid.getInstance ().gameObject.transform);
            }
        }
        /*foreach (Tile t in tiles) {
            if (t != null) {
                t.getGameObj ().gameObject.transform.SetParent (Grid.getInstance ().gameObject.transform);
            }
        }*/
    }

    public void destroy() {
        foreach (Block b in blocks) {
            if (b != null) {
                MonoBehaviour.Destroy (b.getGameObj ());
            }
        }
        //TODO: set up for tiles too
    }

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

    //sets grid position, doesn't change world position on this or any child blocks.
    public void setGridPosition(Vector2 pos) {
        this.gridPosition = pos;
    }

    //also sets position of all the child blocks, since it's not a transform and this doesnt happen automatically
    public void setWorldPosition(Vector3 pos) {
        if (worldPosition != Block.DEFAULT_POSITION) {
            Debug.Log ("Warn: Moving GridSpace to new coordinates "+pos.ToString() );
        }
        this.worldPosition = pos;
        for(int i = 0; i < 8; i++) {
            if (blocks[i] != null) {
                Vector3 offset = calculateBlockPosOffset ((GridPos)i);
                blocks[i].setPosition (offset + worldPosition);
            }
        }
        //TODO: set up for tiles too
    }

    //sets both positions based off of a Vector2 grid coordinate, used to init GridSpaces in world gen.
    //also sets position of all the child blocks, since it's not a transform and this doesnt happen automatically
    public void setBothPositions(Vector2 pos) {
        if (worldPosition != Block.DEFAULT_POSITION) {
            Debug.Log ("Warn: Moving GridSpace to new coordinates "+pos.ToString() );
        }
        this.gridPosition = pos;
        this.worldPosition = new Vector3 (pos.x, 0.0f, pos.y);
        this.worldPosition *= Grid.getSize();//non-atomic scale op
        for(int i = 0; i < 8; i++) {
            if (blocks[i] != null) {
                Vector3 offset = calculateBlockPosOffset ((GridPos)i);
                blocks[i].setPosition (offset + worldPosition);
            }
        }
        //TODO: set up for tiles too
    }

    public Vector2 getGridPosition() {
        return gridPosition;
    }

    public Vector3 getWorldPosition() {
        return worldPosition;
    }
        
}

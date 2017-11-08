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
    private GridSpaceType gridSpaceType = GridSpaceType.None;//set to none for now
    public int gridSpaceConfiguration = 0;
    //the above variable is used during map generation to define the configuration of walls/floors/ceilings this GridSpace has //TODO: this index will eventually map to a keyfile ID for GridSpace Configurations.... use flatfile to generate it? hmmm.... see ticket: #17

    //below is where references to all immobile entities attached to this GridSpace will be stored
    List<StationaryEntity> stationaries = new List<StationaryEntity> (1);

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

    public enum GridSpaceType : byte {None = 0, Corridor = 1, Room = 2};
    //Empty should never be used and is just there to define an undefined room type for possible future programmatical reasons
    //Corridor is an empty corridor used to connect rooms together. it might not actually be a corridor by the definition of the word (like, it might be shaped like a room, but it's still a corridor type)
    //Room is used for the automatic generation of room areas and should probably not be used manually, it will eventually use a custom tile prefab (rooms will have special floors and walls and stuff)

    //sets the specified Block to this GridSpace in the appropriate position
    //overwrites whatever Block was at that position
    //IMPORTANT: the original Block object will persist, without it's GameObject, but it'll just be dereferenced from this GridSpace, so make sure no other objects reference this block so GC can get it
    public void setBlock(Block obj, GridPos bpos) {
        if (obj == null) {
            Debug.LogError ("Error: setBlock passed null block");
            return;
        }
        if ((int)bpos >= 8 || (int)bpos < 0) {
            Debug.LogError ("Error: setBlock was passed an incorrectly formed GridPos: "+bpos);
            return;
        }
        if (blocks [(int)bpos] != null) {
            //need to destroy the old one to overwrite it, or it will persist in the GameWorld. hope you won't need it anymore!
            MonoBehaviour.Destroy (blocks [(int)bpos].getGameObj ());
        }
        if (obj.getParent () == null) {
            obj.setParent (this);
        }
        Vector3 offset = calculateBlockPosOffset (bpos);
        offset.x += this.worldPosition.x;
        //it's weird, but we want the x and y dimensions of our grid to be laid over the x and z dimensions in Unity.
        offset.z += this.worldPosition.y;
        if (offset.x >= MapGrid.getInstance().xDimension || offset.z >= MapGrid.getInstance().yDimension) {
            Debug.LogWarning ("Error: setBlock() generated out of bounds range offset");
            //return;//apparently sometimes we get out of bounds ranges for setBlock?
        }
        obj.setPosition (offset);//set offset
        blocks [(int)bpos] = obj;
    }

    public void setTile(Tile obj, GridPos bpos) {
        if (obj == null) {
            Debug.LogError ("Error: setTile passed null tile");
            return;
        }
        if ((int)bpos >= 8 || (int)bpos < 0) {
            Debug.LogError ("Error: setTile was passed an incorrectly formed GridPos: "+bpos);
            return;
        }
        if (blocks [(int)bpos] != null) {
            //need to destroy the old one to overwrite it, or it will persist in the GameWorld. hope you won't need it anymore!
            MonoBehaviour.Destroy (tiles [(int)bpos].getGameObj ());
        }
        if (obj.getParent () == null) {
            obj.setParent (this);
        }
        Vector3 offset = calculateBlockPosOffset (bpos);
        offset.x += this.worldPosition.x;
        //it's weird, but we want the x and y dimensions of our grid to be laid over the x and z dimensions in Unity.
        offset.z += this.worldPosition.y;
        if (offset.x >= MapGrid.getInstance().xDimension || offset.z >= MapGrid.getInstance().yDimension) {
            Debug.LogWarning ("Error: setBlock() generated out of bounds range offset");
            //return;//apparently sometimes we get out of bounds ranges for setBlock?
        }
        obj.setPosition (offset);//set offset
        tiles [(int)bpos] = obj;
    }

    //TODO: implement getTile()

    //returns the specified Block, if it exists
    public Block getBlock(GridPos bpos) {
        if ((int)bpos >= 8 || (int)bpos < 0) {
            Debug.LogError ("Error: getBlock was passed an incorrectly formed GridPos: "+bpos);
            return null;
        }
        return (Block) blocks[(int)bpos];
    }

    public void setParents(MapGrid g) {
        foreach (Block b in blocks) {
            if (b != null) {
                b.getGameObj ().gameObject.transform.SetParent (MapGrid.getInstance ().gameObject.transform);
            }
        }
        foreach (Tile t in tiles) {
            if (t != null) {
                t.getGameObj ().gameObject.transform.SetParent (MapGrid.getInstance ().gameObject.transform);
            }
        }
    }

    public GridSpaceType getGridSpaceType() {
        return gridSpaceType;
    }

    public void setGridSpaceType(GridSpaceType gst) {
        gridSpaceType = gst;
    }

    //destroys the whole GridSpace
    public void destroy() {
        foreach (Block b in blocks) {
            if (b != null) {
                MonoBehaviour.Destroy (b.getGameObj ());
            }
        }
        foreach (Tile b in tiles) {
            if (b != null) {
                MonoBehaviour.Destroy (b.getGameObj ());
            }
        }
        //TODO: set up for tiles too
        foreach (StationaryEntity se in stationaries) {
            //remove all stationaryentities from the world that are associated with this gridspace
            MonoBehaviour.Destroy (se.gameObject);
        }
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
        for(int i = 0; i < 8; i++) {
            if (tiles[i] != null) {
                Vector3 offset = calculateBlockPosOffset ((GridPos)i);
                tiles[i].setPosition (offset + worldPosition);
            }
        }
    }

    //sets both positions based off of a Vector2 grid coordinate, used to init GridSpaces in world gen.
    //also sets position of all the child blocks, since it's not a transform and this doesnt happen automatically
    public void setBothPositions(Vector2 pos) {
        this.gridPosition = pos;
        this.worldPosition = new Vector3 (pos.x, 0.0f, pos.y);
        this.worldPosition *= MapGrid.getSize();//non-atomic scale op
        for(int i = 0; i < 8; i++) {
            if (blocks[i] != null) {
                Vector3 offset = calculateBlockPosOffset ((GridPos)i);
                blocks[i].setPosition (offset + worldPosition);
            }
        }
        for(int i = 0; i < 8; i++) {
            if (tiles[i] != null) {
                Vector3 offset = calculateBlockPosOffset ((GridPos)i);
                tiles[i].setPosition (offset + worldPosition);
            }
        }
    }

    public Vector2 getGridPosition() {
        return gridPosition;
    }

    public Vector3 getWorldPosition() {
        return worldPosition;
    }

    //so long as we don't already have a reference to this stationary entity, add it to our reference list.
    public void addStationary(StationaryEntity se) {
        //TODO: possibly check for whether or not this stationary gameobject is positionally located within this GridSpace before adding it
        if (!stationaries.Contains (se)) {
            stationaries.Add (se);
        }
    }

    //pretty self-explanatory. 
    //Note: you should probably not use this call by itself, you should instead tell the SE to change it's parent with the SE.setGridSpace() command which does this for you.
    public void removeStationary(StationaryEntity se) {
        stationaries.Remove (se);
    }

    //get the list of stationary entities that reside within this GridSpace
    public List<StationaryEntity> getStationaries() {
        return stationaries;
    }
        
}

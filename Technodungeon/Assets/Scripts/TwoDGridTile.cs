using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDGridTile : MonoBehaviour {
    private Vector2Int gridPosition; //the placement of this gridtile in integer grid coordinates, corresponds to an actual MapGrid GridObject in our Virtual Reality, set when this Tile is created by the GridGenerator class.
    private bool initialized = false;
    private TwoDGridTileState state = TwoDGridTileState.NONE;//default to NONE, synonym for invalid tile
    private int index = 1;
    public enum TwoDGridTileState : byte {NONE=0, UNOCCUPIED, OCCUPIED, PLAYER, ROOM, CORRIDOR, PORTAL, REACTOR, COOLANT, AICORE, TECHNOCORE, TURRET, MOB};//etc.etc.etc

    //call this after you generate these tiles. tile needs to be aware of it's position at all times.
    public void setGridPosition(Vector2Int vint) {
        gridPosition = vint;
        initialized = true;
    }

    public Vector2Int getGridPosition() {
        
        return gridPosition;
    }

    //the tile cannot function if it does not know its position. If this returns false, the tile is invalid, and all bets are off
    public bool isInitialized() {
        return initialized;
    }

    //SHANIL FILL THIS OUT: you're almost there! just need the important functions. I would do it but I'm swamped! :)
    //sets the color of the tile.
    public void setColor(Color c) {
        //access the Renderer component of this.gameObject and set it's color to whatever is specified. hopefully this doesn't affect the other clones
        
        Color startColor = new Color(128, 0, 128, 1);
        MeshRenderer gameObjectRenderer = this.gameObject.GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Purple"));

        newMaterial.color = startColor;
        gameObjectRenderer.material = newMaterial;

    }


    //////////////////////////IMPORTANT////////////////////////////
    //SHANIL: add handler for click event when the user clicks this GameObject. I think it's a onClickSomething() function you put here, it runs when the user clicks this Tile. look it up, its a unity thing. you need to call
    //MapGenerator.Instance.setGridSpace(this.gridPosition.x, this.gridPosition.y, GridSpace.GridSpaceType.ROOM); to spawn a room tile there, for now... later we'll make this drop enemies if its occupied and all that
    //////////////////////////IMPORTANT////////////////////////////
    public void OnMouseDown()
    {
        MapGenerator.Instance.setGridSpace(this.gridPosition.x, this.gridPosition.y, GridSpace.GridSpaceType.Room);
    }

    //main, most important function. MapGenerator will, every second or so, run this for all it's tiles. This will poll the mapgrid and ask what the Tile's state is at the coordinates.
    //Depending on what it returns, we'll set the color of this tile appropriately. I've made an enum to represent the tile states better.
    public void updateStatus() {
        if (!initialized || !MapLoader.isMapResourcesLoadComplete ()) {
            return;
        }
        GridSpace ourGS = MapGrid.getInstance ().getGridSpace (this.gridPosition.x, this.gridPosition.y);
        if (ourGS == null) {
            //we have no gridspace at this position, we are UNOCCUPIED.
            state = TwoDGridTileState.UNOCCUPIED;
        } else {
            if (ourGS.getGridSpaceType () == GridSpace.GridSpaceType.Room) {
                //this is a Room tile.
                state = TwoDGridTileState.ROOM;
            } else if (ourGS.getGridSpaceType () == GridSpace.GridSpaceType.Corridor) {
                state = TwoDGridTileState.CORRIDOR;
            } else {
                state = TwoDGridTileState.OCCUPIED;
                //Debug.Log ("Found occupied Tile");
            }
        }
        //TODO: we only check for the occupied status of the GridSpace in the MapGrid at our corresponding position right now. It would be great if we could get the Player's position, figure out if they're at our GridTile or not
        // and if they are, then color us accordingly. Also, we have to check for all the other types of GridSpace as well, such as Reactors, etc.etc.
        
        updateColor ();
    }

    public void updateColor() {
        switch (state) {
        case (TwoDGridTileState.NONE):
            return;//do nothing, none is an invalid tile, we dont exist yet.
        case (TwoDGridTileState.UNOCCUPIED):
            //color the tile grey, nothing is there.
            this.setColor (Color.grey);
            break;
        case (TwoDGridTileState.OCCUPIED):
            this.setColor (Color.cyan);//a tile exists, color us cyan so you can tell, the color of technology
            break;
        case (TwoDGridTileState.PLAYER):
            this.setColor (Color.red);//that darn technodude is messin with our circuits! color the tile red so we know where hes at. later, he will only be visible of the GridSpace has a working camera trap.
            break;
        case (TwoDGridTileState.REACTOR):
            //this is a tile that has been designated as a REACTOR space. It's special, and we'll denote it as such.
            this.setColor (Color.yellow);
            break;
        case (TwoDGridTileState.ROOM):
            this.setColor (new Color (0f, (Color.cyan.g * 0.66f), (Color.cyan.b * 0.66f)));//color rooms "dark cyan"
            break;
        case (TwoDGridTileState.CORRIDOR):
            this.setColor (new Color (0.25f, (Color.cyan.g * 0.66f), 1f));//color corridors "light cyan"
            break;
        case (TwoDGridTileState.COOLANT):
            this.setColor (Color.blue);//color coolants blue, since they're cool
            break;
        }
    }
}

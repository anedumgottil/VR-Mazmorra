using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDGridTile : MonoBehaviour {
    public static bool TwoDGridTile_Debug = true;
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
        
    //sets the color of the tile.
    public void setColor(Color c) {
        //access the Renderer component of this.gameObject and set it's color to whatever is specified. hopefully this doesn't affect the other clones

        MeshRenderer gameObjectRenderer = this.gameObject.GetComponent<MeshRenderer>();
        if (gameObjectRenderer != null) {
            gameObjectRenderer.material.color = c;
        }

    }
        
    public void OnMouseDown()
    {
        if (TwoDGridTile_Debug) Debug.Log ("Mouse click on Tile : " + this.gridPosition.ToString ());
        Vector3 entitySpawnOffset = new Vector3 (0.5f, 0.5f, 0.5f);

        GridSpace atOurLocation = MapGrid.getInstance ().getGridSpace (this.gridPosition.x, this.gridPosition.y);
        if (atOurLocation == null) {//no corresponding grid tile
            if (AIPlayer.spendEnergy (5)) {
                MapGenerator.Instance.setGridSpace (this.gridPosition.x, this.gridPosition.y, GridSpace.GridSpaceType.Corridor);
            }

        } else {
            if (!AIUIManager.getSelection ().Equals ("None")) {
                if (AIUIManager.getSelection ().Equals ("TreadBot")) {
                    if (AIPlayer.spendEnergy (75)) {
                        createMobileEntity (atOurLocation, "TreadBot", entitySpawnOffset);
                    }
                }
                if (AIUIManager.getSelection ().Equals ("Drone")) {
                    if (AIPlayer.spendEnergy (33)) {
                        createMobileEntity (atOurLocation, "Drone", entitySpawnOffset);
                    }
                }
            }
        }
    }

    private void createStationaryEntity(GridSpace ourGS, string name, Vector3 pos) {
        GameObject prefabInstance = MapLoader.getPrefabInstance (name);
        StationaryEntity stationaryEntityComponent = prefabInstance.GetComponent<StationaryEntity> ();
        if (prefabInstance != null && stationaryEntityComponent != null) {
            prefabInstance.transform.position = ourGS.getWorldPosition () + pos;
            prefabInstance.SetActive (true);
            stationaryEntityComponent.setGridSpace (ourGS);
            if (TwoDGridTile_Debug) Debug.Log ("2DGridTile Created StationaryEntity ["+prefabInstance.gameObject.name.ToString()+"] at world position: " + prefabInstance.transform.position.ToString () + " and tile position: " + gridPosition.ToString ()+" registered to GridSpace: "+ourGS.getGridPosition ().ToString ());
        } else if (prefabInstance == null) {
            Debug.LogWarning ("TwoDGridTile: click event: Attempted to load in an Entity for GridSpace, but couldn't get an instance from the Entity database.");
        } else if (stationaryEntityComponent == null) {
            Debug.LogWarning ("TwoDGridTile: click event Tried to register an entity to a GridSpace during click that was not a StationaryEntity. ["+prefabInstance.gameObject.name.ToString()+"]");
        }
    }

    private void createMobileEntity(GridSpace ourGS, string name, Vector3 pos) {
        GameObject prefabInstance = MapLoader.getPrefabInstance (name);
        if (prefabInstance != null) {
            prefabInstance.transform.position = ourGS.getWorldPosition () + pos;
            prefabInstance.SetActive (true);
            if (TwoDGridTile_Debug) Debug.Log ("2DGridTile Created MobileEntity ["+prefabInstance.gameObject.name.ToString()+"] at world position: " + prefabInstance.transform.position.ToString () + " and tile position: " + gridPosition.ToString ());
        } else {
            Debug.LogWarning ("TwoDGridTile: click event: Attempted to load in an Entity for GridSpace, but couldn't get an instance from the Entity database.");
        }
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
            } else if (ourGS.getGridSpaceType () == GridSpace.GridSpaceType.Reactor) {
                state = TwoDGridTileState.REACTOR;
            } else if (ourGS.getGridSpaceType () == GridSpace.GridSpaceType.Coolant) {
                state = TwoDGridTileState.COOLANT;
            } else {
                state = TwoDGridTileState.OCCUPIED;
                //Debug.Log ("Found occupied Tile");
            }
        }
        //TODO: we only check for the occupied status of the GridSpace in the MapGrid at our corresponding position right now.
        //we have to check for all the other types of GridSpace as well, such as Reactors, etc.etc.
        
        updateColor ();
    }

    //allows us to react to the players position
    public void updateStatus(bool isPlayerTile) {
        if (isPlayerTile) {
            //we have player
            state = TwoDGridTileState.PLAYER;
            updateColor ();
        } else {
            updateStatus ();//normal status update
        }
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

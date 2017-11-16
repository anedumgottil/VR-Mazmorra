using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a collection of GridSpaces, used to define areas of the map that are contiguous
//doesn't actually store GridSpaces, it stores a collection of GridCoordinate and GridSpaceConfiguration index pairs, for memory efficiency
public class Room {

    private GridSpace.GridSpaceType gridSpaceType = GridSpace.GridSpaceType.None;//set to none for now
    private Dictionary<Vector2Int, int > spaceMap;//collection of 2D Grid Coordinates and GridSpace types
    private Dictionary<Vector2Int, KeyValuePair<string, Vector3> > entityMap;//collection of Entity names with local offset Unity coordinates attached to a GridSpace coordinate to attach them to
    //we need to specify a bounding box so we know how much to translate the gridspacecoords to be ready to import if the user specifies weird coordinates.
    private int lowX = -1;
    private int lowY = -1;
    private int highY = -1;
    private int highX = -1;
    private int height = 0;
    private int width = 0;
    private string name = "default";
    private bool isTranslated = false;

    public Room(string name) {
        this.name = name;
        spaceMap = new Dictionary<Vector2Int, int> (2);
        entityMap = new Dictionary<Vector2Int, KeyValuePair<string, Vector3>>();
    }

    public Room(GridSpace.GridSpaceType gstype, string name) {
        gridSpaceType = gstype;
        this.name = name;
        spaceMap = new Dictionary<Vector2Int, int> (2);
        entityMap = new Dictionary<Vector2Int, KeyValuePair<string, Vector3>>();
    }

    public Room(GridSpace.GridSpaceType gstype) {
        gridSpaceType = gstype;
        spaceMap = new Dictionary<Vector2Int, int> (2);
        entityMap = new Dictionary<Vector2Int, KeyValuePair<string, Vector3>>();
    }

    public Room() {
        spaceMap = new Dictionary<Vector2Int, int> (2);
        entityMap = new Dictionary<Vector2Int, KeyValuePair<string, Vector3>>();
    }

    //registers a coordinate-config pair to this Room
    public bool registerSpace(Vector2Int gridCoord, int gridSpaceConfiguration) {
        if (spaceMap.ContainsKey (gridCoord)) {
            Debug.LogWarning ("Room: Tried to add a duplicate coordinate (" + gridCoord.x + ", " + gridCoord.y + ")");
            return false;
        }
        //make sure to update our lowest and highest coordinates
        updateBounds(gridCoord);
        
        spaceMap.Add (gridCoord, gridSpaceConfiguration);
        return true;
    }

    //registers a coordinate to this Room, but specifies a type, so that if Room type is unset at this time, it'll infer the Room type based on what is given to this function
    public bool registerSpace(Vector2Int gridCoord, int gridSpaceConfiguration, GridSpace.GridSpaceType gstype) {
        if (this.gridSpaceType != GridSpace.GridSpaceType.None && gstype != this.gridSpaceType) {
            Debug.LogWarning ("Room: Trying to register a GridSpace to a Room with an incompatible GridSpaceType");
            return false;
        } else if (spaceMap.ContainsKey (gridCoord)) {
            Debug.LogWarning ("Room: Tried to add a duplicate coordinate (" + gridCoord.x + ", " + gridCoord.y + ")");
            return false;
        } else if (this.gridSpaceType == GridSpace.GridSpaceType.None) {
            this.setType (gstype);
        }
        //make sure to update our lowest and highest coordinates
        updateBounds(gridCoord);

        spaceMap.Add (gridCoord, gridSpaceConfiguration);
        return true;
    }

    //registers an Entity to spawn to this Room, with a GridSpace coord to assign it to, a name to search for, and an offset to specify it's local position relative to the GridSpace origin.
    public bool registerEntity(Vector2Int gridCoord, string name, Vector3 localPos) {
        if (name.Trim ().Equals ("")) {
            Debug.LogWarning ("Room: Tried to register an Entity that didn't have a name");
            return false;
        }
        entityMap.Add (gridCoord, new KeyValuePair<string, Vector3>(name.Trim (), localPos));
        return true;
    }

    //translates all the coordinates so that none of them are negative, i.e., the Room will have it's lowest coordinate at (0,0)
    //similar to normalization of the coordinate vectors
    public void translateSpaceMapToOrigin() {
        if (isTranslated) {
            Debug.LogWarning ("Room: Translating room twice, this will lead to incorrect translations - cowardly refusing");
            return;
        }

        int adjustX = -lowX;
        int adjustY = -lowY;

        if (adjustX == 0 && adjustY == 0) {
            //no adjustments necessary
            return;
        }

        foreach (KeyValuePair<Vector2Int, int> entry in spaceMap) {
            spaceMap.Remove (entry.Key);
            Vector2Int newVec = entry.Key + new Vector2Int (adjustX, adjustY);
            spaceMap.Add (newVec, entry.Value);
        }
        this.isTranslated = true;
    }

    //updates the bounding box coordinate records properly
    private void updateBounds(Vector2Int coordinate) {
        //make sure to update our lowest and highest coordinates
        if (lowX > coordinate.x)
            lowX = coordinate.x;
        if (lowY > coordinate.y)
            lowY = coordinate.y;
        if (highX < coordinate.x)
            highX = coordinate.x;
        if (highY < coordinate.y)
            highY = coordinate.y;

        int width = highX - lowX;
        int height = highY - lowY;

        if (width < 0 || height < 0) {
            Debug.LogError ("Room: Height and Width distance calculations produced negative value - this should not happen");
            return;
        }

        this.width = width;
        this.height = height;

    }

    //returns a list of Grid Coordinates connected to GridSpaceConfiguration Indices, translated properly to Origin for MapGeneration
    public Dictionary<Vector2Int, int> getSpaceMap() {
        if (!isTranslated) {
            Debug.LogError ("Room: Tried to get a map of GridSpaceTypes before they were properly translated");
            return null;
        }
        return spaceMap;
    }

    //returns a list of Grid Coordinates connected to Entity names and local positional offsets
    public Dictionary<Vector2Int, KeyValuePair<string, Vector3>> getEntityMap() {
        if (!isTranslated) {
            Debug.LogError ("Room: Tried to get a map of Entities before they were properly translated");
            return null;
        }
        return entityMap;
    }

    //remove all GS's from this room. 
    public void clearSpaces() {
        spaceMap.Clear ();
    }

    //CENTERED ON ORIGIN
    //get a bounding box that will encompass this Room as it was specified in it's original gstype list (xml file, usually)
    private RectInt getBounds() {
        return new RectInt (0, 0, width, height);
    }

    //NOT CENTERED ON ORIGIN
    //get a bounding box that will encompass this Room this origin-centered Room (room coordinates have been translated to origin)
    private RectInt getBoundsRaw() {
        return new RectInt (lowX, lowY, width, height);
    }

    //NOT CENTERED ON ORIGIN
    //return highest most corner of bounding box in grid coordinate system, before they were translated to origin
    private Vector2Int getBoundsHighCoord() {
        return new Vector2Int (highX, highY);
    }

    //NOT CENTERED ON ORIGIN
    //return lowest most corner of bounding box in grid coordinate system, before they were translated to origin
    private Vector2Int getBoundsLowCoord() {
        return new Vector2Int (lowX, lowY);
    }

    public string getName() {
        return name;
    }

    public void setName(string name) {
        this.name = name;
    }

    //set the GridSpaceType
    public void setType(GridSpace.GridSpaceType gridSpaceType) {
        this.gridSpaceType = gridSpaceType;
    }

    //return the type of GS that will/should be stored in this Room
    public GridSpace.GridSpaceType getType() {
        return gridSpaceType;
    }
}

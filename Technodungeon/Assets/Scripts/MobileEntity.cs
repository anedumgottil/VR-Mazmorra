using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an entity that moves. This one will be stored in its' own object pool, and it's own Unity layer. It will have it's own GameObject tag defining it's class type.
//inherit this to define an object that might be picked up or moved around, and shouldn't be attached to a GridSpace (e.g., Mobs, Technoshards, etc.) 

public class MobileEntity : Entity {

    int gridPosX = GridObject.DEFAULT_POSITION.x;
    int gridPosY = GridObject.DEFAULT_POSITION.y;
    
    //translates the Entity entirely to a specified Grid Coordinate
    public virtual void teleportToGridCoords(int x, int y) {
        this.gameObject.transform.Translate (getWorldCoordsFromGridCoords (x, y), Space.World);
        this.gridPosX = x;
        this.gridPosY = y;
    }


    //scales the grid coords accordingly to present world coords according to the Grid transform scale setting
    //WARNING this is broken right now because Grid scale does not inherit to it's objects... s it's useless until we can scale the map up
    public Vector3 getWorldCoordsFromGridCoords(int x, int y) {
        /*Vector3 scale = Grid.getInstance().gameObject.transform.scale;
        return new Vector3 (x * scale.x, this.gameObject.transform.position.y, y * scale.z);*/
        return new Vector3 (x, this.gameObject.transform.position.y, y);
    }
}

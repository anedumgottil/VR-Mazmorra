using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an entity that moves. This one will be stored in its' own object pool, and it's own Unity layer. It will have it's own GameObject tag defining it's class type.
//inherit this to define an object that might be picked up or moved around, and shouldn't be attached to a GridSpace (e.g., Mobs, Technoshards, etc.) 

public class MobileEntity : Entity {

    //translates the Entity entirely to a specified MapGrid Coordinate
    public virtual void teleportToGridLocation(int x, int y) {
        if (this.gameObject == null)
            return;
        this.gameObject.transform.position = MapGrid.getInstance().getWorldCoordsFromGridCoords (x, y);
    }
}

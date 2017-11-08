using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an entity that moves. This one will be stored in its' own object pool, and it's own Unity layer. It will have it's own GameObject tag defining it's class type.
//inherit this to define an object that might be picked up or moved around, and shouldn't be attached to a GridSpace (e.g., Mobs, Technoshards, etc.) 

public class MobileEntity : Entity {

    int gridPosX = (int)GridObject.DEFAULT_POSITION.x;
    int gridPosY = (int)GridObject.DEFAULT_POSITION.y;
    
    //translates the Entity entirely to a specified MapGrid Coordinate
    //TODO: this is broken, why doesn't this work?? FIX IT! then update mapgen to use it.
    public virtual void teleportToGridCoords(int x, int y) {
        if (this.gameObject == null)
            return;
        this.gameObject.transform.Translate (MapGrid.getInstance().getWorldCoordsFromGridCoords (x, y), Space.World);
        this.gridPosX = x;
        this.gridPosY = y;
    }
}

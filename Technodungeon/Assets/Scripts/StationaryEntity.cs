using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an entity that doesn't move. Use this to represent things like Turrets or Doors that need to be associated with a GridSpace, since they'll never move around and we might want to visualize/edit them with the 2DGridview.
//They have some added restrictions, for example, they need to keep track of the parent GridSpace that holds them, so both the GridSpace and the StationaryEntity keep a reference to each other

public abstract class StationaryEntity : Entity {
    protected GridSpace parent = null;//The parent GridSpace that this StationaryEntity is attached to. If this is null, the entity should NOT exist in the game scene! This should always be set.

    public StationaryEntity(GridSpace gs) {
        this.setGridSpace (gs);
    }

    public GridSpace getGridSpace() {
        return parent;
    }

    public Vector2 getGridLocation() {
        return parent.getGridPosition ();
    }

    //adjust the GridSpace that this ent is attached to
    //NOTE: this automatically notifies gridspaces of the ownership change, so you don't have to! Convenient!
    public void setGridSpace(GridSpace gs) {
        if (parent != null) {//remove ourselves from our current parent, if we need to:
            parent.removeStationary (this);
        }
        if (gs == null) {//if we get a null gs, we're trying to unset our parent, so ret.
            parent = null;
            return;
        }
        //add ourselves to the new gridspace
        gs.addStationary(this);
        //update our parent
        parent = gs;
    }
}

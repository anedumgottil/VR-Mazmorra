using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//an entity that moves. This one will be stored in its' own object pool, and it's own Unity layer. It will have it's own GameObject tag defining it's class type.
//inherit this to define an object that might be picked up or moved around, and shouldn't be attached to a GridSpace (e.g., Mobs, Technoshards, etc.) 

public abstract class MobileEntity : Entity {
    //needs filling out
}

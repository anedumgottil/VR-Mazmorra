using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroBlock {
    private GameObject microblockPrimitive = null;
    private Vector3 position; //position relative to block origin
    private static float microblockSize = Block.getSize()/Block.mbDivision;//meters
    private Block parent;
    public const string PARENT_MICROBLOCK_NAME_PREFIX = "Microfab #";

    public MicroBlock(Block parent, Vector3 position) {
        this.parent = parent;
        this.position = position;//set once, never change; defined by its position in the grid...
        microblockPrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        microblockPrimitive.transform.position = this.position;
        microblockPrimitive.transform.localScale = new Vector3(microblockSize, microblockSize, microblockSize);
        microblockPrimitive.name = PARENT_MICROBLOCK_NAME_PREFIX + position.ToString ();//set name so that it can be parsed when we stick this Prefab into a GridSpace. (edit: this may be alleviated by serialization)
        //random colors for prefab template
        Color random = Random.ColorHSV ();
        microblockPrimitive.GetComponent<Renderer>().material.color = random;
        //set this microblock as a child of it's parent block, declutter the inspector, maintain a rigid heirarchy
        microblockPrimitive.transform.SetParent (this.parent.getGameObj ().transform);
    }

    public GameObject getPrimitive() {
        //gets the registered primitive for this MicroBlock
        return microblockPrimitive;
    }

    public Vector3 getPosition() {
        return position;
    }

    //returns computed worldspace position
    public Vector3 getGlobalPosition() {
        Vector3 ret = position;

        //add Block local position to our relative-to-block superlocal position
        ret.x += parent.getPosition().x;
        ret.y += parent.getPosition().y;
        ret.z += parent.getPosition().z;

        //add Grid global position to our relative-to-grid Block local position
        ret.x += parent.getParent().getGrid().transform.position.x;
        ret.y += parent.getParent().getGrid().transform.position.y;
        ret.z += parent.getParent().getGrid().transform.position.z;

        return ret;
    }

    public Block getParent() {
        return parent;
    }

    public static float getSize() {
        return microblockSize;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlockPrefabGenerator {

	// This class exists just so I do not have to include the UnityEditor library in our build
    // It takes a Block and generates a Prefab in the Prefabs folder. These pre-generated map tiles can then be built with the project

    public static void generatePrefab(Block bl) {
        PrefabUtility.CreatePrefab ("Assets/Prefabs/" + Block.PARENT_BLOCK_NAME_PREFIX + bl.getID () + ".prefab", bl.getGameObj ());
        /*        //color the blox
        Color random = Random.ColorHSV ();
        foreach (Transform microblockPrimitive in temp.transform) {
            microblockPrimitive.GetComponent<Renderer> ().material.color = random;
        }*/
    }
}

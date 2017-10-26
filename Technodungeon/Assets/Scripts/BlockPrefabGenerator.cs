using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockPrefabGenerator {

	// This class exists just so I do not have to include the UnityEditor library in our build
    // It takes a Block and generates a Prefab in the Prefabs folder. These pre-generated map tiles can then be built with the project

    public static void generatePrefab(Block bl) {
        #if UNITY_EDITOR
        PrefabUtility.CreatePrefab ("Assets/Prefabs/" + Block.PARENT_BLOCK_NAME_PREFIX + bl.getID () + ".prefab", bl.getGameObj ());
        #endif
        /*        //color the blox
        Color random = Random.ColorHSV ();
        foreach (Transform microblockPrimitive in temp.transform) {
            microblockPrimitive.GetComponent<Renderer> ().material.color = random;
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockPrefabGenerator {

	// This class exists just so I do not have to include the UnityEditor library in our build
    // It takes a Block and generates a Prefab in the Prefabs folder. These pre-generated map tiles can then be built with the project

    public static void generatePrefab(Block bl, bool overwrite) {
        #if UNITY_EDITOR
        if (!overwrite && System.IO.File.Exists("Assets/Prefabs/Blocks/" + Block.PARENT_BLOCK_NAME_PREFIX + bl.getID () + ".prefab")) {
            return;//skip an overwrite
        }
        //Should we be using the ReplacePrefabOptions.ReplaceNameBased argument to CreatePrefab here, or default like we currently are? especially considering an overwrite? 
        PrefabUtility.CreatePrefab ("Assets/Prefabs/Blocks/" + Block.PARENT_BLOCK_NAME_PREFIX + bl.getID () + ".prefab", bl.getGameObj ());
        #endif
        /*        //color the blox
        Color random = Random.ColorHSV ();
        foreach (Transform microblockPrimitive in temp.transform) {
            microblockPrimitive.GetComponent<Renderer> ().material.color = random;
        }*/
    }
}

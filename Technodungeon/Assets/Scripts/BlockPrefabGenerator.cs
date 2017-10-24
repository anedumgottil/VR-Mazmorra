using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlockPrefabGenerator {

	// This class exists just so I do not have to include the UnityEditor library in our build
    // It takes a Block and generates a Prefab in the Prefabs folder. These pre-generated map tiles can then be built with the project

    public static GameObject getPrefab(Block bl) {
        GameObject temp = GameObject.Instantiate(PrefabUtility.CreatePrefab ("Assets/Prefabs/Block_" + bl.getBlockID () + ".prefab", bl.getGameObj ()), bl.getPosition(), Quaternion.identity);
        MapLoader.gamePrefabs.Add ( temp);
        return temp;
    }
    public static GameObject getPrefab(Block bl, Vector3 position) {
        GameObject temp = GameObject.Instantiate(PrefabUtility.CreatePrefab ("Assets/Prefabs/Block_" + bl.getBlockID() + ".prefab", bl.getGameObj()), position, Quaternion.identity);
        //color the blox
        Color random = Random.ColorHSV ();
        foreach (Transform microblockPrimitive in temp.transform) {
            microblockPrimitive.GetComponent<Renderer> ().material.color = random;
        }
        MapLoader.gamePrefabs.Add (temp);
        return temp;
    }
}

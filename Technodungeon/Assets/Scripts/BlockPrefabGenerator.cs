using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BlockPrefabGenerator {

	// This class exists just so I do not have to include the UnityEditor library in our build
    // It takes a Block and generates a Prefab in the Prefabs folder. These pre-generated map tiles can then be built with the project

    public static GameObject generatePrefab(Block bl) {
        GameObject temp = GameObject.Instantiate(PrefabUtility.CreatePrefab ("Assets/Prefabs/Block_" + bl.getID () + ".prefab", bl.getGameObj ()), bl.getPosition(), Quaternion.identity);
        MapLoader.addPrefab(temp.name, temp);
        return temp;
    }
    public static GameObject generatePrefab(Block bl, Vector3 position) {
        GameObject temp = GameObject.Instantiate(PrefabUtility.CreatePrefab ("Assets/Prefabs/Block_" + bl.getID() + ".prefab", bl.getGameObj()), position, Quaternion.identity);
        //color the blox
        Color random = Random.ColorHSV ();
        foreach (Transform microblockPrimitive in temp.transform) {
            microblockPrimitive.GetComponent<Renderer> ().material.color = random;
        }
        MapLoader.addPrefab(temp.name, temp);
        return temp;
    }
}

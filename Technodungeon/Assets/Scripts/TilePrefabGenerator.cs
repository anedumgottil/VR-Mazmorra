using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TilePrefabGenerator {

    private const bool DEBUG = true;

    // This is a new PrefabGenerator class that will allow us to convert a master Tile Prefab (one that has all sides, floors, and ceilings, labeled according to the current standard)
    // into a large number of subsidiary child prefabs (ones with less walls/floors/ceilings and etc., according to the GridObject configuration key flatfile in the Resources folder)
    // I've invented this because updating all of our scenery prefabs is a huge pain to do whenever we want to simply modify a collider or something.
    // Using this - assuming the generation is turned on in the MapLoader and you're in the editor - will greatly simplify scenery creation and make it modular like we set out to do.
    private GameObject parentFloorPrefab;
    private GameObject parentCeilingPrefab;
    private string tileName;

    public TilePrefabGenerator(GameObject parentTileFloorPrefab, GameObject parentTileCeilingPrefab, string tileName) {
        this.parentFloorPrefab = parentTileFloorPrefab;
        this.parentCeilingPrefab = parentTileCeilingPrefab;
        this.tileName = tileName;
    }

    //specify GridObjectIndex range, inclusive -> exclusive, this will generate all prefabs within that range into the given path, overwriting any found with the same name.
    public void generatePrefabs(int indexMin, int indexMax, string path, bool shouldOverwrite) {
        Debug.Log ("Generating Prefabs for " + tileName + ": \t\t--------------");
        if (indexMin < 0 || indexMax < 0 || indexMax < indexMin) {
            Debug.LogError ("PrefabGenerator: generatePrefabs: Passed invalid index");
            return;
        }
        for (int i = indexMin; i < indexMax; i++) {
            Debug.Log ("Generating Prefab Index " + i.ToString ("D2") + " at path \"" + path+"\"...");
            generatePrefab (parentFloorPrefab, parentCeilingPrefab, i, path, tileName, shouldOverwrite);
        }
        Debug.Log ("Finished Generating Prefabs for " + tileName + ".\t--------------");
    }

    //Types currently available (and these are how these objects should be labeled in the prefab, or this class will throw errors) are:
    // -Ceiling, -Floor, -Wall S, -Wall E, -Wall N, -Wall W
    //but this function will try to remove whatever type name you give it
    private static bool removeType(GameObject fabCopy, string type) {
        //remove type of wall/ceiling/floor from object:
        Transform childTransform = fabCopy.transform.Find (type);
        /*if (DEBUG) {
            foreach (Transform c in fabCopy.transform) {
                Debug.Log ("Found Child: " + c.gameObject.name);
            }
        }*/
        if (childTransform != null) {
            childTransform.parent = null;
            GameObject child = childTransform.gameObject;
            if (DEBUG) {Debug.Log("PrefabGenerator: Removing child type "+child.name);}
            GameObject.Destroy (child);
            return true;
        } else {
            Debug.LogWarning ("PrefabGenerator: removeType: Tried to remove type from GameObject but could not find type (" + type + ") in the given child heirarchy");
            return false;
        }
    }

    private static void generatePrefab(GameObject parentFloorFab, GameObject parentCeilingFab, int gridObjectID, string path, string tilename, bool shouldOverwrite) {
        if (gridObjectID >= 0 && gridObjectID <= 1) {
            //if it's tile zero or one, we do not need to generate a prefab from it, it already exists to create this TilePrefabGenerator.
            //remember that one can simply be a copy of zero, and all of this is for naught, but we aren't going to bother checking for that case since the outcome will be the same anyways.
            return;
        } else if (gridObjectID > 1 && gridObjectID <= 6) {
            //2 - 6? use the floor prefab. we don't want the wall tiling on double-high's to be messed up if we use the ceiling one.
            generatePrefab (parentFloorFab, gridObjectID, path, tilename, shouldOverwrite);
        } else if (gridObjectID > 6 && gridObjectID <= 15) {
            //7- 15, we have a ceiling tile.
            generatePrefab (parentCeilingFab, gridObjectID, path, tilename, shouldOverwrite);
        } else if (gridObjectID > 15 && gridObjectID <= 19) {
            //16-19 we have a floor tile again.
            generatePrefab (parentFloorFab, gridObjectID, path, tilename, shouldOverwrite);
        } else {
            Debug.LogWarning ("TilePrefabGenerator: generatePrefab: passed unknown gridObjectID number, not sure if it's a ceiling or floor-walled tile, assuming floor");
            generatePrefab (parentFloorFab, gridObjectID, path, tilename, shouldOverwrite);
        }
    }

    //give the parent tile prefab to base the child prefab off of, the gridObjectID, a path to the folder of these tiles (e.g. "Assets/Prefabs/Tiles/SciFiTiles/",  note the leading slash) and the tile name (e.g. "SciFiTile")
    public static void generatePrefab(GameObject parentFab, int gridObjectID, string path, string tilename, bool shouldOverwrite) {
        //first check if the file already exists. If it does, we can just leave right now, assuming we don't want to overwrite
        if (!shouldOverwrite) {
            //technically, loading twice as a check is an inefficient way of determining the file existence, but it's just easier and this code is only run rarely.
            if (Resources.Load (path + tilename + "_" + gridObjectID.ToString("D2")) != null) {
                if (DEBUG) {Debug.Log("TilePrefabGenerator: Skipping pre-existing " + path + tilename + "_" + gridObjectID.ToString("D2") + ".prefab");}
                return;
            }
        }

        GameObject temp = (GameObject)GameObject.Instantiate (parentFab); 
        bool removalSuccess = false;
        switch (gridObjectID) {
        case 0://Master Tile with floor-type walls
            GameObject.Destroy (temp);
            return;
            //if it's tile zero, we do not need to generate a prefab from it, it already exists to create this TilePrefabGenerator.

        case 1://Master Tile with ceiling-type walls
            GameObject.Destroy (temp);
            return;
            //same as above

        case 2://Floor
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 3://Floor West Wall
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
           removalSuccess = true; break;

        case 4://Floor East Wall
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 5://Floor South Wall
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 6://Floor North Wall
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 7://Ceiling
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 8://Ceiling West Wall
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
           removalSuccess = true; break;

        case 9://Ceiling East Wall
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 10://Ceiling South Wall
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 11://Ceiling North Wall
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 12://Ceiling West/South Corner
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
           removalSuccess = true; break;

        case 13://Ceiling West/North Corner
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
           removalSuccess = true; break;

        case 14://Ceiling East/South Corner
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 15://Ceiling East/North Corner
            //remove floor from object:
            if (!removeType(temp, "Floor")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 16://Floor West/South Corner
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
           removalSuccess = true; break;

        case 17://Floor West/North Corner
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove East Wall from object:
            if (!removeType(temp, "Wall E")) {break;}
           removalSuccess = true; break;

        case 18://Floor East/South Corner
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove North Wall from object:
            if (!removeType(temp, "Wall N")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        case 19://Floor East/North Corner
            //remove ceiling from object:
            if (!removeType(temp, "Ceiling")) {break;}
            //remove South Wall from object:
            if (!removeType(temp, "Wall S")) {break;}
            //remove West Wall from object:
            if (!removeType(temp, "Wall W")) {break;}
           removalSuccess = true; break;

        default://not found
            Debug.LogError ("PrefabGenerator: gridObjectID given not specified in possible gridObjectID list");
            GameObject.Destroy (temp);
            return;
        }

        #if UNITY_EDITOR
        if (removalSuccess) {
            temp.SetActive (true);
            PrefabUtility.CreatePrefab ("Assets/Resources/"+path + tilename + "_" + gridObjectID.ToString("D2") + ".prefab", temp);
            if (DEBUG) {Debug.Log("TilePrefabGenerator: Generated " + path + tilename + "_" + gridObjectID.ToString("D2") + ".prefab");}
        }
        #else
        Debug.LogWarning("TilePrefabGenerator: generatePrefab ran when we're not in the editor and do not have access to editor libraries.");
        #endif
        //get rid of the temporary gameobject used to generate the prefab
        GameObject.Destroy (temp);
    }
}

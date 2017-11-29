using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(GridGenerator))]
public class MapEditor : Editor {
    /* This is code that would generate the 2D grid so we can see it in the Editor. It's more of a convenience, and we cannot get the size of our Actual Grid till runtime, so it cannot run without errors rn. Might remove this file soon.
	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GridGenerator grid = target as GridGenerator;

        grid.GenerateMap(new Vector2Int (MapGrid.xDimension, MapGrid.xDimension));
    }
    */
}

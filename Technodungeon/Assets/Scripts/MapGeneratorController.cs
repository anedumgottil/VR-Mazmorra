using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for the DPad-type Controller in-game. Uses VRTK Events to detect controller Touchpad input, and maps it to four axes. Can be extended.
public class MapGeneratorController : DPadController {
    private int generatorX = 5;
    private int generatorY = 5;

    protected override void doRight() {
        //Debug.Log ("Right Touchpad Press");
        generatorX++;
        genTile(generatorX, generatorY);
    }
    protected override void doLeft() {
        //Debug.Log ("Left Touchpad Press");
        generatorX--;
        genTile(generatorX, generatorY);
    }
    protected override void doUp() {
        //Debug.Log ("Up Touchpad Press");
        generatorY++;
        genTile(generatorX, generatorY);
    }
    protected override void doDown() {
        //Debug.Log ("Down Touchpad Press");
        generatorY--;
        genTile(generatorX, generatorY);
    }

    protected override void doCenter() {
        //Debug.Log ("Touchpad Deadzone Press");
        generatorX = 5;
        generatorY = 5;
    }

    private void genTile(int x, int y) {
        Debug.Log ("Player attempting to generate tile at (" + x + ", " + y + ")");
        if (MapGrid.getInstance ().getGridSpace (x, y) == null) {
            MapGenerator.Instance.setGridSpace (x, y, GridSpace.GridSpaceType.Corridor);
        }
    }

}

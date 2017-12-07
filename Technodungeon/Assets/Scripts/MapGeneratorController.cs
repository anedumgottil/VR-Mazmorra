using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for the DPad-type Controller in-game. Uses VRTK Events to detect controller Touchpad input, and maps it to four axes. Can be extended.
public class MapGeneratorController : DPadController {
    public AudioClip centerPressSound = null;
    GridSpace.GridSpaceType currentType = GridSpace.GridSpaceType.Corridor;

    Vector2Int gridCoords = new Vector2Int(5,5);

    public GameObject generatorPointer = null;//spawns when you click the center touchpad, it will go to the position where the Generator is looking.

    protected override void doRight() {
        //Debug.Log ("Right Touchpad Press");
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (Player.getInstance().getHead().transform.right);
        gridCoords = avoidOutOfBounds(gridCoords + forward);
        genTile(gridCoords.x, gridCoords.y);
    }
    protected override void doLeft() {
        //Debug.Log ("Left Touchpad Press");
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (-Player.getInstance().getHead().transform.right);
        gridCoords = avoidOutOfBounds(gridCoords + forward);
        genTile(gridCoords.x, gridCoords.y);
    }
    protected override void doUp() {
        //Debug.Log ("Up Touchpad Press");
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (Player.getInstance().getHead().transform.forward);
        gridCoords = avoidOutOfBounds(gridCoords + forward);
        genTile(gridCoords.x, gridCoords.y);
    }
    protected override void doDown() {
        //Debug.Log ("Down Touchpad Press");
        Vector2Int forward = MapGrid.roundVectorToCardinalIgnoreY (-Player.getInstance().getHead().transform.forward);
        gridCoords = avoidOutOfBounds(gridCoords + forward);
        genTile(gridCoords.x, gridCoords.y);
    }

    protected override void doCenter() {
        //Debug.Log ("Touchpad Deadzone Press");

        AudioSource playerAudio = Player.getInstance ().GetComponent<AudioSource> ();
        if (playerAudio != null && centerPressSound != null) {
            playerAudio.PlayOneShot (centerPressSound);
        }
        //toggle the type of grid to spawn
        switch (currentType) {
        case (GridSpace.GridSpaceType.Corridor):
            currentType = GridSpace.GridSpaceType.Room;
            break;
        case (GridSpace.GridSpaceType.Room):
            currentType = GridSpace.GridSpaceType.Reactor;
            break;
        case(GridSpace.GridSpaceType.Reactor):
            currentType = GridSpace.GridSpaceType.Corridor;
            break;
        default:
            currentType = GridSpace.GridSpaceType.Corridor;
            break;
        }

        if (generatorPointer == null)
            return;
        
        generatorPointer = Instantiate (generatorPointer);
        Vector3 newpos = MapGrid.getInstance ().getWorldCoordsFromGridCoords (gridCoords);
        newpos = new Vector3 (newpos.x + (MapGrid.getSize () / 2), newpos.y + (MapGrid.getSize () / 2), newpos.z + (MapGrid.getSize () / 2));
        generatorPointer.transform.position = newpos;
        Destroy (generatorPointer, 30f);
    }

    private static Vector2Int avoidOutOfBounds(Vector2Int v) {
        if (v.x >= MapGrid.getInstance().xDimension || v.x < 0 || v.y >= MapGrid.getInstance().yDimension || v.y < 0) {
            //if we go out of bounds, just set the current mapgen tile to the player's grid coords (player should never go out of bounds, so its always valid.)
            return Player.getInstance ().getTeleporterGridLocation ();
        }
        return v;
    }

    private void genTile(int x, int y) {
        Debug.Log ("Player attempting to generate tile at (" + x + ", " + y + ")");
        if (MapGrid.getInstance ().getGridSpace (x, y) == null) {
            MapGenerator.Instance.setGridSpace (x, y, GridSpace.GridSpaceType.Corridor);
        }
    }

}

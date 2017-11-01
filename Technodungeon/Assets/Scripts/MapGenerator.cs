using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class is used to generate all of the GridSpaces and static, unmovable map scenery. The lowest layer of map scenery is created here.
//It holds ADVANCED, STATE-OF-THE-ART AI to generate Map Objects based on their context within the Grid, for example, two hallways created next to each other should automatically merge into one double-wide hallway. This seems like a simple task but is actually a really complicated issue, as you'll see.
//It also keeps track of various GridSpace templates which it'll use to generate the map. Right now, these are hardcoded but //TODO: make the GridSpace templates not hardcoded and instead specified in a format similar to the legacy map.txt flatfile pulled in by the old code in MapLoader.
//You can (and should) use the MapGenerator::setGridSpace(coordinate, type) function to make a grid space on the map. This is what ALL of the map is generated with and is arguably the most important function in the whole freakin' program.
//It can also add stationary items to the map at generation-time, too! So if you want a bunch of Turrets to be generated with the GridSpace for example, you can specify them as a List of items to be added. You'll need to set up those objects beforehand, though, positionally.
//There should be a function to help you figure out Worldspace positions for your StationaryEntities based on Grid coordinates and GridSpacePos ID's, which takes into account stuff like map scale for example to make item creation easier. You can also add them after the fact with Grid::getSpace().GridSpace::addStationary()
public sealed class MapGenerator {

    private static volatile MapGenerator instance;
    private static object syncRoot = new Object();

    private MapGenerator() {}

    private int[][] spaceTemplates = new[]
    { //TODO: remove these hardcoded spaceTemplates and specify them in a flatfile see ticket: #17
        new[]{1, 1, 1, 1, //nothing
            1, 1, 1, 1}, //0
        new[]{2, 2, 2, 2, //just a ceiling and floor
            7, 7, 7, 7}, //1
        new[]{2, 5, 2, 5, //south wall w/ ceiling and floor
            7, 10, 7, 10}, //2
        new[]{6, 2, 6, 2, //north wall w/ ceiling and floor
            11, 7, 11, 7}, //3
        new[]{2, 2, 4, 4, //east wall w/ ceiling and floor
            7, 7, 9, 9}, //4
        new[]{3, 3, 2, 2, //west wall w/ ceiling and floor
            8, 8, 7, 7}, //5
        new[]{3, 3, 4, 4, //two wall tube pointing n/s w/ ceiling and floor
            8, 8, 9, 9}, //6
        new[]{6, 5, 6, 5, //two wall tube pointing e/w w/ ceiling and floor
            11, 10, 11, 10}, //7
        new[]{17, 3, 6, 2, //north/west corner w/ ceiling and floor
            13, 8, 11, 7}, //8
        new[]{6, 2, 19, 4, //north/east corner w/ ceiling and floor
            11, 7, 15, 9}, //9
        new[]{3, 16, 2, 5, //south/west corner w/ ceiling and floor
            8, 12, 7, 10}, //10
        new[]{2, 5, 4, 18, //south/east corner w/ ceiling and floor
            7, 10, 9, 14}, //11
        new[]{3, 16, 4, 18, //south/east/west  deadend w/ ceiling and floor  - looks like: U
            8, 12, 9, 14}, //12
        new[]{3, 16, 4, 18, //south/west/north deadend w/ ceiling and floor  - looks like: C
            8, 12, 9, 14}, //13
        new[]{17, 3, 19, 4, //north/east/west  deadend w/ ceiling and floor  - looks like: upside-down U
            13, 8, 15, 9}, //14
        new[]{6, 5, 19, 18, //north/east/south deadend w/ ceiling and floor  - looks like: backwards C
            11, 10, 15, 14} //15
    };

    //most important function:
    //NEVER pass a gstype of None=0 for the initial setGridSpace call, it's an undefined operation; None is used to specify the call is a child recursive setGridSpace call, and is only used internally. an improvement may be to allow it if we want to just update the tile based on it's neighbors, but we'd need to handle the null thing below
    public void setGridSpace(int x, int y, GridSpace.GridSpaceType gstype) {
        //First, check our Grid to determine if we have a GridSpace there
        GridSpace newGS = Grid.getInstance().getGridSpace(x,y);
        if (gstype == 0) {// IF gstype == 0 we are RECURSING
            //if we have GridSpaceType of 0, we know that this is a child recursive call from the AI matrix below... and we should not try to make a new GridSpace
            //for if this grid coordinate is null, something has gone HORRIBLY WRONG
            if (newGS == null) {
                Debug.LogError ("Error: MapGenerator tried to recursively set null GridSpace (" + x + ", " + y + ") child, when there SHOULD be something here!");
                return;
            }
        }
        if (newGS == null) {//if we're recursing, this will NEVER happen, since we check above.
            //no GS exists, make a new one.
            newGS = new GridSpace(new Vector2(x, y));
        }//now we will just edit the associated GridObjects for the GridSpace so we do not mess up any of it's associated parameters, just in case it has attached stationaryEntities or whatever

        //HERE COMES THE DAT AI

        //Important Note: after this is ran, we have modified the Grid! So do not operate on newGS anymore or it won't be saved.
    }

    //this function is where the AI magic happens
    //Note: we carefully avoid a race condition here in our recursive subcall to analyzeNeighbors by only checking null values in the Grid, but if someone modifies the Grid during map generation we'll be in trouble
    private void analyzeNeighbors(int x, int y, GridSpace.GridSpaceType gstype, GridSpace current) {
        //this is where the MapGenerator needs to intelligently determine the type of GridSpace to spawn, with the help of it's handy-dandy helper function
        bool adjacentWestTile, adjacentEastTile, adjacentNorthTile, adjacentSouthTile = false; // need an array of variables to store state
        checkNeighborGridSpaces (x, y, out adjacentWestTile, out adjacentEastTile, out adjacentNorthTile, out adjacentSouthTile);
        //BEGIN TO DECIDE ON GRIDSPACE LAYOUT:
        if (!adjacentWestTile && !adjacentEastTile && !adjacentNorthTile && !adjacentSouthTile) {
            //base case: we have no surrounding tiles. Just make ourselves into a floor and a ceiling. TODO: make this a 4 wall block instead of a no-wall block.
            applyGridSpaceConfiguration (current, gstype, 1);


            //////////////////////////////////SINGULAR NEIGHBORS//////////////////////////////////
        } else if (adjacentWestTile && !adjacentEastTile && !adjacentNorthTile && !adjacentSouthTile) {
            //WEST TILE
            //we have a just 1 tile to the west. we need to make ourselves into a reversed C shape to accomodate
            applyGridSpaceConfiguration (current, gstype, 15);
            //thankfully, due to us only using cardinal directions here on our graph, we can COMPLETELY eliminate the recursive call flood by just not recursing twice after a change (yay math)
            if (gstype != (GridSpace.GridSpaceType)0) {//if this is not a recursive call, we'll need to recurse to update the tile to our west of the changes we just made to ourselves
                //now we will call this same function over THAT tile, so we force it to update it's configuration to reflect THIS tile
                analyzeNeighbors (x - 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x - 1, y, false));//specify recursive sub-call with gstype 0
            }
        } else if (!adjacentWestTile && !adjacentEastTile && !adjacentNorthTile && adjacentSouthTile) {
            //SOUTH TILE
            applyGridSpaceConfiguration (current, gstype, 14);
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x, y - 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y - 1, false));//specify recursive sub-call with gstype 0
            }
        } else if (!adjacentWestTile && adjacentEastTile && !adjacentNorthTile && !adjacentSouthTile) {
            //EAST TILE
            applyGridSpaceConfiguration (current, gstype, 13);
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x + 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x + 1, y, false));//specify recursive sub-call with gstype 0
            }
        }else if (!adjacentWestTile && !adjacentEastTile && adjacentNorthTile && !adjacentSouthTile) {
            //NORTH TILE
            applyGridSpaceConfiguration (current, gstype, 12);
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x, y + 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y + 1, false));//specify recursive sub-call with gstype 0
            }

            //////////////////////////////////DUAL NEIGHBORS//////////////////////////////////
            //////////////////////////////////   "TUBES"    //////////////////////////////////
        } else if (adjacentWestTile && adjacentEastTile && !adjacentNorthTile && !adjacentSouthTile) {
            //EAST AND WEST TILE
            applyGridSpaceConfiguration (current, gstype, 7);
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x + 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x + 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x - 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x - 1, y, false));
            }
        } else if (!adjacentWestTile && !adjacentEastTile && adjacentNorthTile && adjacentSouthTile) {
            //NORTH AND SOUTH TILE
            applyGridSpaceConfiguration (current, gstype, 6);
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x, y+1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y+1, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x, y-1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y-1, false));
            }


            //////////////////////////////////DUAL NEIGHBORS//////////////////////////////////
            //////////////////////////////////   CORNERS    //////////////////////////////////
        } else if (adjacentWestTile && !adjacentEastTile && adjacentNorthTile && !adjacentSouthTile) {
            //WEST AND NORTH TILE
            applyGridSpaceConfiguration (current, gstype, 11);//s/e corner
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x - 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x - 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x, y + 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y + 1, false));
            }
        } else if (!adjacentWestTile && adjacentEastTile && adjacentNorthTile && !adjacentSouthTile) {
            //EAST AND NORTH TILE
            applyGridSpaceConfiguration (current, gstype, 10);//s/w corner
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x + 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x + 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x, y + 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y + 1, false));
            }
        } else if (adjacentWestTile && !adjacentEastTile && !adjacentNorthTile && adjacentSouthTile) {
            //WEST AND SOUTH TILE
            applyGridSpaceConfiguration (current, gstype, 9);//n/e corner
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x - 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x - 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x, y - 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y - 1, false));
            }
        } else if (!adjacentWestTile && adjacentEastTile && !adjacentNorthTile && adjacentSouthTile) {
            //EAST AND SOUTH TILE
            applyGridSpaceConfiguration (current, gstype, 8);//n/w corner
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x + 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x + 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x, y - 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y - 1, false));
            }


            //////////////////////////////////THREE NEIGHBORS/////////////////////////////////
            ////////////////////////////////// SINGLE-WALLS //////////////////////////////////
        } else if (adjacentWestTile && adjacentEastTile && adjacentNorthTile && !adjacentSouthTile) {
            //WEST AND EAST AND NORTH TILE
            applyGridSpaceConfiguration (current, gstype, 2);//south wall
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x - 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x - 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x + 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x + 1, y, false));
                analyzeNeighbors (x, y + 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y + 1, false));
            }
        } else if (adjacentWestTile && adjacentEastTile && !adjacentNorthTile && adjacentSouthTile) {
            //WEST AND EAST AND SOUTH TILE
            applyGridSpaceConfiguration (current, gstype, 3);//north wall
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x - 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x - 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x + 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x + 1, y, false));
                analyzeNeighbors (x, y - 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y - 1, false));
            }
        } else if (adjacentWestTile && !adjacentEastTile && adjacentNorthTile && adjacentSouthTile) {
            //WEST AND NORTH AND SOUTH TILE
            applyGridSpaceConfiguration (current, gstype, 4);//east wall
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x - 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x - 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x, y - 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y - 1, false));
                analyzeNeighbors (x, y + 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y + 1, false));
            }
        } else if (!adjacentWestTile && adjacentEastTile && adjacentNorthTile && adjacentSouthTile) {
            //EAST AND SOUTH AND NORTH TILE
            applyGridSpaceConfiguration (current, gstype, 5);//west wall
            if (gstype != (GridSpace.GridSpaceType)0) {//if we're not recursive already, recurse to update neighbors:
                analyzeNeighbors (x + 1, y, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x + 1, y, false));//specify recursive sub-call with gstype 0
                analyzeNeighbors (x, y + 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y + 1, false));
                analyzeNeighbors (x, y - 1, (GridSpace.GridSpaceType)0, Grid.getInstance ().getGridSpace (x, y - 1, false));
            }
        }
    }

    //modifies the GridSpace current's array of GridObjects to reflect the new configuration, then adds the GridSpace current to the Grid properly, overwriting the old one.
    //NOTE: requires that the MapLoader has completed loading all map resources before running!
    private void applyGridSpaceConfiguration(GridSpace current, GridSpace.GridSpaceType gstype, int configuration) {
        if (current == null) {
            //severe error/bug
            Debug.LogError ("Error: Tried to apply a GridSpace configuration to a null GridSpace... this should never happen!");
        }
        if (!MapLoader.isMapResourcesLoadComplete ()) {
            //we don't have the resources required to do any GridSpace generation, so just give up.
            Debug.LogError ("Error: MapGenerator found that the MapLoader didn't do it's job, the slacker");
            return;
        }

        current.gridSpaceConfiguration = configuration;
        int reggaeton = 0;
        foreach (int tileID in spaceTemplates[configuration]) {
            //generate a GridSpace similar to the way that MapLoader does, GridObject by GridObject for each ID.
            //TODO: CRITICAL: update this function so that instead of generating blocks, it generates tiles!!! we need the Tile prefabs complete before we can do this though.
            if (reggaeton > 7) {
                //too many positions, this would happen if we loaded in too many indices to our templates, which should be corrected by the MapLoader in advance so... error.
                Debug.LogError ("Error: too many MapGenerator GridSpace positions on the dance floor!");
                return;
            }
            current.setBlock (MapLoader.getBlockInstance (tileID), (GridSpace.GridPos)reggaeton);
            reggaeton++;
        }
        //if we have gstype of 0, just don't edit the gst (it stays the same)
        if (gstype != (GridSpace.GridSpaceType)0) {
            current.setGridSpaceType (gstype);
        } else {
            //DEBUG: remove me please
            Debug.Log ("gridspaceconfig inheriting gst: " + (int)current.getGridSpaceType ());
        }

        //get the Grid position of our GridSpace that we are updating
        Vector2 gridPos = current.getGridPosition ();

        //finally, register our GridSpace to the grid.
        Grid.getInstance ().registerGridSpace ((int)gridPos.x, (int)gridPos.y, current);
    }

    //this function just helps me figure out which of the four cardinal directions around a grid coordinate are occupied.
    private void checkNeighborGridSpaces(int x, int y, out bool adjacentWestTile, out bool adjacentEastTile, out bool adjacentNorthTile, out bool adjacentSouthTile) {
        if (Grid.getInstance().getGridSpace(x-1,y, true) != null) {
            adjacentWestTile = true;
        } else {
            adjacentWestTile = false;
        }
        if (Grid.getInstance().getGridSpace(x+1,y, true) != null) {
            adjacentEastTile = true;
        } else {
            adjacentEastTile = false;
        }
        if (Grid.getInstance().getGridSpace(x,y+1, true) != null) {
            adjacentNorthTile = true;
        } else {
            adjacentNorthTile = false;
        }
        if (Grid.getInstance().getGridSpace(x,y-1, true) != null) {
            adjacentSouthTile = true;
        } else {
            adjacentSouthTile = false;
        }
    }

    //singleton stuff:
    public static MapGenerator Instance
    {
        get 
        {
            if (instance == null) 
            {
                lock (syncRoot) 
                {
                    if (instance == null) 
                        instance = new MapGenerator();
                }
            }

            return instance;
        }
    }
    public MapGenerator getInstance() {
        return instance;
    }
}
//this class creation was fueled in a 7-hour code-binge by G-Eazy, Drum and Bass, reggaeton and a variety of other somewhat-dangerous things
//I'd like to thank my mom, my buddy Joe Demme, Drake/his producer: 40, and my cat for making it all possible
//<3
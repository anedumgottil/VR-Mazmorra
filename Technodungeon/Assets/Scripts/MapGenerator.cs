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
            1, 1, 1, 1}, 
        new[]{2, 2, 2, 2, //just a ceiling and floor
            7, 7, 7, 7}, 
        new[]{2, 5, 2, 5, //south wall w/ ceiling and floor
            7, 10, 7, 10}, 
        new[]{6, 2, 6, 2, //north wall w/ ceiling and floor
            11, 7, 11, 7}, 
        new[]{2, 2, 4, 4, //east wall w/ ceiling and floor
            7, 7, 9, 9}, 
        new[]{3, 3, 2, 2, //west wall w/ ceiling and floor
            8, 8, 7, 7}
    };

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
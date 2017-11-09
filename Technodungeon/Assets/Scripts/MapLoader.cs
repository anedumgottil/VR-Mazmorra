using UnityEngine;
//using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Linq;
using VRTK;

//This class handles prefab loading, pooling and storage
//Used to generate the prefab parents that will be used to generate the map with the map generator
//use this class if you need to create a new object, since it automatically pools objects (or at least, it should. we need an interface for this)

public class MapLoader : MonoBehaviour 
{
    public GameObject VRTKSDKManagerGO = null;
    public int SDKLoaderTimeout = 5;
    private VRTK_SDKManager VRTKSDKManager = null;

    public string blocksPath;
    public string mapPath;

    public bool generatePrefabsFromFile = false; // generates the Block prefab files from the blocks.txt flatfile that outlines their design, recently factored out, keep false. useless
    public bool generateGridFromFile = true; //places the MapGrid Tile prefabs on the grid according to the map file (kind of buggy, difficult to create a map layout in the text file right now)

    private static bool mapResourcesLoadComplete = false; //TODO: this flag is set to true once loadBlocks()/parseBlocks() and loadTiles()/parseTiles() and parseBlockMap() is complete (not set correctly yet)

    //blockTypes is not populated when we don't use the automatic generator
    private static List<Block> blockTypes = new List<Block>(); //Blocks that make up the Prefab Pool. //TODO: make this a unique pool... considering we now have to use it for generating every block, apparently (serialization fell thru)
                                                        //I recommend defining a hash function and an equality comparison function, maybe a ToString and others for Block or at least GridObject to accomplish this.
    private static List<Tile> tileTypes = new List<Tile>();
    private static Dictionary<string, GameObject> gamePrefabs;//These are all the prefabs used in the game, as an object pool. Instantiate these to clone them and use them elsewhere via getPrefabInstance().
    private static GameObject prefabParent;

    //generate our Blocks from a text file, and create Prefabs from them to populate our Prefab Pool. This is ran only when the Unity Editor is available, and should only be ran occasionally, as it's time consuming and generates meta commit conflicts
    public void parseBlocks() {
        //for both TXT files, the matrices are seperated by a single line with a | character, and a block/tile begins with a single line { and vice versa.
        //read in the available blocks for our overall map
        //these are stored in blocks.txt as a series of 2D matrices of X's and ~'s.
        //Each matrix reperesents a binary configuration of a single layer of microblocks.
        string blocks = readString(blocksPath);
        int charcount = 0; //for debug
        int linecount = 1;
        int mbxcount = 0;// how far along we are in a line
        int mbycount = 0;// how many lines down we are
        int mbzcount = 0;// how many layers down we are
        char lastChar = 'A';
        Block current = null;
        foreach (char c in blocks) {
            charcount++;
            if (c == '{') {
                //start of block
                //first check we arent already within a block
                if (current != null) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to start new block inside pre-existing block frame lastChar = '"+lastChar+"("+(int)lastChar+")"+"' line = "+linecount+" charcount = "+charcount);
                    return;
                }
                mbxcount = 0;
                mbycount = 0;
                mbzcount = 0;
                current = new Block ((GridSpace)null);//passing null first param, we don't have a GridSpace to assign it to it, and all parent Block will have NULL parents of their own, to differentiate them 
                lastChar = c;
                continue;
            } else if (c == 'X') {
                //microblock registered
                //first, check if we are within a {} block
                if (current == null) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to set microblock outside of a block. lastChar = '"+lastChar+"("+(int)lastChar+")"+"' line = "+linecount+" charcount = "+charcount);
                    return;
                }
                //make sure we only have Block.mbDivision mb per x line
                if (mbxcount >= Block.mbDivision) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to create a new microblock on x-line, but we already have max amount. mbx = "+mbxcount+" mby = "+mbycount+" mbz = "+mbzcount+" lastChar = '"+lastChar+"("+(int)lastChar+")"+"' line = "+linecount+" charcount = "+charcount);
                    return;
                }
                current.newMicroBlock (mbxcount, Block.mbDivision-1-mbycount, mbzcount);
                lastChar = c;
                mbxcount++;
                continue;
            } else if (c == '~') {
                //no microblock registered
                //first, check if we are within a {} block
                if (current == null) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to set anti-microblock outside of a block. lastChar = '"+lastChar+"("+(int)lastChar+")"+"' line = "+linecount+" charcount = "+charcount);
                    return;
                }
                //make sure we only have Block.mbDivision mb per x line
                if (mbxcount >= Block.mbDivision) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to create a anti-microblock on x-line, but we already have max amount. mbx = "+mbxcount+" mby = "+mbycount+" mbz = "+mbzcount+" lastChar = '"+lastChar+"("+(int)lastChar+")"+"' line = "+linecount+" charcount = "+charcount);
                    return;
                }
                //make sure there is no microblock there
                current.setMicroBlock (null, mbxcount, Block.mbDivision-1-mbycount, mbzcount);
                lastChar = c;
                mbxcount++;
                continue;
            } else if (c == '\n') {
                charcount = 0;
                linecount++;
                //newline found, this could be start of a new line, matrix, or block.
                if (lastChar == 'X' || lastChar == '~') {
                    //last char was a microblock. we should increment our y because we're going down lines in our matrix.
                    mbycount++;
                    if (mbycount != 4) {
                        mbxcount = 0; // we dont want to reset mbx on a ylevel change because  x4,y4 is checked to verify matrix completion during matrix seperator phase
                    }
                }
                lastChar = c;
                continue;
            } else if (c == ' ') {
                continue;//straight up ignore spaces
            } else if (c == '|') {
                //matrix seperator found, this means we're on a new z-level
                //make sure we already handled an Block.mbDivisionxBlock.mbDivision matrix
                if (mbxcount < Block.mbDivision || mbycount < Block.mbDivision) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to move to another z-level, but the prior one wasn't finished yet. mbx = "+mbxcount+" mby = "+mbycount+" mbz = "+mbzcount+" lastChar = '"+lastChar+"("+(int)lastChar+")"+"' line = "+linecount+" charcount = "+charcount);
                    return;
                }
                mbycount = 0;
                mbxcount = 0;
                mbzcount++;
                lastChar = c;
                continue;
            } else if (c == '}') {
                //end of block marker found
                //make sure we handled all matrices
                if (mbxcount < Block.mbDivision|| mbycount < Block.mbDivision || mbzcount < Block.mbDivision - 1) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to end a block before it was finished. mbx = "+mbxcount+" mby = "+mbycount+" mbz = "+mbzcount+" lastChar = '"+lastChar+"("+(int)lastChar+")"+"' line = "+linecount+" charcount = "+charcount);
                    return;
                }
                //package up this block, it's ready to go
                blockTypes.Add (current);//store the block in Block Pool
                current.getGameObj().transform.SetParent (prefabParent.transform);
                MapLoader.addPrefab(current.getGameObj().name, current.getGameObj());//add GameObject template to Prefab Pool.
                if (generatePrefabsFromFile) {
                    BlockPrefabGenerator.generatePrefab (current);//generates a Prefab using the PrefabGenerator and saves it to disk.
                }
                lastChar = 'A';//reset lastChar
                mbzcount = 0;
                mbycount = 0;
                mbzcount = 0;
                current = null;
                continue;
            } else if ((int)c == 13) {//line feed... windows and it's /r/n madness
                charcount--;
                continue;
            } else {
                Debug.LogError ("Error: loading blocks failed, syntax error: unknown character found: '"+(int)c+"'");
                return;
            }
        }
    }

    //load block prefabs in from the Resources folder and store them in our Prefab pool. This is used when the game is compiled.
    public void loadBlocks() {
        //TODO: fill this out
    }

    /* TODO: Implement MapGrid Tile serialization upon launch so we do not need to generate the Prefabs every time.
    public void loadBlocks() {
        //loads pre-generated prefabs from the prefabs folder, and sticks them into the blockTypes array to be used by a later script, or the parseMaps() function.
        //you can use this to avoid having to generate prefabs on launch, eventually this will be used to allow us to create a build that doesn't require Editor libraries
        Object[] tempPrefabs = Resources.LoadAll("Prefabs", typeof(PrefabType));
        foreach (Object a in tempPrefabs) {
            blockTypes.Add ((Block)a);
        }
    }
*/

    //load tile prefabs in from the Resources folder and store them in our Prefab pool. This is ALWAYS used as Tiles are not procedurally generated.
    public void loadTiles() {
        GameObject[] tilez = {};
        Debug.Log ("MapLoader - Attempting to load Tile prefabs from Resources folder on disk...");
        try {
            tilez = (GameObject[]) Resources.LoadAll<GameObject>("Prefabs/Tiles/SciFiTiles/");
            Debug.Log ("MapLoader - Loaded "+tilez.Count()+" Tile Prefabs into prefab pool");
        }
        catch (Exception e)
        {
            Debug.LogError("Error: MapLoader - Loading Tile prefabs from Resources failed with the following exception: ");
            Debug.LogError(e);
        }
        int i = 0;
        foreach (GameObject tile in tilez) {
            Debug.Log ("Got tile["+i+"]: " + tile.name);
            tile.SetActive (false);
            addPrefab (tile.name, tile);
            //tile.transform.SetParent (prefabParent.transform);//TODO: the tiles can't have their parent set because of "corruption" so they clutter up the inspector... find a way to group them and hide them after Resources.LoadAll...
            Tile t = new Tile ((GridSpace)null);
            t.setGameObj (tile);
            tileTypes.Add (t);
            i++;
        }

    }
        
    //This parses the block map file to load in the GridSpaces that will be stored in the MapGrid. It is ALWAYS used as it is meant to run the same regardless of platform.
    public void parseBlockMap() {
        //parseblocks or loadblocks needs to be ran first, this function uses the Prefab Pool to generate it's blocks, even if they've been auto-generated by the MapGen, for portability reasons.
        //since each 2mx2m grid tile is made of 8 blocks, they are reperesented by their block ID number in 2 lists of 4 ID's.
        //we follow the standard ordering LoNW = 0, LoSW = 1, LoNE = 2, LoSE = 3, HiNW = 4, HiSW = 5, HiNE = 6, HiSE = 7
        //their block ID number is just whatever order they are written in the blocks.txt file, I couldve made it an XML file to hardcode this but... meh.

        string map = readString(mapPath);
        bool inTile = false;
        int blockpos = 0;
        int numtiles = 0;
        char lastChar = 'A';
        int chars = 0;
        int lines = 0;//dont use chars for debug print purposes, it includes windows linefeeds so we can iterate map more effectively
        int skips = 0;
        int x = 0;
        int y = 0;
        GridSpace current = null;
        foreach (char c in map) {
            chars++;
            if (c == '{') {
                //begin grid tile
                if (inTile) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: tried to begin a grid tile inside a grid tile");
                    return;
                }
                blockpos = 0;
                lastChar = c;
                inTile = true;
                current = new GridSpace ();//TODO: CRITICAL: we do not specify a position, as the SetGridSpace function will do it for us! :) but it doesn't exist yet :(
                Debug.Log ("Start of tile");
                continue;
            } else if (c == '}') {
                //we're done here, wrap up
                if (!inTile || blockpos != 8) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: attempted to close non-existent/unfinished grid tile");
                    return;
                }
                lastChar = c;
                inTile = false;
                numtiles++;
                Debug.Log ("End of tile");
                continue;
            } else if (c == ' ') {
                continue; //ignore spaces
            } else if ((int)c == 13) {
                continue; //windows linending
            } else if ((int)c == 10) {
                lines++;
                //normal, sane line-ending.
                if (lastChar == '{' || lastChar == '}' || (int)lastChar == 10) {
                    continue;//don't pay attention to line endings immediately following start or end of block, or double/triple/etc. newlines
                }
                blockpos++;
                if (blockpos > 8) {
                    Debug.LogError ("Error: loading blocks failed, syntax error: too many blocks for grid tile");
                    return;
                }
                lastChar = (char)10;
                continue;
            } else {
                if (skips > 0) {
                    //we have skips, which means we dont want these characters.
                    skips--;
                    continue;
                }
                //we have some other character... probably a number, which is what we want. So we must parse it.
                //strip the number off the string with regex
                string number = Regex.Match (map.Substring (chars - 1), @"^\d+").ToString ();
                int parsedNum = 0;//minor TODO: should we sanitize parsedNum somehow? it's technically 'user input'

                if (!System.Int32.TryParse (number, out parsedNum)) {
                    //parsing attempt failed.... skip this char, try again next time...
                    Debug.LogError ("Error: loading blocks warning: couldn't parse characters into Int value... skipping '" + number + "'");
                }
                if (parsedNum < 0 || parsedNum >= gamePrefabs.Count) {
                    Debug.LogError ("Error: MapLoader tried to create a block number by an ID that we do not have in the storage");
                }
                //success at parse, we need to remove this number from our string so we can continue on...
                skips = number.Length - 1;
                lastChar = c;
                //create block in GridSpace using this data
                current.setBlock (getBlockInstance (parsedNum), (GridSpace.GridPos) blockpos);

                if (blockpos == 7) {//we have a full GridSpace, generate it, then send it off to the MapGrid.
                    //we will add GridSpace to map via x:0 -> x:parentGrid.xDimension
                    //then roll over 1 on the y and start again. 
                    int calculation = (int)numtiles / MapGrid.getInstance().xDimension;//number of y, basically just floors the value
                    if (calculation > MapGrid.getInstance().yDimension) {
                        Debug.LogError ("Error: loading blocks failed: Too many GridSpaces on the dance floor!");
                        return;
                    }
                    Debug.Log ("Attempting to create GridSpace at [" + /*(int)(numtiles - (calculation * MapGrid.getInstance().xDimension)) * MapGrid.getSize () +*/ "("+x+")," + /*(int)calculation * MapGrid.getSize () +*/ "("+y+")] at position " + blockpos + ", Tile Type: " + parsedNum + " numTiles=" + numtiles);
//                    MapGrid.getInstance().registerSpace((int)(numtiles - (calculation * MapGrid.getInstance().xDimension)) * MapGrid.getSize (), (int)calculation * MapGrid.getSize (), current);
                    MapGrid.getInstance().registerGridSpace(x, y, current);
                    x++;
                    if (x >= MapGrid.getInstance ().xDimension) {
                        y++;
                        x = 0;
                    }
                    current = null;
                }
            }
                
        }
    }

    //prefab accessor. gets prefab if it exists, returns null if it doesn't. If it doesn't exist it also emits a warning message to Debug.
    //Does not clone the prefab with Instantiate. <---------- IMPORTANT! you should probably be using getPrefabInstance because of this. 
    public static GameObject getPrefab(string name) {
        GameObject temp = gamePrefabs[name];
        if (temp == null) {
            Debug.LogWarning ("Warn: Tried to make MapLoader get a Prefab parent by a name that doesn't exist");
        }
        return temp;
    }

    //creates a new instance (clone) of parent prefab upon being given a name. This is the most important Prefab function, and is used to spawn every new block object
    //dont forget to set the parent!
    public static GameObject getPrefabInstance(string name) {
        GameObject template = getPrefab (name);
        template.transform.parent = null;
        return Instantiate (template);//clone the template
    }

    //gets a clone of a Block in our block pool
    public static Block getBlockInstance(int id) {
        if (id < 0 || id >= blockTypes.Count) {
            Debug.Log ("Error: getBlockInstance fed out of range id");
            return null;
        }
        return new Block(blockTypes [id]);
    }

    //gets a clone of a Tile in our tile pool
    public static Tile getTileInstance(int id) {
        if (id < 0 || id >= tileTypes.Count) {
            Debug.Log ("Error: getTileInstance fed out of range id");
            return null;
        }
        return new Tile(tileTypes [id]);
    }

    //Add a GameObject to be used as a template in our object pool.... you should probably not use this unless you're generating the map, we want to keep this pool small.
    //note that even though this object might not actually be a prefab, once it's in the pool it'll be used like one - duplicated with Instantiate, and such. So we'll just call it a prefab.
    //if the pool already has one by the same name, nothing occurs.
    //Note: We don't set the parent here anymore, it'll set the parent when you get the instance back, due to the fact that Resources.LoadAll objects cannot have their parent set without data corruption.
    //Because of this, anything that uses AddPrefab in the past has been updated to set the parent itself correctly...
    public static void addPrefab(string name, GameObject prefab) {
        gamePrefabs.Add (name, prefab);
    }

    //this method will actually MODIFY the object pool so you DEFINITELY should not use this unless you're the map generator or have a good reason to modify prefabs
    public static void setPrefab(string name, GameObject prefab) {
        gamePrefabs[name] = prefab;
    }

    //returns true if we've correctly loaded all prefabs, and generated all their Blocks and Tiles based on the flatfiles.
    //This says nothing about the state of other loaded resources, for example, entities.... so be careful.
    public static bool isMapResourcesLoadComplete() {
        return mapResourcesLoadComplete;
    }
        

    private static string readString(string path) {
        //Read the text from directly from a txt file
        StreamReader reader = new StreamReader(path); 
        string temp = reader.ReadToEnd(); //warning, this code potentially buffers a lot of memory
        reader.Close();
        return temp;
    }

    void Start() {
        mapPath = Application.dataPath + "/StreamingAssets/" + "map.txt";
        blocksPath = Application.dataPath + "/StreamingAssets/" + "blocks.txt";
        gamePrefabs = new Dictionary<string, GameObject>();
        prefabParent = new GameObject ("Prefab Pool");
        prefabParent.transform.position = Block.DEFAULT_POSITION;
        prefabParent.SetActive (false);
        VRTKSDKManager = VRTKSDKManagerGO.GetComponent<VRTK_SDKManager> ();

        this.parseBlocks ();

        if (this.generateGridFromFile) {
            this.parseBlockMap ();
        }

        this.loadTiles ();
        //TODO: load in all prefabs using LoadAllResources... 
        //TODO: load in all tiles using the prefabs you load in, along with the Tile configuration flatfile (almost done, already)
        //TODO: load in the tile configuration settings from flatfile and feed to MapGenerator via a function so we don't have to hardcode them as an array like they are right now

        mapResourcesLoadComplete = true;

        StartCoroutine (generateStarterMap());

    }

    IEnumerator generateStarterMap() {
        if (VRTKSDKManager != null && VRTKSDKManager.loadedSetup != null) {
            MapGenerator.Instance.generateStarterMap (); //generate immediately.
            yield break;
        } else {
            if (VRTKSDKManager == null) {
                Debug.LogError ("Error: MapLoader - SDK Manager null.");
            } else if (VRTKSDKManager.loadedSetup == null) {
                //we must wait a bit for the setup to be loaded.
                int count = 0;
                while (VRTKSDKManager.loadedSetup == null) {
                    if (count > SDKLoaderTimeout) {
                        Debug.LogError ("Fatal Error: MapLoader - Couldn't generate map! SDKManager would not load SDK setup after a long time!");
                        yield break;
                    }
                    Debug.Log ("MapLoader: Waiting for the SDKManager to load an SDK setup... iteration " + count);
                    yield return new WaitForSeconds(1.0f);
                    count++;
                }
                //generate the map, we have a loadedSetup != null
                Debug.Log("MapLoader: Wait ended. SDK setup found. Generating starter map...");
                MapGenerator.Instance.generateStarterMap (); //generate immediately.
                yield break;
            }

        }
    }

}
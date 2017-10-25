using UnityEngine;
//using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MapLoader : MonoBehaviour 
{
    public Grid parentGrid = null;
    public string blocksPath = "Assets/Resources/blocks.txt";
    public string mapPath = "Assets/Resources/map.txt";
    public bool generatePrefabsFromFile = false; // generates the Grid Tile prefabs from the blocks.txt flatfile that outlines their design, and adds them to the game.
                                                 // if this is false, the script will instead use the pregenerated prefabs (whatever is in prefabs directory at this time)
    public bool generateGridFromFile = true; //places the Grid Tile prefabs on the grid according to the map file (kind of buggy, difficult to create a map layout in the text file right now)

    private List<Block> blockTypes = new List<Block>();
    public static List<GameObject> gamePrefabs;//the gameObj converted to a prefab for re-instantiation, if necessary.

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
                current = new Block (null);
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
                blockTypes.Add (current);
                if (generatePrefabsFromFile) {
                    BlockPrefabGenerator.generatePrefab (current, new Vector3(-10f, -10f, -10f));//TODO: CRITICAL! What happens if we don't get our prefabs? Make it load prefabs in from the Assets folder in that case.
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

    public void parseMaps() {
        //read in the map itself (ALWAYS RUN PARSEBLOCKS FIRST! - perhaps I should just stick this in that function?)
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
                string number = Regex.Match(map.Substring(chars-1), @"^\d+").ToString();
                int x = 0;

                if (!System.Int32.TryParse(number, out x))
                {
                    //parsing attempt failed.... skip this char, try again next time...
                    Debug.LogError ("Error: loading blocks warning: couldn't parse characters into Int value... skipping '"+number+"'");
                }
                if (x < 0 || x >= blockTypes.Count) {
                    Debug.LogError ("Error: MapLoader tried to create a block number by an ID that we do not have in the storage");
                }
                //success at parse, we need to remove this number from our string so we can continue on...
                skips = number.Length-1;
                //create block in world using this data
                //we will generate them x = 0 -> x = parentGrid.xDimension
                //then roll over 1 on the y and start again.
                int calculation = (int)numtiles/parentGrid.xDimension;//number of y, basically just floors the value
                if (calculation > parentGrid.yDimension) {
                    Debug.LogError ("Error: loading blocks failed: Too many blocks on the dance floor!");
                    return;
                }
                Debug.Log("Attempting to create Grid Tile at ["+(int)(numtiles-(calculation*parentGrid.xDimension))*Grid.getSize()+","+(int)calculation*Grid.getSize()+"] at position "+blockpos+", Tile Type: "+x+" numTiles="+numtiles);
                parentGrid.setBlock(blockTypes[x], (int)(numtiles-(calculation*parentGrid.xDimension))*Grid.getSize(), (int)calculation*Grid.getSize(), (GridSpace.GridPos)blockpos);
                lastChar = c;
            }
                
        }
    }
       
/* TODO: Implement Grid Tile serialization upon launch so we do not need to generate the Prefabs every time.
    public void loadBlocks() {
        //loads pre-generated prefabs from the prefabs folder, and sticks them into the blockTypes array to be used by a later script, or the parseMaps() function.
        //you can use this to avoid having to generate prefabs on launch, eventually this will be used to allow us to create a build that doesn't require Editor libraries
        Object[] tempPrefabs = Resources.LoadAll("Prefabs", typeof(PrefabType));
        foreach (Object a in tempPrefabs) {
            blockTypes.Add ((Block)a);
        }
    }
*/


    private static string readString(string path) {
        //Read the text from directly from the blocks.txt file
        StreamReader reader = new StreamReader(path); 
        string temp = reader.ReadToEnd(); //warning, this code potentially buffers a lot of memory
        reader.Close();
        return temp;
    }

    void Start() {
        gamePrefabs = new List<GameObject>();

        this.parseBlocks ();

        if (this.generateGridFromFile) {
            this.parseMaps ();
        }
    }

}
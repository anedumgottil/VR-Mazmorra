using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {
    private static GameObject instanceGO;
    private static GridGenerator instance;
    public Transform tilePrefab;
    private Vector2Int mapSize;
    [Range(0, 1)]
    public float outlinePercent;
    public string containerName;
    private WaitForSeconds waitASecond;
    private Transform containerTransform;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    public int seed = 10;

    public void Start() {
        instance = this;
    }

    public static GridGenerator getInstance() {

        if (instance == null) {
            instanceGO = new GameObject();
            instance = instanceGO.AddComponent<GridGenerator>();
            instanceGO.name = "(singleton) "+ typeof(GridGenerator).ToString();
            return instance;//fix this! NOT A TRUE SINGLETON, cannot just return new because of Unity MonoBehaviour. TODO: factor our MonoBehaviour from this script.
        } else {
            return instance;
        }
    }

    public void GenerateMap(Vector2Int mapSize)
    {
        waitASecond = new WaitForSeconds(1f);
        this.mapSize = mapSize;
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));
        if(GameObject.Find(containerName))
        {
            DestroyImmediate(GameObject.Find(containerName).gameObject);
        }

        Transform mapHolder = new GameObject(containerName).transform;
        containerTransform = mapHolder;
        for(int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
                //SET THE TILE COORDINATES TO THE TILE!!! IMPORTANT.
                TwoDGridTile attachedScript = newTile.GetComponent<TwoDGridTile>() as TwoDGridTile;
                if (attachedScript == null) {
                    Debug.LogError ("GridGenerator: GenerateMap(): couldn't find Tile's attached script, cannot operate... shutting down generation");
                    return;
                }
                attachedScript.setGridPosition (new Vector2Int (x, y));
            }
        }
        //start our coroutine now that the map is ready
        StartCoroutine(updateTiles());
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x * 2 + 0.5f + x, 0, -mapSize.y * 2 + 0.5f + y);
    }
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    IEnumerator updateTiles() {
        //iterate over container Child prefabs... clones of tiles.
        while (instance != null) {
            //Debug.Log ("Updating 2DGridTiles.");
            foreach (Transform t in containerTransform) {
                TwoDGridTile tdgt = t.GetComponent<TwoDGridTile> () as TwoDGridTile;
                if (tdgt == null)
                    continue;
                
                tdgt.updateStatus ();
            }

            yield return waitASecond;

        }
    }
}

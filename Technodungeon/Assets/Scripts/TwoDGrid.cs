using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDGrid : MonoBehaviour {

    public Transform gridtilePrefab;

    public int gridWidth = 10;
    public int gridHeight = 10;
    float cubeWidth = 1.732f;
    float cubeHeight = 2.0f;

    public float gap = 0.0f;

    Vector3 startPos;
    
    void Start()
    {
        AddGap();
        CalcStartPos();
        CreateGrid();
    }

    void AddGap()
    {
        cubeWidth = cubeWidth + gap;
        cubeHeight = cubeHeight + gap;
    }

    void CalcStartPos()
    {
        float offset = 0;
        if(gridHeight / 2 % 2 == 0)
            offset = cubeWidth / 2;

        float x = -cubeWidth * (gridWidth / 2) - offset;
        float z = cubeHeight * (gridHeight / 2);
        startPos = new Vector3(x, 0, z);
    }

    Vector3 CalcWorldPos(Vector2 gridPos)
    {
        float offset = 0;
        float x = startPos.x + gridPos.x * cubeWidth + offset;
        float z = startPos.z - gridPos.y * cubeHeight;

        return new Vector3(x, 0, z);
    }

    void CreateGrid()
    {
        for(int y = 0; y < gridHeight; y++)
        {
            for(int x = 0; x < gridWidth; x++)
            {
                Transform gridtile = Instantiate(gridtilePrefab) as Transform;
                Vector2 gridPos = new Vector2(x, y);
                gridtile.position = CalcWorldPos(gridPos);
                gridtile.parent = this.transform;
                gridtile.name = "gridtile" + x + "|" + y;
            }
        }
    }
}

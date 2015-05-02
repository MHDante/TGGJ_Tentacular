using UnityEngine;
using System.Collections;
using System;
[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    public static RoomManager roomManager;
    public Cell[][] Grid;
    public static float cellSize = 40;
    public static int gridWidth = 20, gridHeight = 20;
    void Awake()
    {
        roomManager = this;
        //if (Application.isPlaying)
        //{
        //    GameObject preexisting = GameObject.Find("Indicator");
        //    if (preexisting != null) DestroyImmediate(preexisting);
        //}

        //if (masterParent == null) masterParent = GameObject.Find("Puzzle_Pieces");
        //if (masterParent == null) masterParent = new GameObject("Puzzle_Pieces");
        Grid = new Cell[gridWidth][];
        for (int i = 0; i < gridWidth; i++)
        {
            Grid[i] = new Cell[gridHeight];
            for (int j = 0; j < gridHeight; j++)
            {
                Grid[i][j] = new Cell(i, j);
            }
        }
        
        try
        {
            FileWrite.DeserializationCallback();
        }
        catch (Exception e)
        {
            Debug.Log("Exception : " + e);
            throw e;
        }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public static Cell GetFromWorldPos(float x, float y)
    {
        int originX = (int)Mathf.Floor(x / cellSize);
        int originY = (int)Mathf.Floor(y / cellSize);
        //Debug.Log(originX + " " + originY);
        return RoomManager.Get(originX, originY);
    }
    public static Cell Get(int x, int y)
    {
        if (x < 0 || x >= RoomManager.roomManager.Grid.Length
            || y < 0 || y >= RoomManager.roomManager.Grid[0].Length)
            return null;
        Cell ret = RoomManager.roomManager.Grid[x][y];
        if (x != ret.x || y != ret.y) throw new SystemException();
        return ret;
    }
}

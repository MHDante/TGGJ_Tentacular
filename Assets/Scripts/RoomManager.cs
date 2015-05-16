using System;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    private const int CAM_SIZE = 8;
    public static RoomManager instance;
    public static float cellSize = 40;
    public static bool hardMode = false;
    private int _gW = 0, _gH = 0;
    private bool _awoken;
    public int differentColors = 3;
    public GameObject entityObject;
    public int gameSteps;
    public Cell[][] grid;
    public GameObject gridObject;
    public int gridWidth = 5, gridHeight = 4;
    public string levelName;
    public int maxEnemies = 6;
    public string nextlevel;
    public Octopus octopus;
    public int octopusX, octopusY = 0;
    public Pause pause;
    public Player player;
    public int playerStartX = 0, playerStartY = 0;
    public int secondsUntilGoat = 2;
    public float transitionPercent;
    private const float UPDATE_RATE = 0.25f;

    public void Awake()
    {
        Application.targetFrameRate = 60;
#if UNITY_EDITOR
        if (!Application.isPlaying && EditorApplication.currentScene != "Assets/Scenes/Workshop.unity") return;
#endif
        Camera.main.orthographicSize = CAM_SIZE;
        RegenMap(true);
        _awoken = true;
        pause = FindObjectOfType<Pause>();
    }

    public bool IsPaused()
    {
        return pause.MenuShowing;
    }

    public void OnValidate()
    {
        if (levelName == "")
            Debug.LogError("levelname is null");

        if (_awoken && (_gW != gridWidth || _gH != gridHeight))
        {
            FileWrite.InitSerialization(gridWidth, gridHeight);
            RegenMap(true);
        }
    }

    private void RegenMap(bool validation)
    {
        instance = this;
        if (gridObject == null) gridObject = GameObject.Find("Grid") ?? new GameObject("Grid");
        if (entityObject == null) entityObject = GameObject.Find("Entities") ?? new GameObject("Entities");
        gridObject.transform.parent = transform;
        entityObject.transform.parent = transform;


        var obs = GameObject.FindGameObjectsWithTag("generated");

        foreach (var o in obs)
        {
            DestroyGeneralized(o.gameObject);
        }

        try
        {
            if (levelName == "blank0")
                FileWrite.DeserializationCallback();
            else if (!Application.isPlaying)
                FileWrite.DeserializationCallback(levelName + ".xml");
            else FileWrite.InitDeserialization(levelName + ".xml");
        }
        catch (FileNotFoundException)
        {
            GenerateEmptyGrid();
        }
    }

    public static void DestroyGeneralized(GameObject o)
    {
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            var o1 = o;
            EditorApplication.delayCall += () => { DestroyImmediate(o1); };
        }
        else
        {
#endif
            Destroy(o);
#if UNITY_EDITOR
        }
#endif
    }

    public void GenerateEmptyGrid()
    {
        grid = new Cell[gridWidth][];
        for (int i = 0; i < gridWidth; i++)
        {
            grid[i] = new Cell[gridHeight];
            for (int j = 0; j < gridHeight; j++)
            {
                grid[i][j] = new Cell(i, j);
            }
        }

        _gW = gridWidth;
        _gH = gridHeight;
    }

    private void Update()
    {
        if (!Application.isPlaying || IsPaused()) return;

        gameSteps = (int) (Time.time/UPDATE_RATE);
        transitionPercent = (Time.time%UPDATE_RATE)/UPDATE_RATE;

        if (player != null)
        {
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }


        foreach (var row in grid)
            foreach (var cell in row)
                if (cell != null)
                    cell.Update();
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(((float) gridWidth)/2, ((float) gridHeight)/2), new Vector3(gridWidth, gridHeight));
    }

    public static Cell Get(int x, int y)
    {
        if (!(x >= 0 && x < instance.grid.Length
                && y >= 0 && y < instance.grid[0].Length))
            return null;
        Cell ret = instance.grid[x][y];
        if (x != ret.x || y != ret.y) throw new SystemException();
        return ret;
    }
}
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    // Update is called once per frame


    private const int CAM_SIZE = 8;
    public static RoomManager roomManager;
    public static float cellSize = 40;
    public static bool hardMode = false;
    private int _gW = 0, _gH = 0;
    private bool awoken = false;
    public int differentColors = 3;
    public Cell[][] Grid;
    public int gridWidth = 5, gridHeight = 4;
    public string levelName;
    public int maxEnemies = 6;
    public string nextlevel;
    public Octopus octopus;
    public int OctopusX = 0, OctopusY = 0;
    public Player player;
    public int PlayerStartX = 0, PlayerStartY = 0;
    public int secondsUntilGoat = 2;
    public GameObject gridObject;
    public Pause pause;
    public GameObject entityObject;

    public void Awake()
    {

#if UNITY_EDITOR
        if (!Application.isPlaying && EditorApplication.currentScene != "Assets/Scenes/Workshop.unity") return;
#endif
        Camera.main.orthographicSize = CAM_SIZE;
        RegenMap(true);
        awoken = true;
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

        if (awoken && (_gW != gridWidth || _gH != gridHeight))
        {
            FileWrite.InitSerialization(gridWidth, gridHeight);
            RegenMap(true);
        }
    }

    private void RegenMap(bool validation)
    {
        roomManager = this;
        if (gridObject == null) gridObject = GameObject.Find("Grid")?? new GameObject("Grid");
        if (entityObject == null) entityObject = GameObject.Find("Entities")?? new GameObject("Entities");
        gridObject.transform.parent = transform;
        entityObject.transform.parent = transform;


        var obs = GameObject.FindGameObjectsWithTag("generated");
        foreach (var o in obs)
        {
            DestroyGeneralized(o.gameObject);
        }

        //if (Application.isPlaying)
        //{
        try
        {
            string n = levelName;
            if (n == "blank0")
            {
                FileWrite.DeserializationCallback();
            }
            else if (!Application.isPlaying)
            {
                FileWrite.DeserializationCallback(n + ".xml");
            }

            else FileWrite.InitDeserialization(n + ".xml");
        }
        catch (FileNotFoundException)
        {
            GenerateEmptyGrid();
        }
        //}
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
        Grid = new Cell[gridWidth][];
        for (int i = 0; i < gridWidth; i++)
        {
            Grid[i] = new Cell[gridHeight];
            for (int j = 0; j < gridHeight; j++)
            {
                Grid[i][j] = new Cell(i, j);
            }
        }

        _gW = gridWidth;
        _gH = gridHeight;

        //mainCamera.orthographicSize = gridHeight/2 + (gridHeight%2)*2;
        //transform.position = new Vector3(((float) gridWidth)/2, ((float) gridHeight)/2);
    }

    // Use this for initialization
    private void Start()
    {
        //hintText.text = Hints.GetHint();
    }

    private void Update()
    {
        if (!Application.isPlaying||IsPaused())return;

        if (player != null)
        {
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }


        for(int i = 0; i < Grid.Length; i++)
        {
            for (int j = 0; j < Grid[0].Length; j++)
            {
                Cell c = Grid[i][j];
                if (c != null)
                {
                    c.Update();
                }
            }
        }
    }

    public void SkillButton(int num)
    {
    }

    public static Cell GetFromWorldPos(float x, float y)
    {
        int originX = (int) Mathf.Floor(x);
        int originY = (int) Mathf.Floor(y);
        return Get(originX, originY);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(((float) gridWidth)/2, ((float) gridHeight)/2), new Vector3(gridWidth, gridHeight));
    }

    public static Cell Get(int x, int y)
    {
        if (!IsWithinGrid(x, y))
            return null;
        Cell ret = roomManager.Grid[x][y];
        if (x != ret.x || y != ret.y) throw new SystemException();
        return ret;
    }

    public static bool IsWithinGrid(int x, int y)
    {
        return (x >= 0 && x < roomManager.Grid.Length
                && y >= 0 && y < roomManager.Grid[0].Length);
    }
}
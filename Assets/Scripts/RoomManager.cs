using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    // Update is called once per frame
    public enum FadeStates
    {
        FadeIn,
        Show,
        FadeOut,
        Wait
    }

    private const int CAM_SIZE = 8;
    public static RoomManager roomManager;
    public static float cellSize = 40;
    public static bool hardMode = false;
    private int _gW = 0, _gH = 0;
    private bool awoken = false;
    public int differentColors = 3;
    private FadeStates fadeState = FadeStates.FadeOut;
    public Cell[][] Grid;
    public int gridWidth = 5, gridHeight = 4;
    private float hintAlpha = 0;
    private Text hintText;
    public string levelName;
    public int maxEnemies = 6;
    private float MaxFadeTimer = 2;
    private int MaxHintTimer = 4;
    public string nextlevel;
    public Octopus octopus;
    public int OctopusX = 0, OctopusY = 0;
    public Player player;
    public int PlayerStartX = 0, PlayerStartY = 0;
    public int secondsUntilGoat = 2;
    private float tempHintTimer = 0;
    private GameObject gridObject;

    public void Awake()
    {
        Camera.main.orthographicSize = CAM_SIZE;
        RegenMap(true);
        awoken = true;
        tempHintTimer = (float) MaxHintTimer;

        var obj = GameObject.Find("HintText");
        hintText = obj == null ? null : obj.GetComponent<Text>();
    }

    public bool IsPaused()
    {
        return gameObject.GetComponent<Pause>().MenuShowing;
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
        transform.position = new Vector3(((float) gridWidth)/2, ((float) gridHeight)/2);
    }

    // Use this for initialization
    private void Start()
    {
        //hintText.text = Hints.GetHint();
    }

    private void Update()
    {
        if (IsPaused() || !Application.isPlaying) return;

        tempHintTimer -= Time.deltaTime;
        if (tempHintTimer <= 0)
        {
            if (fadeState == FadeStates.FadeIn)
            {
                fadeState = FadeStates.Show;
                hintText.color = Color.black;
                tempHintTimer = MaxHintTimer;
            }
            else if (fadeState == FadeStates.Show)
            {
                fadeState = FadeStates.FadeOut;
                tempHintTimer = MaxFadeTimer;
            }
            else if (fadeState == FadeStates.FadeOut)
            {
                fadeState = FadeStates.Wait;
                hintText.color = new Color(0, 0, 0, 0);
                tempHintTimer = MaxHintTimer;
                //Hints.GetHint();
                hintText.text = Hints.GetHint();
            }
            else if (fadeState == FadeStates.Wait)
            {
                fadeState = FadeStates.FadeIn;
                tempHintTimer = MaxFadeTimer;
            }
        }

        float percent = (float) tempHintTimer/(float) MaxFadeTimer;
        if (fadeState == FadeStates.FadeOut)
        {
            hintText.color = new Color(0, 0, 0, percent);
        }
        else if (fadeState == FadeStates.FadeIn)
        {
            hintText.color = new Color(0, 0, 0, 1 - percent);
        }


        if (player != null)
        {
            Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
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
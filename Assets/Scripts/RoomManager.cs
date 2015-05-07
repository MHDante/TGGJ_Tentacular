using UnityEngine;
using System.Collections;
using System;
using System.IO;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RoomManager : MonoBehaviour
{
    public static RoomManager roomManager;
    public Cell[][] Grid;
    public static float cellSize = 40;
    public int gridWidth = 5, gridHeight = 4;
    private int _gW = 0, _gH = 0;
    private bool awoken = false;
    public int PlayerStartX = 0, PlayerStartY = 0;
    public Player player;
    public int OctopusX = 0, OctopusY = 0;
    public Octopus octopus;
    public Camera mainCamera;
    private const int CAM_SIZE = 8;
    public int maxEnemies = 6;
    public int differentColors = 3;
    public static bool hardMode = false;
    public int secondsUntilGoat = 2;
    private Text hintText;
    private int MaxHintTimer = 4;
    private float tempHintTimer = 0;
    private float MaxFadeTimer = 2;

    public void Awake()
    {
        mainCamera = GetComponentInChildren<Camera>();
        mainCamera.orthographicSize = CAM_SIZE;
        RegenMap(true);
        awoken = true;
        tempHintTimer = (float) MaxHintTimer;

        var obj = GameObject.Find("HintText");
        hintText = obj == null? null : obj.GetComponent<Text>();


    }
	public bool IsPaused(){
		return gameObject.GetComponent<Pause> ().MenuShowing;
	}
    public string levelName;
    public string nextlevel;

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
        //if (Application.isPlaying)
        //{
        //    GameObject preexisting = GameObject.Find("Indicator");
        //    if (preexisting != null) DestroyImmediate(preexisting);
        //}

        //if (masterParent == null) masterParent = GameObject.Find("Puzzle_Pieces");
        //if (masterParent == null) masterParent = new GameObject("Puzzle_Pieces");
        var obs = GameObject.FindGameObjectsWithTag("cell");
        foreach (var o in obs)
        {
            DestroyGeneralized(o.gameObject);
        }
        var plyr = FindObjectOfType<Player>();
        if (plyr != null)
        {
            DestroyGeneralized(plyr.gameObject);
        }
        var oct = FindObjectOfType<Octopus>();
        if (oct != null)
        {
            DestroyGeneralized(oct.gameObject);
        }

        //if (Application.isPlaying)
        //{
        try
            {
                string n = levelName;
                if (n == "blank0")
                {
                    FileWrite.DeserializationCallback();
                }else if (!Application.isPlaying)
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
            UnityEditor.EditorApplication.delayCall += () =>
            {
                DestroyImmediate(o1);
            };
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
        void Start()
    {
        //hintText.text = Hints.GetHint();
    }

    private float hintAlpha = 0;
    // Update is called once per frame
    public enum FadeStates
    {
        FadeIn,
        Show,
        FadeOut,
        Wait

    }
    private  FadeStates fadeState = FadeStates.FadeOut;
    void Update()
    {

		if (IsPaused () || !Application.isPlaying) return;

        tempHintTimer -= Time.deltaTime;
        if (tempHintTimer <= 0)
        {
            if (fadeState == FadeStates.FadeIn)
            {
                fadeState = FadeStates.Show;
                hintText.color = Color.black;
                tempHintTimer = MaxHintTimer;
            }
            else if(fadeState == FadeStates.Show)
            {
                fadeState = FadeStates.FadeOut;
                tempHintTimer = MaxFadeTimer;
            } else if (fadeState == FadeStates.FadeOut) {
                fadeState = FadeStates.Wait;
                hintText.color = new Color(0, 0, 0, 0);
                tempHintTimer = MaxHintTimer;
                //Hints.GetHint();
                hintText.text = Hints.GetHint();
            } else if (fadeState == FadeStates.Wait) {
                fadeState = FadeStates.FadeIn;
                tempHintTimer = MaxFadeTimer;
            }
            
        }

        float percent = (float)tempHintTimer / (float)MaxFadeTimer;
        if (fadeState == FadeStates.FadeOut)
        {
            hintText.color = new Color(0,0,0,percent);
        }
        else if (fadeState == FadeStates.FadeIn) {
            hintText.color = new Color(0, 0, 0, 1-percent);
        }


        if (player != null)
        {
            mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }
    }

    public void SkillButton(int num)
    {
        
    }

    public static Cell GetFromWorldPos(float x, float y)
    {
        int originX = (int)Mathf.Floor(x);
        int originY = (int)Mathf.Floor(y);
        //Debug.Log(originX + " " + originY);
        return RoomManager.Get(originX, originY);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(((float)gridWidth)/2, ((float)gridHeight)/2), new Vector3(gridWidth, gridHeight));
    }
    public static Cell Get(int x, int y)
    {
        if (!IsWithinGrid(x, y))
            return null;
        Cell ret = RoomManager.roomManager.Grid[x][y];
        if (x != ret.x || y != ret.y) throw new SystemException();
        return ret;
    }
    public static bool IsWithinGrid(int x, int y)
    {
        return (x >= 0 && x < RoomManager.roomManager.Grid.Length
             && y >= 0 && y < RoomManager.roomManager.Grid[0].Length);
    }
}

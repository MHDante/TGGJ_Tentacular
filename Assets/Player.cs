using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    public float playerSpeed;
    public Cell currentCell;

    private GameObject spriteChild;
    private Quaternion rotGoal;
    public static Dictionary<Dirs, Vector2> dirToVect = new Dictionary<Dirs, Vector2>()
    {
        { Dirs.N, Vector2.up },
        { Dirs.S, -Vector2.up },
        { Dirs.E, Vector2.right },
        { Dirs.W, -Vector2.right },
    };
    public static Dictionary<Vector2, Dirs> vectToDir = new Dictionary<Vector2, Dirs>();
    // Use this for initialization
    void Start() {
        currentCell = RoomManager.roomManager.Grid[(int)transform.position.x][(int)transform.position.y];
        playerSpeed = 0.04f;
    }
    //bool StandingStill = true;
    bool IsMoving = false;
    Vector2 dest = Vector2.zero;
    Cell nextCell;
    Dirs prevDir;
	// Update is called once per frame
    void Awake()
    {
        if (dictPossibleDirs.Count == 0)
        {
            foreach (Dirs d in Enum.GetValues(typeof(Dirs)))
            {
                dictPossibleDirs[d] = GetPossibleDirs(d);
            }
        }
        if (vectToDir.Count == 0)
        {
            foreach (Dirs d in dirToVect.Keys)
            {
                vectToDir.Add(dirToVect[d], d);
            }
        }

        spriteChild = transform.FindChild("spriteChild").gameObject;
        skillButton1 = GameObject.Find("skillButton1");
        skillButton2 = GameObject.Find("skillButton2");
        skillButton3 = GameObject.Find("skillButton3");
        skillButton4 = GameObject.Find("skillButton4");
    }

    private GameObject skillButton1, skillButton2, skillButton3, skillButton4;

    void alterButton(GameObject button, bool pressed)
    {
        var colors = button.GetComponentInChildren<Button>().colors;
        button.GetComponent<Image>().color = new Color(button.GetComponent<Image>().color.r, button.GetComponent<Image>().color.g, button.GetComponent<Image>().color.b,pressed?1 : .25f);
        button.GetComponentInChildren<Text>().color = pressed?Color.white :Color.black;
    }
    void Update () {
		if (RoomManager.roomManager.IsPaused ())
			return;
        Colors? currentCol = null;
        if      (Input.GetButton("Col1")) currentCol = Colors.Red;
        else if (Input.GetButton("Col2")) currentCol = Colors.Green;
        else if (Input.GetButton("Col3")) currentCol = Colors.Blue;
        else if (Input.GetButton("Col4")) currentCol = Colors.Black;


        alterButton(skillButton1, currentCol == Colors.Red);
        alterButton(skillButton2, currentCol == Colors.Green);
        alterButton(skillButton3, currentCol == Colors.Blue);
        alterButton(skillButton4, currentCol == Colors.Black);

        if (currentCol != null)
        {
            //if (nextCell != null)
            //{
            //    nextCell.SetColor(currentCol.Value);
            //}
            //else
            //{
                currentCell.SetColor(currentCol.Value);
            //}
        }
        Color pCol = currentCol == null ? Color.white : Cell.colorVals[currentCol.Value];
        var sp = GetComponentInChildren<SpriteRenderer>();
        sp.color = pCol;

        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, Mathf.Min(playerSpeed, Vector2.Distance(transform.position, dest)));
            //if (Vector2.Distance(transform.position, dest) < playerSpeed * Time.deltaTime)
            if (transform.position.x == dest.x && transform.position.y == dest.y)
            {
                currentCell = nextCell;
                nextCell = null;
                IsMoving = false;
                //if (RoomManager.roomManager.octopus.IsWithinOctopus(currentCell.x, currentCell.y))
                //{
                //    Debug.Log("WIN");
                //    if (string.IsNullOrEmpty(RoomManager.roomManager.nextlevel))
                //    {
                //        Application.LoadLevel("TitleScreen");
                //    }
                //    else
                //    {
                //        FileWrite.InitDeserialization(RoomManager.roomManager.nextlevel);
                //    }
                //    return;
                //}
            }
        }
        else
        {
            if (currentCell.enemy != null && RoomManager.roomManager.hardMode)
            {
                var pause = GameObject.FindObjectOfType<Pause>();
                pause.MenuToggle("GameOver");
            }
            float horiz = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            if (horiz != 0) vert = 0;
            if (horiz != 0 || vert != 0)
            {
                int x = (int)horiz + currentCell.x;
                int y = (int)vert + currentCell.y;
                if (RoomManager.IsWithinGrid(x, y))
                {
                    Cell possibleNext = RoomManager.Get(x, y);
                    bool isOctTile = RoomManager.roomManager.octopus.IsWithinOctopus(x, y);
                    if (isOctTile || (int)possibleNext.type != 0)
                    {
                        Dirs dir = vectToDir[new Vector2(horiz, vert)];
                        if (isOctTile || Cell.IsValidMove(dir, currentCell.type, possibleNext.type))
                        {
                            IsMoving = true;
                            dest = new Vector2(x, y);

                            float angle = Mathf.Atan2(-dirToVect[dir].x, dirToVect[dir].y) * Mathf.Rad2Deg;
                            spriteChild.transform.rotation = new Quaternion { eulerAngles = new Vector3(0, 0, angle) };

                            nextCell = possibleNext;
                            prevDir = dir;

                            Update();
                            return;
                        }
                    }
                }
            }
        }
	}
    
    public static Dictionary<Dirs, List<Dirs>> dictPossibleDirs = new Dictionary<Dirs, List<Dirs>>();
    public static List<Dirs> GetPossibleDirs(Dirs dir)
    {
        List<Dirs> dirs = new List<Dirs>();
        Dirs opp = Cell.GetOppositeDir(dir);
        dirs.Add(dir);
        foreach (Dirs d in Enum.GetValues(typeof(Dirs)))
        {
            if (d == opp || d == dir) continue;
            dirs.Add(d);
        }
        return dirs;
    }
    public void SetCell(int x, int y)
    {
        Cell next = RoomManager.Get(x, y);
        if (next != null)
        {
            currentCell = next;
            transform.position = new Vector3(currentCell.x, currentCell.y);
        }
    }
}

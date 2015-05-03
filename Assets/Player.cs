using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour {
    public float playerSpeed;
    public Cell currentCell;

    private GameObject spriteChild;
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
        playerSpeed = 0.1f;
    }
    bool StandingStill = true;
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
    }
    Vector2 lastPressDir = Vector2.zero;
	void Update () {
        if (StandingStill)
        {
            float horiz = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            if (horiz != 0) vert = 0;
            else if (vert == 0) return;
            int x = (int)horiz + currentCell.x;
            int y = (int)vert + currentCell.y;
            if (!RoomManager.IsWithinGrid(x, y)) return;
            Cell possibleNext = RoomManager.Get(x, y);
            bool isOctTile = RoomManager.roomManager.octopus.IsWithinOctopus(x, y);
            if (!isOctTile && (int)possibleNext.type == 0) return;
            Dirs dir = vectToDir[new Vector2(horiz, vert)];
            if (isOctTile || Cell.IsValidMove(dir, currentCell.type, possibleNext.type))
            {
                IsMoving = true;
                dest = new Vector2(x, y);

                float angle = Mathf.Atan2(-dirToVect[dir].x, dirToVect[dir].y) * Mathf.Rad2Deg;
                spriteChild.transform.rotation = new Quaternion { eulerAngles = new Vector3(0, 0, angle) };

                nextCell = possibleNext;
                StandingStill = false;
                prevDir = dir;
            }
        }
        else
        {
            if (IsMoving)
            {
                float horiz = Input.GetAxisRaw("Horizontal");
                float vert = Input.GetAxisRaw("Vertical");
                if (horiz != 0) vert = 0;
                if (horiz != 0 || vert != 0)
                {
                    lastPressDir = new Vector2(horiz, vert);
                }

                transform.position = Vector3.MoveTowards(transform.position, dest, playerSpeed);
                //if (Vector2.Distance(transform.position, dest) < playerSpeed * Time.deltaTime)
                if (transform.position.x == dest.x && transform.position.y == dest.y)
                    {
                    currentCell = nextCell;
                    IsMoving = false;
                    if (RoomManager.roomManager.octopus.IsWithinOctopus(currentCell.x, currentCell.y))
                    {
                        Debug.Log("WIN");
                        if (string.IsNullOrEmpty(RoomManager.roomManager.nextlevel))
                        {
                            Application.LoadLevel("TitleScreen");
                        }
                        else
                        {
                            FileWrite.InitDeserialization(RoomManager.roomManager.nextlevel);
                        }
                        return;
                    }
                    //Update();
                }
                //Debug.Log("moving");
            }
            else
            {
                float horiz = Input.GetAxisRaw("Horizontal");
                float vert = Input.GetAxisRaw("Vertical");
                if (horiz != 0) vert = 0;
                if (horiz == 0 && vert == 0)
                {
                    horiz = lastPressDir.x;
                    vert = lastPressDir.y;
                }
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
                                StandingStill = false;
                                prevDir = dir;
                                lastPressDir = Vector2.zero;

                                Update();
                                return;
                            }
                        }
                    }
                }
                foreach (Dirs d in dictPossibleDirs[prevDir])
                {
                    Dirs opp = Cell.GetOppositeDir(d);
                    Vector2 next = dirToVect[d] + (Vector2)transform.position;
                    Cell c = RoomManager.Get((int)next.x, (int)next.y);
                    if (c != null 
                        && (RoomManager.roomManager.octopus.IsWithinOctopus(c.x,c.y) 
                               || (c.type != Types.Blank
                          && (Cell.typeDirs[c.type].Contains(opp) || Cell.typeDirs[currentCell.type].Contains(d)))))
                    {
                        IsMoving = true;
                        dest = next;
                        
                        float angle = Mathf.Atan2(-dirToVect[d].x, dirToVect[d].y) * Mathf.Rad2Deg;
                        spriteChild.transform.rotation = new Quaternion { eulerAngles = new Vector3(0, 0, angle) };

                        nextCell = c;
                        prevDir = d;
                        lastPressDir = Vector2.zero;

                        Update();
                        return;
                    }
                }
                StandingStill = true;
                Debug.Log("Or something");
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Player : MonoBehaviour {
    public float playerSpeed;
    public Cell currentCell;

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
        playerSpeed = 0.05f;
    }
    bool StandingStill = true;
    bool IsMoving = false;
    Vector2 dest = Vector2.zero;
    Cell nextCell;
    Dirs prevDir;
	// Update is called once per frame
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
                dest = new Vector3(x, y);
                nextCell = possibleNext;
                StandingStill = false;
                prevDir = dir;
            }
        }
        else
        {
            if (IsMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, dest, playerSpeed);
                if (transform.position.x == dest.x && transform.position.y == dest.y)
                {
                    currentCell = nextCell;
                    IsMoving = false;
                    if (RoomManager.roomManager.octopus.IsWithinOctopus(currentCell.x, currentCell.y))
                    {
                        Debug.Log("WIN");
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
                                dest = new Vector3(x, y);
                                nextCell = possibleNext;
                                StandingStill = false;
                                prevDir = dir;
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
                    if (c != null && (Cell.typeDirs[c.type].Contains(opp) || Cell.typeDirs[currentCell.type].Contains(d)))
                    {
                        IsMoving = true;
                        dest = next;
                        nextCell = c;
                        prevDir = d;
                        Update();
                        return;
                    }
                }
                StandingStill = true;
            }
        }
        
	}
    public static Dictionary<Dirs, List<Dirs>> dictPossibleDirs = new Dictionary<Dirs, List<Dirs>>();
    static Player(){
        foreach (Dirs d in Enum.GetValues(typeof(Dirs)))
        {
            dictPossibleDirs[d] = GetPossibleDirs(d);
        }
        foreach(Dirs d in dirToVect.Keys)
        {
            vectToDir.Add(dirToVect[d], d);
        }
    }
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

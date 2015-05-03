using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    public float enemySpeed;
    public Cell currentCell;
    // Use this for initialization
    void Start () {
        currentCell = RoomManager.roomManager.Grid[(int)transform.position.x][(int)transform.position.y];
        enemySpeed = 0.05f;
    }
    bool IsMoving = false;
    Vector2 dest = Vector2.zero;
    Cell nextCell;
    public Dirs prevDir;
    // Update is called once per frame
    void Update () {
        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, enemySpeed);
            if (transform.position.x == dest.x && transform.position.y == dest.y)
            {
                currentCell = nextCell;
                IsMoving = false;
            }
        }
        else
        {
            bool enemyFound = false;
            List<Dirs> possibleDirs = new List<Dirs>();
            foreach (Dirs d in Player.dictPossibleDirs[prevDir])
            {
                Dirs opp = Cell.GetOppositeDir(d);
                Vector2 next = Player.dirToVect[d] + (Vector2)transform.position;
                Cell c = RoomManager.Get((int)next.x, (int)next.y);
                //check if player is adjacent, and kill no matter what
                if (c != null)
                {
                    if (RoomManager.roomManager.octopus.IsWithinOctopus(c.x, c.y))
                    {
                        enemyFound = true;
                    }
                    else if (c.type != Types.Blank
                        && (Cell.typeDirs[c.type].Contains(opp) || Cell.typeDirs[currentCell.type].Contains(d)))
                    {
                        if (c.enemy != null)
                        {
                            enemyFound = true;
                        }
                        else
                        {
                            possibleDirs.Add(d);
                        }
                    }
                }
            }
            if (possibleDirs.Count > 0)
            {
                Dirs d = possibleDirs.ToArray()[UnityEngine.Random.Range(0, possibleDirs.Count)];
                MoveInDir(d);
            }
            else if (enemyFound)
            {
                MoveInDir(Cell.GetOppositeDir(prevDir));
            }
        }
    }
    void MoveInDir(Dirs d)
    {
        Vector2 next = Player.dirToVect[d] + (Vector2)transform.position;
        Cell c = RoomManager.Get((int)next.x, (int)next.y);
        IsMoving = true;
        dest = next;
        nextCell = c;
        currentCell.enemy = null;
        nextCell.enemy = this;
        prevDir = d;
        Update();
    }
    public void SetCell(int x, int y)
    {
        Cell next = RoomManager.Get(x, y);
        if (next != null)
        {
            currentCell = next;
            transform.position = new Vector3(currentCell.x, currentCell.y);
            currentCell.enemy = this;
        }
    }
}

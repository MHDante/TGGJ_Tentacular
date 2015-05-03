using UnityEngine;
using System.Collections;
using System;

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
    Dirs prevDir;
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
            foreach (Dirs d in Enum.GetValues(typeof(Dirs)))
            {
                if (d == prevDir) continue;

            }


            //int x = (int)horiz + currentCell.x;
            //int y = (int)vert + currentCell.y;
            //if (!RoomManager.IsWithinGrid(x, y)) return;
            //Cell possibleNext = RoomManager.Get(x, y);
            //bool isOctTile = RoomManager.roomManager.octopus.IsWithinOctopus(x, y);
            //if (!isOctTile && (int)possibleNext.type == 0) return;
            //Dirs dir = Dirs.N;
            //if (horiz == 1) dir = Dirs.E;
            //else if (horiz == -1) dir = Dirs.W;
            //else if (vert == 1) dir = Dirs.N;
            //else if (vert == -1) dir = Dirs.S;
            //
            //if (isOctTile || Cell.IsValidMove(dir, currentCell.type, possibleNext.type))
            //{
            //    IsMoving = true;
            //    dest = new Vector3(x, y);
            //    nextCell = possibleNext;
            //}
        }
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

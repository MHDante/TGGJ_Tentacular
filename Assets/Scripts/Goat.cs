using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Goat : MonoBehaviour {
    public float goatSpeed;
    public Cell currentCell;
    // Use this for initialization
    void Start () {
        currentCell = RoomManager.roomManager.Grid[(int)transform.position.x][(int)transform.position.y];
        goatSpeed = 0.1f;
    }
    bool IsMoving = false;
    Vector2 dest = Vector2.zero;
    Cell nextCell;
    public Dirs prevDir;

    float hue = 0, huespeed = 1f;
    // Update is called once per frame
    void Update () {
        if (RoomManager.roomManager.IsPaused())
            return;

        hue = (hue + huespeed) % 360;
        Color c = Octopus.HSVToRGB(hue / 360f, 1f, 1f);
        var sp = GetComponentInChildren<SpriteRenderer>();
        sp.color = c;

        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, goatSpeed);
            if (transform.position.x == dest.x && transform.position.y == dest.y)
            {
                currentCell = nextCell;
                IsMoving = false;
            }
        }
        else
        {
            bool turnAround = false;
            List<Dirs> firstDirChoices = new List<Dirs>();
            foreach (Dirs d in Player.dictPossibleDirs[prevDir])
            {
                Dirs opp = Cell.GetOppositeDir(d);
                Vector2 next = Player.dirToVect[d] + (Vector2)transform.position;
                Cell nextCell = RoomManager.Get((int)next.x, (int)next.y);
                if (nextCell != null)
                {
                    if (RoomManager.roomManager.octopus.IsWithinOctopus(nextCell.x, nextCell.y))
                    {
                        turnAround = true;
                    }
                    else if (nextCell.type == Types.Blank)
                    {
                        turnAround = true; // ??
                    }
                    else if (nextCell.type != Types.Blank
                        && (Cell.typeDirs[nextCell.type].Contains(opp) || Cell.typeDirs[currentCell.type].Contains(d)))
                    {
                        if (RoomManager.roomManager.player.currentCell == nextCell)
                        {
                            //die

                            var pause = GameObject.FindObjectOfType<Pause>();
                            pause.MenuToggle("GameOver");
                        }
                        firstDirChoices.Add(d);
                    }
                }
            }
            if (firstDirChoices.Count > 0)
            {
                Dirs d = firstDirChoices[Random.Range(0, firstDirChoices.Count)];
                MoveInDir(d);
            }
            else if (turnAround)
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
        }
    }
}

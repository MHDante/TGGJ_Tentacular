using System.Collections.Generic;
using UnityEngine;

public class Goat : MonoBehaviour
{
    public Cell currentCell;
    private Vector2 dest = Vector2.zero;
    public float goatSpeed;
    private float hue = 0, huespeed = 1f;
    private bool IsMoving = false;
    private Cell nextCell;
    public Dirs prevDir;
    // Use this for initialization
    private void Start()
    {
        currentCell = RoomManager.instance.grid[(int) transform.position.x][(int) transform.position.y];
        goatSpeed = 0.1f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (RoomManager.instance.IsPaused())
            return;

        hue = (hue + huespeed)%360;
        Color c = Octopus.HSVToRGB(hue/360f, 1f, 1f);
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
                Vector2 next = Player.dirToVect[d] + (Vector2) transform.position;
                Cell nextCell = RoomManager.Get((int) next.x, (int) next.y);
                if (nextCell != null)
                {
                    if (RoomManager.instance.octopus.IsWithinOctopus(nextCell.x, nextCell.y))
                    {
                        turnAround = true;
                    }
                    else if (nextCell.CellType == Types.Blank)
                    {
                        turnAround = true; // ??
                    }
                    else if (nextCell.CellType != Types.Blank
                             &&
                             (Cell.typeDirs[nextCell.CellType].Contains(opp) || Cell.typeDirs[currentCell.CellType].Contains(d)))
                    {
                        if (RoomManager.instance.player.currentCell == nextCell)
                        {
                            //die

                            var pause = FindObjectOfType<Pause>();
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

    private void MoveInDir(Dirs d)
    {
        Vector2 next = Player.dirToVect[d] + (Vector2) transform.position;
        Cell c = RoomManager.Get((int) next.x, (int) next.y);
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
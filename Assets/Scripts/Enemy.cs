using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    public float enemySpeed;
    public Cell currentCell;
    public Colors col;
    private GameObject spriteChild;

    void Awake()
    {
        spriteChild = transform.FindChild("spriteChild").gameObject;

    }
    // Use this for initialization
    void Start () {
        currentCell = RoomManager.roomManager.Grid[(int)transform.position.x][(int)transform.position.y];
        enemySpeed = 0.04f;

    }
    public void SetColor(Colors color)
    {
        if (col == color) return;
        col = color;
        var sp = spriteChild.GetComponent<SpriteRenderer>();
        sp.color = Cell.colorVals[col];

    }
    public bool IsWinCondition()
    {
        var enemies = FindObjectsOfType<Enemy>();
        bool win = true;
        if (enemies.Length != RoomManager.roomManager.maxEnemies) win = false;
        Dictionary<Colors, int> dict = new Dictionary<Colors, int>()
        {
            { Colors.Red, 0 },
            { Colors.Green, 0 },
            { Colors.Blue, 0 },
        };
        foreach (var e in enemies)
        {
            if (e.col != e.currentCell.col)
            {
                dict[e.col]++;
                win = false;
            }
        }
        foreach(var c in dict.Keys)
        {
            string name = c.ToString().ToLower() + "FishNumber";
            var go = GameObject.Find(name);
            var txt = go.GetComponent<Text>();
            txt.text = dict[c].ToString();
        }
        return win;
    }
    public static bool WinningState = false;
    bool IsMoving = false;
    Vector2 dest = Vector2.zero;
    Cell nextCell;
    public Dirs prevDir;
    // Update is called once per frame
    void Update () {
		if (RoomManager.roomManager.IsPaused ())
			return;
        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, enemySpeed);
            if (transform.position.x == dest.x && transform.position.y == dest.y)
            {
                currentCell = nextCell;
                IsMoving = false;
                if (currentCell.col == col)
                {
                    currentCell.Decay();
                }
                WinningState = IsWinCondition();
            }
            RoomManager.roomManager.octopus.ChangeState();
        }
        else
        {
            bool turnAround = false;
            List<Dirs> firstDirChoices = new List<Dirs>();
            List<Dirs> secondDirChoices = new List<Dirs>();
            Dirs? colorDir = null;
            int highestDecay = 0;
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
                        //if (RoomManager.roomManager.player.currentCell == nextCell)
                        //{
                        //    Debug.Log("You are Dead.");
                        //}
                        if (col == nextCell.col && colorDir == null && nextCell.decayLeft > highestDecay)
                        {
                            colorDir = d;
                            highestDecay = nextCell.decayLeft;
                        }
                        else if (nextCell.col == Colors.Black)
                        {
                            firstDirChoices.Add(d);
                        }
                        else
                        {
                            secondDirChoices.Add(d);
                        }
                    }
                }
            }
            if (colorDir != null)
            {
                MoveInDir(colorDir.Value);
                return;
            }
            if (firstDirChoices.Count > 0)
            {
                Dirs d = firstDirChoices[0];
                MoveInDir(d);
            }
            else if (secondDirChoices.Count > 0)
            {
                Dirs d = secondDirChoices[0];
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
        float angle = Mathf.Atan2(-Player.dirToVect[d].x, Player.dirToVect[d].y) * Mathf.Rad2Deg;
        spriteChild.transform.rotation = new Quaternion { eulerAngles = new Vector3(0, 0, angle) };

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

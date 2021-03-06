﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Dictionary<Dirs, Vector2> dirToVect = new Dictionary<Dirs, Vector2>()
    {
        {Dirs.N, Vector2.up},
        {Dirs.S, -Vector2.up},
        {Dirs.E, Vector2.right},
        {Dirs.W, -Vector2.right},
    };

    public static Dictionary<Vector2, Dirs> vectToDir = new Dictionary<Vector2, Dirs>();
    public static Dictionary<Dirs, List<Dirs>> dictPossibleDirs = new Dictionary<Dirs, List<Dirs>>();
    private GameObject background;
    public Cell currentCell;
    private Vector2 dest = Vector2.zero;
    public LayerMask enemies;
    //bool StandingStill = true;
    private bool IsMoving = false;
    private Cell nextCell;
    public float playerSpeed;
    private Quaternion rotGoal;
    private GameObject skillButton1, skillButton2, skillButton3, skillButton4;
    private GameObject spriteChild;
    // Use this for initialization
    private void Start()
    {
        currentCell = RoomManager.instance.grid[(int) transform.position.x][(int) transform.position.y];
        playerSpeed = 0.08f;
    }

    // Update is called once per frame
    private void Awake()
    {
        if (dictPossibleDirs.Count == 0)
        {
            foreach (Dirs d in Enum.GetValues(typeof (Dirs)))
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
        background = GameObject.Find("Background");
        spriteChild = transform.FindChild("spriteChild").gameObject;
        skillButton1 = GameObject.Find("skillButton1");
        skillButton2 = GameObject.Find("skillButton2");
        skillButton3 = GameObject.Find("skillButton3");
        skillButton4 = GameObject.Find("skillButton4");
    }

    private void alterButton(GameObject button, bool pressed)
    {
        button.GetComponent<Image>().color = new Color(button.GetComponent<Image>().color.r,
            button.GetComponent<Image>().color.g, button.GetComponent<Image>().color.b, pressed ? 1 : .40f);
        button.GetComponentInChildren<Text>().color = pressed ? Color.white : Color.black;
    }

    private void Update()
    {
        if (RoomManager.instance.IsPaused())
            return;
        if (background != null)
        {
            background.transform.position = transform.position*0.3f;
        }
        float scrollwheel = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scrollwheel != 0)
        {
            Camera.main.orthographicSize -= scrollwheel;
        }

        CheckCollision();
        Colors? currentCol = null;
        if (Input.GetButton("Col1")) currentCol = Colors.Red;
        else if (Input.GetButton("Col2")) currentCol = Colors.Green;
        else if (Input.GetButton("Col3")) currentCol = Colors.Blue;
        else if (Input.GetButton("Col4")) currentCol = Colors.Black;


        alterButton(skillButton1, currentCol == Colors.Red);
        alterButton(skillButton2, currentCol == Colors.Green);
        alterButton(skillButton3, currentCol == Colors.Blue);
        alterButton(skillButton4, currentCol == Colors.Black);

        if (currentCol != null)
        {
            if (nextCell != null && Vector2.Distance(transform.position, dest) < 0.5f)
            {
                nextCell.SetColor(currentCol.Value);
            }
            else
            {
                currentCell.SetColor(currentCol.Value);
            }
        }
        Color pCol = currentCol == null ? Color.white : Cell.colorVals[currentCol.Value];
        var sp = GetComponentInChildren<SpriteRenderer>();
        sp.color = pCol;

        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest,
                Mathf.Min(playerSpeed, Vector2.Distance(transform.position, dest)));
            //if (Vector2.Distance(transform.position, dest) < playerSpeed * Time.deltaTime)
            if (transform.position.x == dest.x && transform.position.y == dest.y)
            {
                currentCell = nextCell;
                nextCell = null;
                IsMoving = false;
            }
        }
        else
        {
            float horiz = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            if (horiz != 0) vert = 0;
            if (horiz != 0 || vert != 0)
            {
                int x = (int) horiz + currentCell.x;
                int y = (int) vert + currentCell.y;
                Cell possibleNext = RoomManager.Get(x, y);
                if (possibleNext != null)
                {
                    
                    bool isOctTile = RoomManager.instance.octopus.IsWithinOctopus(x, y);
                    bool prevOctTile = RoomManager.instance.octopus.IsWithinOctopus(currentCell.x, currentCell.y);
                    if (isOctTile || (int) possibleNext.CellType != 0)
                    {
                        Dirs dir = vectToDir[new Vector2(horiz, vert)];
                        if ((isOctTile && prevOctTile) || Cell.IsValidMove(dir, currentCell.CellType, possibleNext.CellType))
                        {
                            IsMoving = true;
                            dest = new Vector2(x, y);

                            float angle = Mathf.Atan2(-dirToVect[dir].x, dirToVect[dir].y)*Mathf.Rad2Deg;
                            spriteChild.transform.rotation = new Quaternion {eulerAngles = new Vector3(0, 0, angle)};

                            nextCell = possibleNext;

                            Update();
                            return;
                        }
                    }
                }
            }
        }
    }

    public static List<Dirs> GetPossibleDirs(Dirs dir)
    {
        List<Dirs> dirs = new List<Dirs>();
        Dirs opp = Cell.GetOppositeDir(dir);
        dirs.Add(dir);
        foreach (Dirs d in Enum.GetValues(typeof (Dirs)))
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

    public void CheckCollision()
    {
        var col = GetComponent<CircleCollider2D>();
        foreach (var enemy in Physics2D.OverlapCircleAll((Vector2) transform.position + col.offset, col.radius, enemies)
            )
        {
            //Debug.Log("Hard "  + RoomManager.hardMode);
            //Debug.Log("Enemy? " + (enemy.gameObject.GetComponent<Enemy>() != null));
            if ((RoomManager.hardMode && (enemy.gameObject.GetComponent<Enemy>() != null)) ||
                enemy.gameObject.GetComponent<Goat>() != null)
            {
                RoomManager.instance.pause.MenuToggle("GameOver");
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Colors
{
    Black,
    Red,
    Green,
    Blue,
    Yellow,
}
public enum Types
{
    Blank,
    Horiz,
    Vert,
    TopLeft,
    TopRight,
    BotRight,
    BotLeft,
}
public enum Dirs
{
    N,
    E,
    S,
    W,
}

public class Cell
{
    public int x { get; set; }
    public int y { get; set; }
    public Colors col { get; set; }
    private Types _type;
    public Types type { set { Orient(value); _type = value; } get { return _type; } }
    public GameObject go;
    public static GameObject template = Resources.Load<GameObject>("cellPrefab");
    public static Sprite[] sprs = Resources.LoadAll<Sprite>(@"roads_wht");
    public Enemy enemy;

    public static Dictionary<Types, HashSet<Dirs>> typeDirs = new Dictionary<Types, HashSet<Dirs>>()
    {
        { Types.Blank, new HashSet<Dirs>() },
        { Types.Vert, new HashSet<Dirs>() { Dirs.N, Dirs.S } },
        { Types.Horiz, new HashSet<Dirs>() { Dirs.E, Dirs.W } },
        { Types.TopLeft, new HashSet<Dirs>() { Dirs.N, Dirs.W } },
        { Types.TopRight, new HashSet<Dirs>() { Dirs.N, Dirs.E } },
        { Types.BotRight, new HashSet<Dirs>() { Dirs.S, Dirs.E } },
        { Types.BotLeft, new HashSet<Dirs>() { Dirs.S, Dirs.W } },
    };
    public static Dictionary<Colors, Color> colorVals = new Dictionary<Colors, Color>()
    {
        { Colors.Black, Color.black },
        { Colors.Red, Color.red},
        { Colors.Blue, Color.blue },
        { Colors.Green, Color.green },
        { Colors.Yellow, Color.yellow },
    };
    public static Dirs GetOppositeDir(Dirs d)
    {
        return (Dirs)(((int)d + 2) % Enum.GetValues(typeof(Dirs)).Length);
    }
    public static bool IsValidMove(Dirs direction, Types prev, Types dest)
    {
        Dirs opp = GetOppositeDir(direction);
        return (typeDirs[dest].Contains(opp) || typeDirs[prev].Contains(direction));
    }

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.col = Colors.Black;
        go = (GameObject)GameObject.Instantiate(template);
        go.tag = "cell";
        go.transform.position = new Vector2(x, y);
        Orient(type);
    }
    public void SetColor(Colors color)
    {
        if (col == color) return;
        col = color;
        if (go != null)
        {
            var sp = go.GetComponent<SpriteRenderer>();
            sp.color = colorVals[col];
        }
        
    }
    private void Orient(Types t)
    {
        var spr = go.GetComponent<SpriteRenderer>();
       
        switch (t)
        {
            case Types.Blank:
                spr.sprite = null;
                break;
            default:
                spr.sprite = sprs[(int)t- 1];
                    break;
        }

    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

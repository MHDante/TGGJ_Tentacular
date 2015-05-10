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
    public int decayMax = 15, decayLeft;
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
        { Colors.Black, Color.white },
        { Colors.Red, Color.red},
        { Colors.Blue, Color.blue },
        { Colors.Green, Color.green },
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
        go.tag = "generated";
        go.transform.parent = RoomManager.roomManager.gridObject.transform;
        go.transform.position = new Vector2(x, y);
        Orient(type);
        decayLeft = decayMax;
    }
    public void SetColor(Colors color)
    {
        if (col == color) return;
        col = color;
        if (go != null)
        {
            var sp = go.GetComponent<SpriteRenderer>();
            Color c = colorVals[col] == Color.white ? Color.black : colorVals[col];
            sp.material.SetColor("_LastColor", sp.color);
            sp.color = c;

            decayLeft = decayMax;
            //SetAlpha();
            lastChangeTime = Time.time;
            Update();
        }
    }
    public void Decay()
    {
        decayLeft--;
        if (decayLeft <= 0)
        {
            SetColor(Colors.Black);
        }
        else
        {
            SetAlpha();
        }
    }
    float timeItTakes = .5f;
    float? lastChangeTime = null;
    void SetAlpha()
    {
        float percent = (float)decayLeft / (float)decayMax;
        float range = 0.8f;
        percent = percent * range + (1f - range);
        var sp = go.GetComponent<SpriteRenderer>();
        Color temp = Cell.colorVals[col];
        temp *= percent;
        temp.a = 1f;
        sp.color = temp;

        

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
    public void Update()
    {
        if (lastChangeTime.HasValue)
        {
            float diff = Time.time - lastChangeTime.Value;
            Vector4 vect = new Vector4(1, 1, 1, 1);
            if (diff > timeItTakes)
            {
                lastChangeTime = null;
            }
            else
            {
                float ratio = diff / timeItTakes;
                int[] arr = { 0, 1, 0, -1, 1, 0, -1, 0 };

                for (int i = 0; i < arr.Length; i += 2)
                {
                    int x = arr[i], y = arr[i + 1];
                    Cell c = RoomManager.Get(x+this.x, y+this.y);
                    var newRatio = c.lastChangeTime == null ? ratio : Mathf.Min(ratio, (Time.time - c.lastChangeTime.Value) / timeItTakes);
                    if (c != null && c.col == col)
                    {
                        vect[i / 2] = newRatio;
                    }
                    else
                    {
                        vect[i / 2] = newRatio -2;
                    }
                }
            }
            
            var sp = go.GetComponent<SpriteRenderer>();
            sp.material.SetVector("_UDLR", vect);

        }
    }
}

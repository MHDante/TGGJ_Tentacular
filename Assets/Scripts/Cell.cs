using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class Cell
{
    public static GameObject template = Resources.Load<GameObject>("cellPrefab");
    public static Sprite[] sprs = Resources.LoadAll<Sprite>(@"roads_wht");

    public static Dictionary<Types, HashSet<Dirs>> typeDirs = new Dictionary<Types, HashSet<Dirs>>
    {
        {Types.Blank, new HashSet<Dirs>()},
        {Types.Vert, new HashSet<Dirs> {Dirs.N, Dirs.S}},
        {Types.Horiz, new HashSet<Dirs> {Dirs.E, Dirs.W}},
        {Types.TopLeft, new HashSet<Dirs> {Dirs.N, Dirs.W}},
        {Types.TopRight, new HashSet<Dirs> {Dirs.N, Dirs.E}},
        {Types.BotRight, new HashSet<Dirs> {Dirs.S, Dirs.E}},
        {Types.BotLeft, new HashSet<Dirs> {Dirs.S, Dirs.W}}
    };

    public static Dictionary<Colors, Color> colorVals = new Dictionary<Colors, Color>
    {
        {Colors.Black, Color.white},
        {Colors.Red, Color.red},
        {Colors.Blue, Color.blue},
        {Colors.Green, Color.green}
    };

    private bool _fromMid;
    private float? _lastChangeTime;
    private Types _type;
    public Colors color;
    public int decayLeft;
    public GameObject gameObject;
    public int x;
    public int y;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.color = Colors.Black;
        gameObject = Object.Instantiate(template);
        gameObject.tag = "generated";
        gameObject.transform.parent = RoomManager.instance.gridObject.transform;
        gameObject.transform.position = new Vector2(x, y);
        Orient(CellType);
        decayLeft = Values.CellDecayMax;
    }

    public Types CellType
    {
        set
        {
            Orient(value);
            _type = value;
        }
        get { return _type; }
    }

    public static Dirs GetOppositeDir(Dirs d)
    {
        return (Dirs) (((int) d + 2)%Enum.GetValues(typeof (Dirs)).Length);
    }

    public static bool IsValidMove(Dirs direction, Types prev, Types dest)
    {
        Dirs opp = GetOppositeDir(direction);
        return (typeDirs[dest].Contains(opp) || typeDirs[prev].Contains(direction));
    }

    public void SetColor(Colors col)
    {
        if (this.color == col & decayLeft == Values.CellDecayMax) return;
        this.color = col;
        if (gameObject != null)
        {
            var sp = gameObject.GetComponent<SpriteRenderer>();
            Color c = colorVals[color] == Color.white ? Color.black : colorVals[color];
            sp.material.SetColor("_LastColor", sp.color);
            sp.color = c;

            decayLeft = Values.CellDecayMax;
            _lastChangeTime = Time.time;
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

    private void SetAlpha()
    {
        float percent = decayLeft/(float) Values.CellDecayMax;
        float range = 0.8f;
        percent = percent*range + (1f - range);
        var sp = gameObject.GetComponent<SpriteRenderer>();
        Color temp = colorVals[color];
        temp *= percent;
        temp.a = 1f;
        sp.color = temp;
    }

    private void Orient(Types t)
    {
        var spr = gameObject.GetComponent<SpriteRenderer>();

        switch (t)
        {
            case Types.Blank:
                spr.sprite = null;
                break;
            default:
                spr.sprite = sprs[(int) t - 1];
                break;
        }
    }

    public void Update()
    {
        if (_lastChangeTime.HasValue)
        {
            float diff = Time.time - _lastChangeTime.Value;
            Vector4 vect = new Vector4(1, 1, 1, 1);
            if (diff > Values.CellAnimationTime)
            {
                _lastChangeTime = null;
                _fromMid = false;
            }
            else
            {
                float ratio = diff/Values.CellAnimationTime;
                int[] arr = {0, 1, 0, -1, 1, 0, -1, 0};

                for (int i = 0; i < arr.Length; i += 2)
                {
                    Cell c = RoomManager.Get(arr[i] + x, arr[i + 1] + y);

                    if (c != null && c.color == color && !_fromMid)
                    {
                        var newRatio = c._lastChangeTime == null
                            ? ratio
                            : Mathf.Min(ratio, (Time.time - c._lastChangeTime.Value)/Values.CellAnimationTime);
                        vect[i/2] = newRatio;
                    }
                    else
                    {
                        vect[i/2] = ratio - 2;
                        _fromMid = true;
                    }
                }
            }

            var sp = gameObject.GetComponent<SpriteRenderer>();
            sp.material.SetVector("_UDLR", vect);
        }
    }
}

public enum Colors
{
    Black,
    Red,
    Green,
    Blue
}

public enum Types
{
    Blank,
    Horiz,
    Vert,
    TopLeft,
    TopRight,
    BotRight,
    BotLeft
}

public enum Dirs
{
    N,
    E,
    S,
    W
}
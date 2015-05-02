using UnityEngine;
using System.Collections;
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
    Vert,
    Horiz,
    TopLeft,
    TopRight,
    BotRight,
    BotLeft,
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
    public static Sprite[] sprs = Resources.LoadAll<Sprite>(@"roads");

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

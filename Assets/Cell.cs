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
    BotLeft,
    BotRight,
}
public class Cell
{
    public int x { get; set; }
    public int y { get; set; }
    public Colors col { get; set; }
    public Types type { get; set; }
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.col = Colors.Black;

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

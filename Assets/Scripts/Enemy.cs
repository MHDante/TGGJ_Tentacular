using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public static bool WinningState = false;
    public Colors col;
    public Cell currentCell;
    private int currentStep = 0;
    private Vector2 dest = Vector2.zero;
    private Cell destCell;
    public float enemySpeed;
    public Dirs prevDir;
    private GameObject spriteChild;

    private void Awake()
    {
        spriteChild = transform.FindChild("spriteChild").gameObject;
    }

    // Use this for initialization
    private void Start()
    {
        currentCell = RoomManager.instance.grid[(int) transform.position.x][(int) transform.position.y];
        enemySpeed = 0.04f;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("E Enter.");
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
        if (enemies.Length != RoomManager.instance.maxEnemies) win = false;
        Dictionary<Colors, int> dict = new Dictionary<Colors, int>()
        {
            {Colors.Red, 0},
            {Colors.Green, 0},
            {Colors.Blue, 0},
        };
        foreach (var e in enemies)
        {
            if (e.col != e.currentCell.color)
            {
                dict[e.col]++;
                win = false;
            }
        }
        foreach (var c in dict.Keys)
        {
            string name = c.ToString().ToLower() + "FishNumber";
            var go = GameObject.Find(name);
            var txt = go.GetComponent<Text>();
            txt.text = dict[c].ToString();
        }
        return win;
    }

    // Update is called once per frame
    private void Update()
    {
        if (RoomManager.instance.IsPaused())
            return;
        if (currentStep == RoomManager.instance.gameSteps && currentStep != 0)
        {
            transform.position = Vector3.Lerp(currentCell.gameObject.transform.position, dest,
                RoomManager.instance.transitionPercent);
            RoomManager.instance.octopus.ChangeState();
        }
        else
        {
            currentCell = destCell ?? currentCell;
            currentStep = RoomManager.instance.gameSteps;
            if (currentCell.color == col)
            {
                currentCell.Decay();
            }
            WinningState = IsWinCondition();

            bool turnAround = false;
            List<Dirs> firstDirChoices = new List<Dirs>();
            List<Dirs> secondDirChoices = new List<Dirs>();
            Dirs? colorDir = null;
            int highestDecay = 0;
            foreach (Dirs d in Player.dictPossibleDirs[prevDir])
            {
                Dirs opp = Cell.GetOppositeDir(d);
                Vector2 next = Player.dirToVect[d] + (Vector2) currentCell.gameObject.transform.position;
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
                        //if (RoomManager.roomManager.player.currentCell == nextCell)
                        //{
                        //    Debug.Log("You are Dead.");
                        //}
                        if (col == nextCell.color && colorDir == null && nextCell.decayLeft > highestDecay)
                        {
                            colorDir = d;
                            highestDecay = nextCell.decayLeft;
                        }
                        else if (nextCell.color == Colors.Black)
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

    private void MoveInDir(Dirs d)
    {
        Vector2 next = Player.dirToVect[d] + (Vector2) currentCell.gameObject.transform.position;
        Cell c = RoomManager.Get((int) next.x, (int) next.y);
        currentStep = RoomManager.instance.gameSteps;
        dest = next;
        float angle = Mathf.Atan2(-Player.dirToVect[d].x, Player.dirToVect[d].y)*Mathf.Rad2Deg;
        spriteChild.transform.rotation = new Quaternion {eulerAngles = new Vector3(0, 0, angle)};

        destCell = c;
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
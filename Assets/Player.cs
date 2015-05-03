using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public float playerSpeed;
    public Cell currentCell;
	// Use this for initialization
	void Start () {
        currentCell = RoomManager.roomManager.Grid[(int)transform.position.x][(int)transform.position.y];
        playerSpeed = 0.05f;
    }
    bool IsMoving = false;
    Vector2 dest = Vector2.zero;
    Cell nextCell;
	// Update is called once per frame
	void Update () {
        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, playerSpeed);
            if (transform.position.x == dest.x && transform.position.y == dest.y)
            {
                currentCell = nextCell;
                IsMoving = false;
                if (RoomManager.roomManager.octopus.IsWithinOctopus(currentCell.x, currentCell.y))
                {
                    Debug.Log("WIN");
                }
            }
            //Debug.Log("moving");
        }
        else
        {
            float horiz = Input.GetAxisRaw("Horizontal");
            float vert = Input.GetAxisRaw("Vertical");
            //Debug.Log(horiz + "  :  " + vert);
            if (horiz != 0) vert = 0;
            else if (vert == 0) return;
            int x = (int)horiz + currentCell.x;
            int y = (int)vert + currentCell.y;
            if (!RoomManager.IsWithinGrid(x, y)) return;
            Cell possibleNext = RoomManager.Get(x, y);
            bool isOctTile = RoomManager.roomManager.octopus.IsWithinOctopus(x, y);
            if (!isOctTile && (int)possibleNext.type == 0) return;
            Dirs dir = Dirs.N;
            if (horiz == 1) dir = Dirs.E;
            else if (horiz == -1) dir = Dirs.W;
            else if (vert == 1) dir = Dirs.N;
            else if (vert == -1) dir = Dirs.S;
            
            if (isOctTile || Cell.IsValidMove(dir, currentCell.type, possibleNext.type))
            {
                IsMoving = true;
                dest = new Vector3(x, y);
                nextCell = possibleNext;
            }
        }
        
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

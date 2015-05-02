using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    public int x, y;
    public float playerSpeed;
    public Cell currentCell;
	// Use this for initialization
	void Start () {
        x = (int)transform.position.x;
        y = (int)transform.position.y;
        currentCell = RoomManager.roomManager.Grid[x][y];
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
            IsMoving = true;
            dest = new Vector3(x, y);
            nextCell = RoomManager.Get(x, y);
        }
        
	}
}

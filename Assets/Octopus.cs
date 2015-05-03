using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Octopus : MonoBehaviour {
    public Cell currentCell;
    public List<Cell> spawnCells = new List<Cell>();
	// Use this for initialization
	void Start () {
	    
	}
    void FindSpawnCells()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
    public bool IsWithinOctopus(int x, int y)
    {
        return x >= currentCell.x && x < currentCell.x + 5
            && y >= currentCell.y && y < currentCell.y + 5;
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

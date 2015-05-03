using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Octopus : MonoBehaviour {
    public Cell currentCell;
    public List<Cell> spawnCells = new List<Cell>();
    public Dictionary<Cell, Dirs> cellDirs = new Dictionary<Cell, Dirs>();
    public float spawnInterval = 2f;
    float timer = 0f;
    Colors lastEnemyCol = Colors.Blue;
    //public int maxEnemies = 6;
    int enemyCounter = 0;
	// Use this for initialization
	void Start () {
        FindSpawnCells();
    }
    void FindSpawnCells()
    {
        for (int x = 0; x < 5; x++)
        {
            CheckSpawnCellValidity(new Vector2(x, -1), Dirs.N);
        }
        for (int y = 0; y < 5; y++)
        {
            CheckSpawnCellValidity(new Vector2(5, y), Dirs.W);
        }
        for (int x = 5; x >=0; x--)
        {
            CheckSpawnCellValidity(new Vector2(x, 5), Dirs.S);
        }
        for (int y = 5; y >=0; y--)
        {
            CheckSpawnCellValidity(new Vector2(-1, y), Dirs.E);
        }
    }
    void CheckSpawnCellValidity(Vector2 pos, Dirs dir)
    {
        Vector2 v = pos + (Vector2)transform.position;
        Cell c = RoomManager.Get((int)v.x, (int)v.y);
        if (c != null)
        {
            if (Cell.typeDirs[c.type].Contains(dir))
            {
                spawnCells.Add(c);
                cellDirs[c] = Cell.GetOppositeDir(dir);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if (RoomManager.roomManager.IsPaused ())
			return;
        if (enemyCounter < RoomManager.roomManager.maxEnemies)
        {
        timer += Time.deltaTime;
        if (timer > spawnInterval)
        {
            timer = 0;
            SpawnEnemy();
        }
	}
	}
    int enemyIndex = 0;
    public void SpawnEnemy()
    {
        Cell c = spawnCells.ToArray()[enemyIndex];
        enemyIndex = (enemyIndex + 1) % spawnCells.Count;

        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("enemyPrefab"));
        Enemy enemy = go.GetComponent<Enemy>();
        enemy.SetCell(c.x, c.y);
        enemy.SetColor(lastEnemyCol);
        enemy.prevDir = cellDirs[c];

        int colIndex = ((int)lastEnemyCol + 1) % 4;
        if (colIndex == 0) colIndex++;
        lastEnemyCol = (Colors)colIndex;

        enemyCounter++;
        //start enemy movement
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

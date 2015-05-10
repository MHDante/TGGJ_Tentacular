using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Octopus : MonoBehaviour {
    public enum OctopusStates
    {
        Normal,
        Happy,
        Angry,
    }
    public Cell currentCell;
    public List<Cell> spawnCells = new List<Cell>();
    public Dictionary<Cell, Dirs> cellDirs = new Dictionary<Cell, Dirs>();
    public float spawnInterval = 2f;
    float timer = 0f;
    Colors lastEnemyCol = Colors.Red;
    //public int maxEnemies = 6;
    int enemyCounter = 0;
    public OctopusStates state = OctopusStates.Normal;

    public Sprite OctNormal;
    public Sprite OctAngry;
    public Sprite OctHappy;

    public float timeUntilGoat = 10;
    Texture2D fishtex;
    // Use this for initialization
    void Start () {
        FindSpawnCells();
        var sp = GetComponent<SpriteRenderer>();
        sp.color = Color.black;
        timeUntilGoat = (float)RoomManager.roomManager.secondsUntilGoat;

        
        fishtex = new Texture2D(RoomManager.roomManager.maxEnemies, 1, TextureFormat.ARGB32, false);
        for(int i = 0; i < RoomManager.roomManager.maxEnemies; i++)
        {
            fishtex.SetPixel(i, 0, new Color(0, 0, 0, 0));
        }
        fishtex.Apply();
        sp.material.SetTexture("_FishTex", fishtex);
        sp.material.SetInt("_FishTexLen", RoomManager.roomManager.maxEnemies);
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
    float hue = 0f, huespeed = 5f, val = 0f, valspeed = 0.25f;
	// Update is called once per frame
	void Update () {
		if (RoomManager.roomManager.IsPaused ())
			return;

        for(int i =0; i <  RoomManager.roomManager.maxEnemies; i++)
        {
            if (i >= enemies.Count) break;
            var e = enemies[i];
            float cap = (e.currentCell.col == e.col) ? 1 : 0;
            Color c = Cell.colorVals[e.col];
            c.a = cap;
            fishtex.SetPixel(i, 0, c);
        }
        fishtex.Apply();

        var sp = GetComponent<SpriteRenderer>();
        sp.material.SetTexture("_FishTex", fishtex);


        if (timeUntilGoat > 0)
        {
            timeUntilGoat -= Time.deltaTime;
            if (timeUntilGoat <= 0)
            {
                timeUntilGoat = 0;
                SpawnGoat();
            }
            var go = GameObject.Find("GoatCounter");
            var txt = go.GetComponent<Text>();
            txt.text = string.Format("{0:00.0}",timeUntilGoat);
        }
        
        if (enemyCounter < RoomManager.roomManager.maxEnemies)
        {
            timer += Time.deltaTime;
            if (timer > spawnInterval)
            {
                timer = 0;
                SpawnEnemy();
            }
	    }
        if (state == OctopusStates.Happy)
        {
            val += valspeed;
            if (val > 100f) val = 100f;
            sp.material.SetFloat("_Percent", val / 100f);
            //hue = (hue + huespeed) % 360;
            //Color c = HSVToRGB(hue / 360f, 1f - val/100f, val/100f);
            //sp.color = c;

            if (val == 100f)
            {
                //Debug.Log("TRUE WIN!");
                if (RoomManager.roomManager.octopus.IsWithinOctopus(RoomManager.roomManager.player.currentCell.x, RoomManager.roomManager.player.currentCell.y))
                {
                    //Debug.Log("WIN");
                    if (string.IsNullOrEmpty(RoomManager.roomManager.nextlevel))
                    {
                        //Application.LoadLevel("TitleScreen");
                        RoomManager.roomManager.gameObject.GetComponent<Pause>().MenuToggle("Victory");
                    }
                    else
                    {
                        FileWrite.InitDeserialization(RoomManager.roomManager.nextlevel);
                        Hints.Level++;
                    }
                    return;
                }
            }
        }
        else
        {
            sp.material.SetFloat("_Percent", 0);
        }
	}
    int enemyIndex = 0;
    List<Enemy> enemies = new List<Enemy>();
    public void SpawnEnemy()
    {
        Cell c = spawnCells.ToArray()[enemyIndex];
        enemyIndex = (enemyIndex + 1) % spawnCells.Count;

        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("enemyPrefab"));
        go.transform.parent = RoomManager.roomManager.entityObject.transform;

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.SetCell(c.x, c.y);
        enemy.SetColor(lastEnemyCol);
        enemy.prevDir = cellDirs[c];

        int colIndex = ((int)lastEnemyCol + 1) % (RoomManager.roomManager.differentColors + 1);
        if (colIndex == 0) colIndex++;
        lastEnemyCol = (Colors)colIndex;

        enemyCounter++;

        enemies.Add(enemy);
        //start enemy movement
    }
    public void SpawnGoat()
    {
        Cell c = spawnCells.ToArray()[0];

        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("goatPrefab"));
        Goat goat = go.GetComponent<Goat>();
        goat.SetCell(c.x, c.y);
        goat.prevDir = cellDirs[c];
    }
    public void ChangeState()
    {
        if (Enemy.WinningState)
        {
            if (state != OctopusStates.Happy)
            {
                state = OctopusStates.Happy;
                var sp = GetComponent<SpriteRenderer>();
                sp.sprite = OctHappy;
                hue = 0f;
                val = 0f;
            }
        }
        else
        {
            if (state != OctopusStates.Normal)
            {
                state = OctopusStates.Normal;
                var sp = GetComponent<SpriteRenderer>();
                sp.sprite = OctNormal;
                sp.color = Color.black;
            }
        }
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

    public static Color HSVToRGB(float H, float S, float V)
    {
        if (S == 0f)
            return new Color(V, V, V);
        else if (V == 0f)
            return Color.black;
        else
        {
            Color col = Color.black;
            float Hval = H * 6f;
            int sel = Mathf.FloorToInt(Hval);
            float mod = Hval - sel;
            float v1 = V * (1f - S);
            float v2 = V * (1f - S * mod);
            float v3 = V * (1f - S * (1f - mod));
            switch (sel + 1)
            {
                case 0:
                    col.r = V;
                    col.g = v1;
                    col.b = v2;
                    break;
                case 1:
                    col.r = V;
                    col.g = v3;
                    col.b = v1;
                    break;
                case 2:
                    col.r = v2;
                    col.g = V;
                    col.b = v1;
                    break;
                case 3:
                    col.r = v1;
                    col.g = V;
                    col.b = v3;
                    break;
                case 4:
                    col.r = v1;
                    col.g = v2;
                    col.b = V;
                    break;
                case 5:
                    col.r = v3;
                    col.g = v1;
                    col.b = V;
                    break;
                case 6:
                    col.r = V;
                    col.g = v1;
                    col.b = v2;
                    break;
                case 7:
                    col.r = V;
                    col.g = v3;
                    col.b = v1;
                    break;
            }
            col.r = Mathf.Clamp(col.r, 0f, 1f);
            col.g = Mathf.Clamp(col.g, 0f, 1f);
            col.b = Mathf.Clamp(col.b, 0f, 1f);
            return col;
        }
    }
}

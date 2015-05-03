using UnityEngine;
//using OrbItUtils;
using System.Xml.Linq;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
public static class FileWrite
{
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.S)) InitSerialization();
    //    if (Input.GetKeyDown(KeyCode.D)) InitDeserialization();
    //    if (Input.GetKeyDown(KeyCode.T)) RoomManager.roomManager.RefreshColorFamilyAll();
    //}
    static string defaultFileName = null;
    public static void InitSerialization(float? newX = null, float? newY = null)
    {
        if (RoomManager.roomManager == null) return;
        XElement all = SerializeGrid(newX,newY);
        var fname = RoomManager.roomManager.levelName;
        if (string.IsNullOrEmpty(fname))
        {
#if UNITY_EDITOR
            fname = Application.isPlaying ? Application.loadedLevelName : Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
#else
            fname = Application.loadedLevelName;
#endif
        }
        fname = WWW.EscapeURL(fname);
        Debug.Log("Writing file: " + fname);
        WriteFile(fname + ".xml", all.ToString());
        
    }
    static string WriteFile(string filename, string text)
    {
        string path = GetPath();
        string fullFileName = path + "/SavedLevels/" + filename;
        //string fullFileName = path + "/" + filename;
        StreamWriter fileWriter = File.CreateText(fullFileName);
        //fileWriter.WriteLine("Hello world");
        fileWriter.Write(text);
        fileWriter.Close();


        Debug.Log("=======PERSISTENTDATAPATH:" + Application.persistentDataPath);
        Debug.Log("=================DATAPATH:" + Application.dataPath);

        return fullFileName;
    }
    static string GetPath()
    {
        string path = Application.persistentDataPath;
#if UNITY_EDITOR
        path = Application.dataPath;
#endif
        return path;
    }
    public static void InitDeserialization(string filename)
    {
        //MonoBehaviour.Destroy(RoomManager.roomManager);
        //RoomManager.roomManager = null;
        Application.LoadLevel("Blank");
        FileWrite.defaultFileName = filename;
    }
    public static bool DeserializationCallback(string filename = null)
    {

        RoomManager room = MonoBehaviour.FindObjectOfType<RoomManager>();

        string path = GetPath();

        XElement loaded;
        try
        {
            loaded = XElement.Load(Application.dataPath + "/SavedLevels/" + (filename ?? defaultFileName));
            //XElement loaded = XElement.Load(path + "/" + defaultFileName + ".xml");
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        XElement meta = loaded.Element(XName.Get("Meta"));
        room.levelName = meta.Attribute("LevelName").Value;
        room.nextlevel = meta.Attribute("NextLevel") == null ? null : meta.Attribute("NextLevel").Value;

        XElement grid = loaded.Element("Grid");
        RoomManager.roomManager.gridWidth = int.Parse(grid.Attribute("Width").Value);
        RoomManager.roomManager.gridHeight = int.Parse(grid.Attribute("Height").Value);
        RoomManager.roomManager.GenerateEmptyGrid();

        XAttribute elemPlayerX = grid.Attribute("PlayerX");
        XAttribute elemPlayerY = grid.Attribute("PlayerY");
        bool exists = elemPlayerX != null && elemPlayerY != null;
        RoomManager.roomManager.PlayerStartX = exists ? int.Parse(elemPlayerX.Value) : 0;
        RoomManager.roomManager.PlayerStartY = exists ? int.Parse(elemPlayerY.Value) : 0;
        GameObject go = (GameObject)GameObject.Instantiate(Resources.Load("playerPrefab"));
        RoomManager.roomManager.player = go.GetComponent<Player>();
        RoomManager.roomManager.player.SetCell(RoomManager.roomManager.PlayerStartX, RoomManager.roomManager.PlayerStartY);

        XAttribute elemOctopusX = grid.Attribute("OctopusX");
        XAttribute elemOctopusY = grid.Attribute("OctopusY");
        bool exists2 = elemOctopusX != null && elemOctopusY != null;
        RoomManager.roomManager.OctopusX = exists2 ? int.Parse(elemOctopusX.Value) : 0;
        RoomManager.roomManager.OctopusY = exists2 ? int.Parse(elemOctopusY.Value) : 0;
        GameObject go2 = (GameObject)GameObject.Instantiate(Resources.Load("octopusPrefab"));
        RoomManager.roomManager.octopus = go2.GetComponent<Octopus>();
        RoomManager.roomManager.octopus.SetCell(RoomManager.roomManager.OctopusX, RoomManager.roomManager.OctopusY);



        XAttribute elemMaxEnemies = grid.Attribute("MaxEnemies");
        if (elemMaxEnemies != null)
        {
            RoomManager.roomManager.maxEnemies = int.Parse(elemMaxEnemies.Value);
        }
        XAttribute elemDifferentColors = grid.Attribute("DifferentColors");
        if (elemDifferentColors != null)
        {
            RoomManager.roomManager.differentColors = int.Parse(elemDifferentColors.Value);
        }

        foreach (XElement row in grid.Elements("Row"))
        {
            foreach (XElement eCell in row.Elements("Cell"))
            {
                int cellx = int.Parse(eCell.Attribute("x").Value);
                int celly = int.Parse(eCell.Attribute("y").Value);
                Cell cell = RoomManager.roomManager.Grid[cellx][celly];
                //pick up fields
                cell.col = (Colors)Enum.Parse(typeof(Colors), eCell.Attribute("Color").Value);
                cell.type = (Types)Enum.Parse(typeof(Types), eCell.Attribute("Type").Value);
            }
        }

        return true;
    }
    public static XElement SerializeGrid(float? newX = null, float? newY = null)
    {
        
        XElement eRoot = new XElement("Root");

        XElement eInfo = new XElement("Meta");
        eRoot.Add(eInfo);
        eInfo.Add(new XAttribute("LevelName", RoomManager.roomManager.levelName));
        eInfo.Add(new XAttribute("NextLevel", RoomManager.roomManager.nextlevel??""));

        XElement eGrid = new XElement("Grid");
        eRoot.Add(eGrid);

        //var grid = RoomManager.roomManager.Grid;

        var width = newX?? RoomManager.roomManager.Grid.Length;
        var height = newY?? RoomManager.roomManager.Grid[0].Length;
        eGrid.Add(new XAttribute("Width", width));
        eGrid.Add(new XAttribute("Height", height));
        eGrid.Add(new XAttribute("PlayerX", RoomManager.roomManager.PlayerStartX));
        eGrid.Add(new XAttribute("PlayerY", RoomManager.roomManager.PlayerStartY));
        eGrid.Add(new XAttribute("OctopusX", RoomManager.roomManager.OctopusX));
        eGrid.Add(new XAttribute("OctopusY", RoomManager.roomManager.OctopusY));
        eGrid.Add(new XAttribute("MaxEnemies", RoomManager.roomManager.maxEnemies));
        eGrid.Add(new XAttribute("DifferentColors", RoomManager.roomManager.differentColors));

        for (int y = 0; y < height; y++)
        {
            XElement eRow = new XElement("Row", new XAttribute("y", y));
            for (int x = 0; x < width; x++)
            {
                Cell cell = RoomManager.Get(x,y) ?? new Cell(x, y); ;
                XElement eCell = new XElement("Cell", new XAttribute("x", x), new XAttribute("y", y));
                eCell.Add(new XAttribute("Color", cell.col));
                eCell.Add(new XAttribute("Type", cell.type));
                eRow.Add(eCell);
                //add new data elems

            }
            eGrid.Add(eRow);
        }
        return eRoot;
    }

}
[System.AttributeUsage(System.AttributeTargets.Property |
                       System.AttributeTargets.Field)
]
public class SerializeBlockIt : Attribute { }

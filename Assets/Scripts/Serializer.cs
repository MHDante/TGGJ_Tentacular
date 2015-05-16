using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class FileWrite
{
    private static string _defaultFileName;

    public static void InitSerialization(float? newX = null, float? newY = null)
    {
        if (RoomManager.instance == null) return;
        XElement all = SerializeGrid(newX, newY);
        var fname = RoomManager.instance.levelName;
        if (string.IsNullOrEmpty(fname))
        {
            fname = Application.isPlaying
                ? Application.loadedLevelName
                : Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
        }
        fname = WWW.EscapeURL(fname);
        Debug.Log("Writing file: " + fname);
        WriteFile(fname + ".xml", all.ToString());
    }

    public static string WriteFile(string filename, string text)
    {
        string path = Application.dataPath;
        string fullFileName = path + "/SavedLevels/Resources/" + filename;
        StreamWriter fileWriter = File.CreateText(fullFileName);
        fileWriter.Write(text);
        fileWriter.Close();
        return fullFileName;
    }
    
    public static void InitDeserialization(string filename)
    {
        Application.LoadLevel("Blank");
        _defaultFileName = filename;
    }

    public static void DeserializationCallback(string filename = null)
    {
        RoomManager room = Object.FindObjectOfType<RoomManager>();
        XElement loaded;
        try
        {
            loaded = LoadLevel(filename);
        }
        catch (UnauthorizedAccessException)
        {
            return;
        }

        XElement meta = loaded.Element(XName.Get("Meta"));
        room.levelName = meta.Attribute("LevelName").Value;
        room.nextlevel = meta.Attribute("NextLevel") == null ? null : meta.Attribute("NextLevel").Value;

        XElement grid = loaded.Element("Grid");
        RoomManager.instance.gridWidth = int.Parse(grid.Attribute("Width").Value);
        RoomManager.instance.gridHeight = int.Parse(grid.Attribute("Height").Value);
        RoomManager.instance.GenerateEmptyGrid();

        XAttribute elemPlayerX = grid.Attribute("PlayerX");
        XAttribute elemPlayerY = grid.Attribute("PlayerY");
        bool exists = elemPlayerX != null && elemPlayerY != null;
        RoomManager.instance.playerStartX = exists ? int.Parse(elemPlayerX.Value) : 0;
        RoomManager.instance.playerStartY = exists ? int.Parse(elemPlayerY.Value) : 0;
        GameObject go = (GameObject) Object.Instantiate(Resources.Load("playerPrefab"));
        go.tag = "generated";
        go.transform.parent = RoomManager.instance.entityObject.transform;
        RoomManager.instance.player = go.GetComponent<Player>();
        RoomManager.instance.player.SetCell(RoomManager.instance.playerStartX,
            RoomManager.instance.playerStartY);

        XAttribute elemOctopusX = grid.Attribute("OctopusX");
        XAttribute elemOctopusY = grid.Attribute("OctopusY");
        bool exists2 = elemOctopusX != null && elemOctopusY != null;
        RoomManager.instance.octopusX = exists2 ? int.Parse(elemOctopusX.Value) : 0;
        RoomManager.instance.octopusY = exists2 ? int.Parse(elemOctopusY.Value) : 0;
        GameObject go2 = (GameObject) Object.Instantiate(Resources.Load("octopusPrefab"));
        go2.tag = "generated";
        go2.transform.parent = RoomManager.instance.entityObject.transform;
        RoomManager.instance.octopus = go2.GetComponent<Octopus>();
        RoomManager.instance.octopus.SetCell(RoomManager.instance.octopusX, RoomManager.instance.octopusY);


        XAttribute elemMaxEnemies = grid.Attribute("MaxEnemies");
        if (elemMaxEnemies != null)
        {
            RoomManager.instance.maxEnemies = int.Parse(elemMaxEnemies.Value);
        }
        XAttribute elemDifferentColors = grid.Attribute("DifferentColors");
        if (elemDifferentColors != null)
        {
            RoomManager.instance.differentColors = int.Parse(elemDifferentColors.Value);
        }
        XAttribute elemsecondsUntilGoat = grid.Attribute("secondsUntilGoat");
        if (elemsecondsUntilGoat != null)
        {
            RoomManager.instance.secondsUntilGoat = int.Parse(elemsecondsUntilGoat.Value);
        }

        foreach (XElement row in grid.Elements("Row"))
        {
            foreach (XElement eCell in row.Elements("Cell"))
            {
                int cellx = int.Parse(eCell.Attribute("x").Value);
                int celly = int.Parse(eCell.Attribute("y").Value);
                Cell cell = RoomManager.instance.grid[cellx][celly];
                //pick up fields
                cell.color = (Colors) Enum.Parse(typeof (Colors), eCell.Attribute("Color").Value);
                cell.CellType = (Types) Enum.Parse(typeof (Types), eCell.Attribute("Type").Value);
            }
        }
    }

    private static XElement LoadLevel(string filename)
    {

#if UNITY_EDITOR
        return XElement.Load(Application.dataPath + "/SavedLevels/Resources/" + (filename ?? _defaultFileName));

#else
            var name = Path.GetFileNameWithoutExtension(filename ?? defaultFileName);
            Debug.Log(name);
            TextAsset t = Resources.Load<TextAsset>(name);
            loaded = XElement.Parse(t.text);
#endif
    }

    public static XElement SerializeGrid(float? newX = null, float? newY = null)
    {
        XElement eRoot = new XElement("Root");

        XElement eInfo = new XElement("Meta");
        eRoot.Add(eInfo);
        eInfo.Add(new XAttribute("LevelName", RoomManager.instance.levelName));
        eInfo.Add(new XAttribute("NextLevel", RoomManager.instance.nextlevel ?? ""));

        XElement eGrid = new XElement("Grid");
        eRoot.Add(eGrid);

        var width = newX ?? RoomManager.instance.grid.Length;
        var height = newY ?? RoomManager.instance.grid[0].Length;
        eGrid.Add(new XAttribute("Width", width));
        eGrid.Add(new XAttribute("Height", height));
        eGrid.Add(new XAttribute("PlayerX", RoomManager.instance.playerStartX));
        eGrid.Add(new XAttribute("PlayerY", RoomManager.instance.playerStartY));
        eGrid.Add(new XAttribute("OctopusX", RoomManager.instance.octopusX));
        eGrid.Add(new XAttribute("OctopusY", RoomManager.instance.octopusY));
        eGrid.Add(new XAttribute("MaxEnemies", RoomManager.instance.maxEnemies));
        eGrid.Add(new XAttribute("DifferentColors", RoomManager.instance.differentColors));
        eGrid.Add(new XAttribute("secondsUntilGoat", RoomManager.instance.secondsUntilGoat));

        for (int y = 0; y < height; y++)
        {
            XElement eRow = new XElement("Row", new XAttribute("y", y));
            for (int x = 0; x < width; x++)
            {
                Cell cell = RoomManager.Get(x, y) ?? new Cell(x, y);
                XElement eCell = new XElement("Cell", new XAttribute("x", x), new XAttribute("y", y));
                eCell.Add(new XAttribute("Color", cell.color));
                eCell.Add(new XAttribute("Type", cell.CellType));
                eRow.Add(eCell);
            }
            eGrid.Add(eRow);
        }
        return eRoot;
    }
}

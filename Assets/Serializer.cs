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
    public static string InitSerialization(float? newX = null, float? newY = null)
    {
        XElement all = SerializeGrid(newX,newY);
        Debug.Log(all);
        var fname = MonoBehaviour.FindObjectOfType<MetaData>().levelName;
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
        return WriteFile(fname + ".xml", all.ToString());
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
        XElement loaded = XElement.Load(Application.dataPath + "/SavedLevels/" + (filename??defaultFileName));
        //XElement loaded = XElement.Load(path + "/" + defaultFileName + ".xml");
        XElement meta = loaded.Element(XName.Get("Meta"));
        MetaData metaData = (MetaData)MonoBehaviour.FindObjectOfType(typeof(MetaData));
        metaData.levelName = meta.Attribute("LevelName").Value;

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
        //RoomManager.roomManager.player.x = RoomManager.roomManager.PlayerStartX;
        //RoomManager.roomManager.player.y = RoomManager.roomManager.PlayerStartY;
        //RoomManager.roomManager.player.transform.position = new Vector3(RoomManager.roomManager.player.x, RoomManager.roomManager.player.y);

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
        MetaData auth = MonoBehaviour.FindObjectOfType<MetaData>();
        if (auth != null)
        {
            eInfo.Add(new XAttribute("LevelName", auth.levelName));
        }

        XElement eGrid = new XElement("Grid");
        eRoot.Add(eGrid);

        //var grid = RoomManager.roomManager.Grid;

        var width = newX?? RoomManager.roomManager.Grid.Length;
        var height = newY?? RoomManager.roomManager.Grid[0].Length;
        eGrid.Add(new XAttribute("Width", width));
        eGrid.Add(new XAttribute("Height", height));
        eGrid.Add(new XAttribute("PlayerX", RoomManager.roomManager.PlayerStartX));
        eGrid.Add(new XAttribute("PlayerY", RoomManager.roomManager.PlayerStartY));

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

    //public static XElement SerializeObject(object o)
    //{
    //    Type type = o.GetType();
    //
    //    XElement r = new XElement(type.Name);
    //    if (o is MonoBehaviour)
    //    {
    //        MonoBehaviour mono = (MonoBehaviour)o;
    //        if (mono.gameObject != null)
    //        {
    //            XAttribute att = new XAttribute("Name", mono.gameObject.name);
    //            r.Add(att);
    //            if (mono is GamePiece)
    //            {
    //                GamePiece gp = (GamePiece)mono;
    //                XAttribute att2 = new XAttribute("Zpos", gp.getZPosition());
    //                r.Add(att2);
    //            }
    //        }
    //    }
    //    foreach (var prop in type.GetProperties())
    //    {
    //        if (prop.GetCustomAttributes(typeof(SerializeBlockIt), true).Length > 0)
    //        {
    //            XElement p = new XElement("Property");
    //            XAttribute att = new XAttribute("Name", prop.Name);
    //            XAttribute att2 = new XAttribute("Type", prop.PropertyType);
    //            XAttribute att3 = new XAttribute("Value", prop.GetValue(o, null));
    //
    //            p.Add(att);
    //            p.Add(att2);
    //            p.Add(att3);
    //            r.Add(p);
    //        }
    //    }
    //    foreach (var field in type.GetFields())
    //    {
    //        if (field.GetCustomAttributes(typeof(SerializeBlockIt), true).Length > 0)
    //        {
    //            XElement f = new XElement("Field");
    //            XAttribute att = new XAttribute("Name", field.Name);
    //            XAttribute att2 = new XAttribute("Type", field.FieldType);
    //            XAttribute att3 = new XAttribute("Value", field.GetValue(o));
    //
    //            f.Add(att);
    //            f.Add(att2);
    //            f.Add(att3);
    //            r.Add(f);
    //        }
    //    }
    //    return r;
    //}

}
[System.AttributeUsage(System.AttributeTargets.Property |
                       System.AttributeTargets.Field)
]
public class SerializeBlockIt : Attribute { }

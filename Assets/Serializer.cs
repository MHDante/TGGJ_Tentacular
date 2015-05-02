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
    static string defaultFileName = "Battletoads+Tetris";
    public static string InitSerialization(string fileName = "")
    {
        XElement all = SerializeGrid();
        Debug.Log(all);
        string fname = "";
        if (!string.IsNullOrEmpty(fileName))
        {
            fname = fileName;
        }
        else
        {
            fname = MonoBehaviour.FindObjectOfType<MetaData>().levelName;
        }
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
        AwaitingDSCallback = true;
        FileWrite.defaultFileName = filename;
    }
    static bool AwaitingDSCallback = false;
    public static bool DeserializationCallback()
    {
        if (!AwaitingDSCallback)
        {
            return false;
        }
        AwaitingDSCallback = false;

        RoomManager room = MonoBehaviour.FindObjectOfType<RoomManager>();

        string path = GetPath();
        XElement loaded = XElement.Load(Application.dataPath + "/SavedLevels/" + defaultFileName);
        //XElement loaded = XElement.Load(path + "/" + defaultFileName + ".xml");
        XElement meta = loaded.Element(XName.Get("Meta"));
        MetaData metaData = (MetaData)MonoBehaviour.FindObjectOfType(typeof(MetaData));
        metaData.levelName = meta.Attribute("LevelName").Value;

        XElement grid = loaded.Element("Grid");

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
    public static XElement SerializeGrid()
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

        var grid = RoomManager.roomManager.Grid;
        eGrid.Add(new XAttribute("Width", grid[0].Length));
        eGrid.Add(new XAttribute("Height", grid.Length));


        for (int y = 0; y < grid[0].Length; y++)
        {
            XElement eRow = new XElement("Row", new XAttribute("y", y));
            for (int x = 0; x < grid.Length; x++)
            {
                Cell cell = grid[x][y];
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

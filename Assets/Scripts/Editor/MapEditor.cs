using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Linq;

public class MapEditor : EditorWindow {
    [MenuItem("BlockIt/PuzzleMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }
    void OnEnable()
    {
        if (EditorApplication.currentScene == "Assets/Scenes/Blank.unity") return;
        SceneView.onSceneGUIDelegate += OnUpdate;
        EditorApplication.playmodeStateChanged = () =>
        {
            levelname = RoomManager.roomManager.levelName;
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                FileWrite.InitSerialization();
            }
        };
    }
    void OnUpdate(SceneView sceneView)
    {
        if (EditorApplication.currentScene == "Assets/Scenes/Blank.unity") return;
        if (Event.current.type == EventType.keyDown)
        {
            if (Event.current.keyCode == KeyCode.LeftControl)
            {
                ctrldown = true;
            }
        }
        else if (Event.current.type == EventType.keyUp)
        {
            if (Event.current.keyCode == KeyCode.LeftControl)
            {
                ctrldown = false;
            }
        }

        if (!Active) return;
        Vector2 screenpos = Event.current.mousePosition;
        screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
        Vector2 MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;
        if (LeftDown())
        {
            CellAction(MousePos);
            Event.current.Use();
        }
        else if (RightDown())
        {
            CellAction(MousePos, true);
            Event.current.Use();
        }
        else if (MiddleDown())
        {
            if (ctrldown)
            {
                PlaceOctopus(MousePos);
            }
            else
            {
                PlacePlayer(MousePos);
            }
            Event.current.Use();
        }
        

        //Event.current.keyCode
    }
    bool ctrldown = false;
    Types lastType;
    public void CellAction(Vector2 pos, bool useLastType = false)
    {
        Cell c = RoomManager.GetFromWorldPos(pos.x, pos.y);
        if (c != null)
        {
            if (useLastType)
            {
                c.type = lastType;
            }
            else
            {
                c.type = (Types)(((int)c.type + 1) % Enum.GetValues(typeof(Types)).Length);
                lastType = c.type;
            }
        }
    }
    public void PlacePlayer(Vector2 pos)
    {
        RoomManager.roomManager.PlayerStartX = (int)pos.x;
        RoomManager.roomManager.PlayerStartY = (int)pos.y;
        RoomManager.roomManager.player.SetCell((int)pos.x, (int)pos.y);
    }
    public void PlaceOctopus(Vector2 pos)
    {
        RoomManager.roomManager.OctopusX = (int)pos.x;
        RoomManager.roomManager.OctopusY = (int)pos.y;
        RoomManager.roomManager.octopus.SetCell((int)pos.x, (int)pos.y);
        
    }
    bool RightDown()
    {
        return (Event.current.type == EventType.MouseDown && Event.current.button == 1);
    }
    bool LeftDown()
    {
        return (Event.current.type == EventType.MouseDown && Event.current.button == 0);
    }
    bool MiddleDown()
    {
        return (Event.current.type == EventType.MouseDown && Event.current.button == 2);
    }
    public bool Active;

    string result = "";
    string levelname;
    private string nextlevel = "";
    void OnGUI()
    {
        if (EditorApplication.currentScene == "Assets/Scenes/Blank.unity") return;
        Active = (EditorGUILayout.Toggle("Active", Active));
        string path = Application.dataPath + "/SavedLevels";
        var infos = new DirectoryInfo(path);
        //we're leaving this here
        var fileinfos = infos.GetFiles().Select(f => f.Name).Where(s => !s.Contains(".meta")).Union(new string[] { "" }).ToArray();
        result = fileinfos[EditorGUILayout.Popup("Load:", fileinfos.ToList().IndexOf(result), fileinfos)];

        if (GUILayout.Button("Load"))
        {
            if (string.IsNullOrEmpty(result)) return;
            //Debug.Log(result);
            if (Application.isPlaying)
                FileWrite.InitDeserialization(result);
            else
            {
                levelname = result.Replace(".xml", "");
                RoomManager.roomManager.levelName = levelname;
                RoomManager.roomManager.Awake();
                EditorUtility.SetDirty(RoomManager.roomManager);

            }
        }
        var tempname = EditorGUILayout.TextField("Name:",levelname);
        if (tempname != levelname)
        {
            levelname = tempname;
            RoomManager.roomManager.levelName = levelname;
            EditorUtility.SetDirty(RoomManager.roomManager);
        }

        var tempnext = fileinfos[EditorGUILayout.Popup("NextLevel:", fileinfos.ToList().IndexOf(nextlevel), fileinfos)];
        if (nextlevel != tempnext)
        {
            nextlevel = tempnext;
            RoomManager.roomManager.nextlevel = nextlevel;
            EditorUtility.SetDirty(RoomManager.roomManager);
        }
            if (GUILayout.Button("Save"))
            {

            RoomManager.roomManager.nextlevel = nextlevel;
            EditorUtility.SetDirty(RoomManager.roomManager);

            FileWrite.InitSerialization();
        }

    }
}

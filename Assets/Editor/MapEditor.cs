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
        SceneView.onSceneGUIDelegate += OnUpdate;
        EditorApplication.playmodeStateChanged = () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                FileWrite.InitSerialization();
            }
        };
    }
    void OnUpdate(SceneView sceneView)
    {
        if (!Active) return;
        Vector2 screenpos = Event.current.mousePosition;
        screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
        Vector2 MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;
        if (LeftDown())
        {
            Debug.Log(MousePos);
            CellAction(MousePos);
        }
    }
    public void CellAction(Vector2 pos)
    {
        Cell c = RoomManager.GetFromWorldPos(pos.x, pos.y);
        if (c != null)
        {
            c.type = (Types)(((int)c.type + 1) % Enum.GetValues(typeof(Types)).Length);
        }
    }
    bool RightDown()
    {
        return (Event.current.type == EventType.MouseDown && Event.current.button == 1);
    }
    bool LeftDown()
    {
        return (Event.current.type == EventType.MouseDown && Event.current.button == 0);
    }
    public bool Active;

    string result = "";
    void OnGUI()
    {
        Active = (EditorGUILayout.Toggle("Active", Active));
        string path = Application.dataPath + "/SavedLevels";
        var infos = new DirectoryInfo(path);
        //we're leaving this here
        var fileinfos = infos.GetFiles().Select(f => f.Name).Where(s => !s.Contains(".meta")).Union(new string[] { "" }).ToArray();
        result = fileinfos[EditorGUILayout.Popup("Choose Filename", fileinfos.ToList().IndexOf(result), fileinfos)];

        if (GUILayout.Button("Load"))
        {
            if (string.IsNullOrEmpty(result)) return;
            //Debug.Log(result);
            if (Application.isPlaying)
                FileWrite.InitDeserialization(result);
        }
        if (GUILayout.Button("Save"))
        {
            FileWrite.InitSerialization();
        }

    }
}

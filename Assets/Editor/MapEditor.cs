using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class MapEditor : EditorWindow {
    [MenuItem("BlockIt/PuzzleMaker")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }
    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnUpdate;
    }
    void OnUpdate(SceneView sceneView)
    {
        Vector2 screenpos = Event.current.mousePosition;
        screenpos.y = sceneView.camera.pixelHeight - screenpos.y;
        Vector2 MousePos = sceneView.camera.ScreenPointToRay(screenpos).origin;
        if (LeftDown())
        {
            //Debug.Log(MousePos);
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
}

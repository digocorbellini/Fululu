using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierPath))]
public class PathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BezierPath myTarget = (BezierPath)target;
        
        // button for making new point

        if (GUILayout.Button(new GUIContent("New vertex", "Shift + mouse0 in scene"))) {
            myTarget.AddNewVertex(myTarget.transform.position);
        }

        if (GUILayout.Button(new GUIContent("Remove last vertex", "Shift + mouse1 in scene"))) {
            myTarget.RemoveLastVertex();
        }

        if (GUILayout.Button("Reset")) {
            myTarget.ClearVerticies();
        }

        EditorGUILayout.LabelField("Vertex List Size: ", myTarget.NumOfVerticies().ToString());
    }

    private void OnSceneGUI() {
        BezierPath myTarget = (BezierPath)target;

        // listen for input
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && e.shift) {
            // place new vertex at mouse pos if shift + left mouse are pressed
            Vector3 mousePos = e.mousePosition;
            // some sort of voodoo magic I found on the internet
            float ppp = EditorGUIUtility.pixelsPerPoint;
            Camera sceneCamera = SceneView.currentDrawingSceneView.camera;
            mousePos.y = sceneCamera.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;
            Vector3 mouseWorldPos = sceneCamera.ScreenToWorldPoint(mousePos);
            myTarget.AddNewVertex(mouseWorldPos);
            //Debug.Log("spawn new vertex on mouse pos");
        } else if (e.type == EventType.MouseDown && e.button == 1 && e.shift) {
            // remove last vertex if shift + right mouse are pressed
            myTarget.RemoveLastVertex();
            //Debug.Log("remove vertex from mouse and keyboard input");
        }
    }
}

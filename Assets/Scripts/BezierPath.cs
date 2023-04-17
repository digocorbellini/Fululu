using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierPath : MonoBehaviour
{
    public int precision = 100; // # of points per bezier curve segment 
    public bool isConnected; 
    public Transform p0;
    public Transform p1;
    public Transform handle0;
    public Transform handle1;
    public GameObject handlePrefab;
    public GameObject vertexPrefab;


    [SerializeField]
    private List<GameObject> verticies = new List<GameObject>();
    [SerializeField]
    private List<Vector3> listOfPoints = new List<Vector3>();

    /// <summary>
    /// Return a point on the path that would be reached if you traveled the
    /// given distance starting at the first point. For connected paths, a full
    /// rotation is counted. For paths that are not connected, it assumes you
    /// turned around once you reached a dead end.
    /// </summary>
    /// <param name="distance">Distance traveled on the path. Must be positive!</param>
    /// <returns>A point on the path that would be reached if you traveled
    /// the given distance starting a the first point</returns>
    public Vector3 GetPosAtDistance(float distance) {
        // TODO: make this more efficient. This will not hold up for extended 
        // periods of time or for really long distances
        int index = 0;
        int direction = 1; 
        while (distance > 0) {
            if (isConnected) {
                distance -= Vector3.Distance(listOfPoints[index], listOfPoints[(index + 1) % listOfPoints.Count]);
                index = (index + 1) % listOfPoints.Count;
            } else {
                int nextIndex = index + direction;
                if (nextIndex >= listOfPoints.Count) {
                    direction = -1;
                    nextIndex = listOfPoints.Count - 1;
                } else if (nextIndex < 0) {
                    direction = 1;
                    nextIndex = 0;
                }
                distance -= Vector3.Distance(listOfPoints[index], listOfPoints[nextIndex]);
                index = nextIndex;
            }  
        }

        Vector3 curPos = listOfPoints[index];
        int prevIndex;
        if (isConnected) {
            prevIndex = (index - 1 < 0) ? listOfPoints.Count - 1 : index - 1;
        } else {
            prevIndex = index - direction;
            if (prevIndex >= listOfPoints.Count) {
                    prevIndex = listOfPoints.Count - 1;
            } else if (prevIndex < 0) {
                prevIndex = 0;
            }
        }
        Vector3 prevPos = listOfPoints[prevIndex];

        Vector3 dir = prevPos - curPos;
        dir.Normalize();
        return curPos + (dir * Mathf.Abs(distance));
    }

    /// <summary>
    /// Get a normalized vector that represents the direction of movement
    /// if you were at the point found by traveling the given distance on the 
    /// path.
    /// </summary>
    /// <param name="distance">The distance to be traveled on the path. Must be positive!</param>
    /// <returns>Returns a normalized vector that represents the direction of 
    /// movement if you were at the point found by traveling the given distance 
    /// on the path.</returns>
    public Vector3 GetDirAtDistance(float distance) {
        // TODO: make this more efficient. This will not hold up for extended 
        // periods of time or for really long distances
        int index = 0;
        int direction = 1; 
        while (distance > 0) {
            if (isConnected) {
                distance -= Vector3.Distance(listOfPoints[index], listOfPoints[(index + 1) % listOfPoints.Count]);
                index = (index + 1) % listOfPoints.Count;
            } else {
                int nextIndex = index + direction;
                if (nextIndex >= listOfPoints.Count) {
                    direction = -1;
                    nextIndex = listOfPoints.Count - 1;
                } else if (nextIndex < 0) {
                    direction = 1;
                    nextIndex = 0;
                }
                distance -= Vector3.Distance(listOfPoints[index], listOfPoints[nextIndex]);
                index = nextIndex;
            }  
        }

        Vector3 curPos = listOfPoints[index];
        int prevIndex;
        if (isConnected) {
            prevIndex = (index - 1 < 0) ? listOfPoints.Count - 1 : index - 1;
        } else {
            prevIndex = index - direction;
            if (prevIndex >= listOfPoints.Count) {
                    prevIndex = listOfPoints.Count - 1;
            } else if (prevIndex < 0) {
                prevIndex = 0;
            }
        }
        Vector3 prevPos = listOfPoints[prevIndex];

        Vector3 dir = curPos - prevPos;
        dir.Normalize();
        return dir;
    }

    public void AddNewVertex(Vector3 pos) {
        GameObject newVertex = Instantiate(vertexPrefab, pos, Quaternion.identity);
        GameObject newHandle = Instantiate(handlePrefab, pos + Vector3.up, Quaternion.identity);
        newVertex.transform.parent = transform;
        newHandle.transform.parent = newVertex.transform;
        //vertecies.Add((newVertex, newHandle));
        //exampleThing.verticies.Add((newVertex, newHandle));
        verticies.Add(newVertex);
        newVertex.name = "p" + verticies.Count + 1;
        //Debug.Log("New Vertex built");
    }

    public void RemoveLastVertex() {
        if (verticies.Count == 0)
            return;
        DestroyImmediate(verticies[verticies.Count - 1].transform.GetChild(0).gameObject);
        DestroyImmediate(verticies[verticies.Count - 1]);
        verticies.RemoveAt(verticies.Count - 1);
        //Debug.Log("Last Vertex removed");
    }

    public void ClearVerticies(){
        int numVerticies = verticies.Count;
        for (int i = 0; i < numVerticies; i++) {
            RemoveLastVertex();
        }
    }

    public int NumOfVerticies() {
        //return vertecies.Count;
        //return exampleThing.verticies.Count;
        return verticies.Count;
    }

    private Vector3 quadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        return ((1 - t) * linearBezier(p0, p1, t)) + (t * linearBezier(p1, p2, t));
    }

    private Vector3 linearBezier(Vector3 p0, Vector3 p1, float t) {
        return p0 + t * (p1 - p0);
    }

    private void OnDrawGizmos() {
        // Draw quadratic bezier curve

        listOfPoints = new List<Vector3>();
        for (int t = 1; t <= precision; t++) {
            listOfPoints.Add(quadraticBezier(p0.position, handle0.position, p1.position, t / (float)precision));
        }

        // draw line from first point to handle
        Gizmos.color = Color.green;
        Gizmos.DrawLine(p0.position, handle0.position);

        // iterate through list of verticies and connect them
        for (int i = 0; i < verticies.Count; i++) {
            Vector3 p0Vert = (i == 0) ? p1.position : verticies[i - 1].transform.position;
            Vector3 handle = (i == 0) ? handle1.position : verticies[i - 1].transform.GetChild(0).position;

            Vector3 p1Vert = verticies[i].transform.position;

            for (int t = 1; t <= precision; t++) {
                listOfPoints.Add(quadraticBezier(p0Vert, handle, p1Vert, t / (float)precision));
            }

            // connect handle
            Gizmos.color = Color.green;
            Gizmos.DrawLine(p0Vert, handle);
        }

        // close path if path is connected
        if (isConnected) {
            Vector3 lastVertex = (verticies.Count == 0) ? p1.position : verticies[verticies.Count - 1].transform.position;
            Vector3 lastHandle = (verticies.Count == 0) ? handle1.position : verticies[verticies.Count - 1].transform.GetChild(0).position;

            for (int t = 1; t <= precision; t++) {
                listOfPoints.Add(quadraticBezier(lastVertex, lastHandle, p0.position, t / (float)precision));
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(lastVertex, lastHandle);
        }

        // draw all bezier curves as a bunch of small lines
        Gizmos.color = Color.blue;
        for (int i = 0; i < listOfPoints.Count - 1; i++) {
            Gizmos.DrawLine(listOfPoints[i], listOfPoints[i + 1]);
        } 
    }

}

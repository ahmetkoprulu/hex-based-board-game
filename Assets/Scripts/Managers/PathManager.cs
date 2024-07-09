using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathLine
{
    public GameObject PathPrefab;
    public GameObject PathObject;
    public bool DisplayPath = false;
    private BezierRenderer Renderer;

    private float SmoothingLength = 0;
    private int SmoothingSections = 1;

    public PathLine(float smoothingLength = 2f, int smoothingSections = 100 * 10000000)
    {
        SmoothingLength = smoothingLength;
        SmoothingSections = smoothingSections;
    }

    public void SetPath(List<Node> points, GameObject prefab)
    {
        PathPrefab = prefab;
        if (points.Count == 0)
        {
            DestroyPath();
            return;
        }

        InstantiatePath();

        Renderer.SetPoints(points);
    }

    public List<Vector3> GetPoints()
    {
        return Renderer.GetPoints();
    }

    public void InstantiatePath()
    {
        if (PathObject != null) return;

        PathObject = Object.Instantiate(PathPrefab, Vector3.zero, Quaternion.identity);
        Renderer = PathObject.GetComponent<BezierRenderer>();
        Renderer.SmoothingLength = SmoothingLength;
        Renderer.SmoothingSections = SmoothingSections;
    }

    public void DestroyPath()
    {
        if (PathObject == null) return;

        Object.Destroy(PathObject);
        // PathObject = null;
        // Renderer = null;
    }
}

public class PathLineManager
{
    public IDictionary<string, PathLine> PathLines = new Dictionary<string, PathLine>();

    public void SetPath(string key, List<Node> path, GameObject prefab, float smoothingLength = 0f, int smoothingSections = 1)
    {

        if (!PathLines.ContainsKey(key))
        {
            PathLines.Add(key, new PathLine(smoothingLength, smoothingSections));
        }

        PathLines[key].SetPath(path, prefab);
    }

    public void DestroyPath(string key)
    {
        if (!PathLines.ContainsKey(key)) return;

        PathLines[key].DestroyPath();
        PathLines.Remove(key);
    }

    public List<Vector3> GetPoints(string key)
    {
        if (!PathLines.ContainsKey(key)) return new();

        return PathLines[key].GetPoints();
    }
}

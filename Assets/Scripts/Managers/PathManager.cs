using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager
{
    public GameObject PathPrefab;
    public GameObject PathObject;
    public bool DisplayPath = false;
    private BezierRenderer Renderer;

    public void SetPath(List<HexCell> path, GameObject prefab)
    {
        PathPrefab = prefab;
        if (path.Count == 0)
        {
            DestroyPath();
            return;
        }

        InstantiatePath();
        var points = path.Select(x => x.GetCenter() + new Vector3(0, 0.2f, 0)).ToList();
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
    }

    public void DestroyPath()
    {
        if (PathObject == null) return;

        Object.Destroy(PathObject);
        PathObject = null;
        Renderer = null;
    }
}

using System;
using System.Linq;
using UnityEngine;

public class BezierCurve
{
    public Node[] Points;

    public BezierCurve()
    {
        Points = new Node[4];
    }

    public BezierCurve(Node[] points)
    {
        Points = points;
    }

    public Vector3 StartPosition => Points[0];

    public Vector3 EndPosition => Points[^1];

    public Node GetPoint(float t)
    {
        t = Mathf.Clamp01(t);
        var time = 1 - t;
        return time * time * time * Points[0] +
               3 * time * time * t * Points[1] +
               3 * time * t * t * Points[2] +
               t * t * t * Points[3];
    }

    public Node[] GetPoints(int segments)
    {
        return Enumerable
            .Range(1, segments)
            .Select(i => GetPoint(i / (float)segments))
            .ToArray();
    }
}
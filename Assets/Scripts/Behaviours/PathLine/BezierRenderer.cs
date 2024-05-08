using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierRenderer : MonoBehaviour
{
    [field: SerializeField] public LineRenderer LineRenderer { get; set; }
    [field: SerializeField] public List<Vector3> Points { get; set; } = new();
    [field: SerializeField] public float SmoothingLength { get; set; } = 2f;
    [field: SerializeField] public int SmoothingSections { get; set; } = 100 * 10000000;

    public void SetPoints(List<Vector3> points)
    {
        LineRenderer.positionCount = 0;
        Points = points;
        if (Points.Count == 0) return;

        Smooth(points);
    }

    public List<Vector3> GetPoints()
    {
        var positions = new Vector3[LineRenderer.positionCount];
        LineRenderer.GetPositions(positions);

        var a = positions.ToList();
        return a;
    }

    public void RestoreDefault()
    {
        LineRenderer.positionCount = Points.Count;
        LineRenderer.SetPositions(Points.ToArray());
    }

    public List<BezierCurve> GenerateCurves() => GenerateCurves(Points);

    public List<BezierCurve> GenerateCurves(List<Vector3> points)
    {
        return Enumerable
            .Range(0, points.Count - 1)
            .Select(x =>
            {
                var position = points[x];
                var lastPosition = x == 0 ? points[x] : points[x - 1];
                var nextPosition = points[x + 1];

                var lastDirection = (position - lastPosition).normalized;
                var nextDirection = (nextPosition - position).normalized;

                var startTangent = (lastDirection + nextDirection) * SmoothingLength;
                var endTangent = (lastDirection + nextDirection) * -1 * SmoothingLength;

                return new BezierCurve(
                    new Vector3[] {
                        position,
                        position + startTangent,
                        nextPosition + endTangent,
                        nextPosition
                    }
                );
            }).ToList();
    }

    public List<Vector3> GenerateSmoothedPoints()
    {
        return GenerateCurves()
            .SelectMany(x => x.GetPoints(SmoothingSections))
            .ToList();
    }

    public List<Vector3> GenerateSmoothedPoints(List<Vector3> points)
    {
        return GenerateCurves(points)
            .SelectMany(x => x.GetPoints(SmoothingSections))
            .ToList();
    }

    public void Smooth()
    {
        var smoothedPoints = GenerateSmoothedPoints();
        LineRenderer.positionCount = smoothedPoints.Count;
        LineRenderer.SetPositions(smoothedPoints.ToArray());
    }

    public void Smooth(List<Vector3> points)
    {
        var smoothedPoints = GenerateSmoothedPoints(points);
        LineRenderer.positionCount = smoothedPoints.Count;
        LineRenderer.SetPositions(smoothedPoints.ToArray());
    }
}
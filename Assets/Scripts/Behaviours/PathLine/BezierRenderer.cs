using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierRenderer : MonoBehaviour
{
    [field: SerializeField] public LineRenderer LineRenderer { get; set; }
    [field: SerializeField] public List<Node> Points { get; set; } = new();
    [field: SerializeField] public float SmoothingLength { get; set; } = 2f;
    [field: SerializeField] public int SmoothingSections { get; set; } = 100 * 10000000;

    public void SetPoints(List<Node> points)
    {
        LineRenderer.positionCount = 0;
        LineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.1f), new Keyframe(1, 0.1f));
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
        LineRenderer.SetPositions(Points.Select(x => (Vector3)x).ToArray());
    }

    public List<BezierCurve> GenerateCurves() => GenerateCurves(Points);

    public List<BezierCurve> GenerateCurves(List<Node> points)
    {
        return Enumerable
            .Range(0, points.Count - 1)
            .Select(x =>
            {
                var position = points[x];
                var lastPosition = x == 0 ? points[x] : points[x - 1];
                var nextPosition = points[x + 1];

                var lastDirection = ((Vector3)(position - lastPosition)).normalized;
                var nextDirection = ((Vector3)(nextPosition - position)).normalized;

                var startTangent = (lastDirection + nextDirection) * SmoothingLength;
                var endTangent = (lastDirection + nextDirection) * -1 * SmoothingLength;

                return new BezierCurve(
                    new Node[] {
                        position,
                        position + startTangent,
                        nextPosition + endTangent,
                        nextPosition
                    }
                );
            }).ToList();
    }

    public List<Node> GenerateSmoothedPoints()
    {
        return GenerateCurves()
            .SelectMany(x => x.GetPoints(SmoothingSections))
            .ToList();
    }

    public List<Node> GenerateSmoothedPoints(List<Node> points)
    {
        return GenerateCurves(points)
            .SelectMany(x => x.GetPoints(SmoothingSections))
            .ToList();
    }

    public void Smooth()
    {
        var smoothedPoints = GenerateSmoothedPoints();
        LineRenderer.positionCount = smoothedPoints.Count;
        LineRenderer.SetPositions(smoothedPoints.Select(x => (Vector3)x).ToArray());
    }

    public void Smooth(List<Node> points)
    {
        var smoothedPoints = GenerateSmoothedPoints(points);
        LineRenderer.positionCount = smoothedPoints.Count;
        LineRenderer.SetPositions(smoothedPoints.Select(x => (Vector3)x).ToArray());
    }
}
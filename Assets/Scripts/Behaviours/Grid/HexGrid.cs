using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [field: SerializeField] public HexOrientation Orientation { get; set; }
    [field: SerializeField] public int Width { get; set; }
    [field: SerializeField] public int Height { get; set; }
    [field: SerializeField] public float HexSize { get; set; }
    private List<Vector3> Vertices { get; set; } = new();

    public HexCell[,] OffsetGrid { get; private set; }
    public HexCell[,,] AxialGrid { get; private set; }

    public void SetOffsetGrid(HexCell[,] grid) => OffsetGrid = grid;

    public void SetAxialGrid(HexCell[,,] grid) => AxialGrid = grid;

    public List<Line> GetPerimeter(List<Vector3> vertices)
    {
        var hullFinder = new HullFinder(HullType.Concave);
        hullFinder.SetPoints(vertices.Select(x =>
        {
            Handles.DotHandleCap(GUIUtility.GetControlID(FocusType.Passive), x, Quaternion.identity, 0.05f, EventType.Repaint);
            return x;
        }).ToList());
        var edges = hullFinder.CalculateHull(0.5, 2);

        // Gizmos.color = Color.blue;
        // for (int i = 0; i < edges.Count; i++)
        // {
        //     var left = new Vector3((float)edges[i].nodes[0].X, -0.2f, (float)edges[i].nodes[0].Y);
        //     var right = new Vector3((float)edges[i].nodes[1].X, -0.2f, (float)edges[i].nodes[1].Y);
        //     Gizmos.DrawLine(left, right);
        // }
        return edges;
    }

    private void OnDrawGizmos()
    {
        var vertices = new HashSet<Vector3>();
        for (int z = 0; z < Height; z++)
        {
            for (var x = 0; x < Width; x++)
            {
                var center = HexHelpers.GetCenter(HexSize, x, z, Orientation) + transform.position;
                var corners = HexHelpers.GetCorners(HexSize, Orientation);
                vertices.AddRange(corners.Select(corner => center + corner).ToArray());

                for (var c = 0; c < corners.Length; c++)
                {
                    var startPoint = (center + corners[c]).Round();
                    // Handles.Label(startPoint + Vector3.forward * 0.0f, $"[{startPoint.x}, {startPoint.z}]");
                    // s.HandleDotHandleCap(GUIUtility.GetControlID(FocusType.Passive), startPoint, Quaternion.identity, 0.05f, EventType.Repaint);
                    var next = (c + 1) % 6;
                    var endPoint = (center + corners[next]).Round();
                    Gizmos.DrawLine(startPoint, endPoint);
                }
            }
        }
    }
}

public static class VectorExtensions
{
    public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
    {
        float multiplier = 1;
        for (int i = 0; i < decimalPlaces; i++)
        {
            multiplier *= 10f;
        }
        return new Vector3(
            Mathf.Round(vector3.x * multiplier) / multiplier,
            Mathf.Round(vector3.y * multiplier) / multiplier,
            Mathf.Round(vector3.z * multiplier) / multiplier);
    }
}

public enum HexOrientation : short
{
    FlatTop,
    PointyTop
}

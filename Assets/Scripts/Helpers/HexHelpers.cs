using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

// Reference: https://www.redblobgames.com/grids/hexagons/#basics
public static class HexHelpers
{
    public static readonly Vector3[] Directions = { new(1, 0, -1), new(1, -1, 0), new(0, -1, 1), new(-1, 0, 1), new(-1, 1, 0), new(0, 1, -1) };

    public static float OuterRadius(float size) => size; // Width of the flat hexagon

    public static float InnerRadius(float size) => size * 0.866025404f; // sqrt(3) * size. Height of the flat hexagon

    public static Vector3[] GetCorners(float size, HexOrientation orientation)
    {
        var corners = new Vector3[6];
        for (var i = 0; i < 6; i++) corners[i] = GetCorner(size, i, orientation);
        return corners;
    }

    public static Vector3 GetCorner(float size, int i, HexOrientation orientation)
    {
        var angle = 60f * i;
        if (orientation == HexOrientation.PointyTop) angle += 30f;
        return new Vector3(size * Mathf.Cos(angle * Mathf.Deg2Rad), 0f, size * Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public static Vector3 GetCenter(float size, int x, int z, HexOrientation orientation)
    {
        var center = Vector3.zero;
        center.x = orientation == HexOrientation.PointyTop ? (x + z * 0.5f - z / 2) * InnerRadius(size) * 2f : x * OuterRadius(size) * 1.5f;
        center.z = orientation == HexOrientation.PointyTop ? z * OuterRadius(size) * 1.5f : (z + x * 0.5f - x / 2) * InnerRadius(size) * 2f;
        return center;
    }

    public static Vector2 CubeToAxial(Vector3 cube) => new Vector2((int)cube.x, (int)cube.y);

    public static Vector3 AxialToCube(Vector2 axial) => new Vector3(axial.x, axial.y, -axial.x - axial.y);

    public static Vector2 OffsetToAxial(int col, int row, HexOrientation orientation) => orientation == HexOrientation.PointyTop ? OffsetToAxialPointy(col, row) : OffsetToAxialFlat(col, row);

    private static Vector2 OffsetToAxialPointy(int col, int row)
    {
        var q = col - (row - (row & 1)) / 2;
        var r = row;
        return new Vector2(q, r);
    }

    private static Vector2 OffsetToAxialFlat(int col, int row)
    {
        var q = col;
        var r = row - (col - (col & 1)) / 2;
        return new Vector2(q, r);
    }

    public static Vector3 OffsetToCube(int col, int row, HexOrientation orientation) => orientation == HexOrientation.PointyTop ? AxialToCube(OffsetToAxialPointy(col, row)) : AxialToCube(OffsetToAxialFlat(col, row));

    public static Vector3 OffsetToCube(Vector2 offset, HexOrientation orientation) => OffsetToCube((int)offset.x, (int)offset.y, orientation);

    public static Vector2 CubeToOffset(Vector3 cube, HexOrientation orientation) => orientation == HexOrientation.PointyTop ? CubeToOffsetPointy((int)cube.x, (int)cube.y, (int)cube.z) : CubeToOffsetFlat((int)cube.x, (int)cube.y, (int)cube.z);

    public static Vector2 CubeToOffsetPointy(int x, int y, int z) => new Vector2(x + (y - (y & 1)) / 2, y);

    public static Vector2 CubeToOffsetFlat(int x, int y, int z) => new Vector2(x, y + (x - (x & 1)) / 2);

    public static Vector3 CubeRound(Vector3 frac)
    {
        var rx = Mathf.RoundToInt(frac.x);
        var ry = Mathf.RoundToInt(frac.y);
        var rz = Mathf.RoundToInt(frac.z);

        var xDiff = Mathf.Abs(rx - frac.x);
        var yDiff = Mathf.Abs(ry - frac.y);
        var zDiff = Mathf.Abs(rz - frac.z);

        if (xDiff > yDiff && xDiff > zDiff) rx = -ry - rz;
        else if (yDiff > zDiff) ry = -rx - rz;
        else rz = -rx - ry;

        return new Vector3(rx, ry, rz);
    }

    public static Vector2 AxialRound(Vector2 coordinates) => CubeToAxial(CubeRound(AxialToCube(coordinates)));

    public static Vector2 CoordinateToAxial(float x, float z, float hexSize, HexOrientation orientation) => orientation == HexOrientation.PointyTop ? CoordinateToPointyAxial(x, z, hexSize) : CoordinateToFlatAxial(x, z, hexSize);

    public static Vector2 CoordinateToPointyAxial(float x, float z, float hexSize)
    {
        var pointyHexCoordinates = Vector2.zero;
        pointyHexCoordinates.x = (Mathf.Sqrt(3) / 3f * x - 1f / 3f * z) / hexSize;
        pointyHexCoordinates.y = 2f / 3f * z / hexSize;
        return AxialRound(pointyHexCoordinates);
    }

    public static Vector2 CoordinateToFlatAxial(float x, float z, float hexSize)
    {
        var flatHexCoordinates = Vector2.zero;
        flatHexCoordinates.x = 2f / 3 * x / hexSize;
        flatHexCoordinates.y = (-1f / 3 * x + Mathf.Sqrt(3) / 3 * z) / hexSize;
        return AxialRound(flatHexCoordinates);
    }

    public static Vector2 CoordinateToOffset(float x, float z, float hexSize, HexOrientation orientation) => CubeToOffset(AxialToCube(CoordinateToAxial(x, z, hexSize, orientation)), orientation);

    public static List<Vector2> GetNeighboursCoordinates(int x, int y, HexOrientation orientation)
    {
        var neighbours = new List<Vector2>();
        var cubeCoordinates = OffsetToCube(x, y, orientation);

        return Enumerable.Range(0, 6)
            .Select(i => cubeCoordinates + Directions[i])
            .Select(x => CubeToOffset(x, orientation))
            .ToList();
    }

    public static bool IsExceedingGrid(int x, int y, int width, int height) => x < 0 || y < 0 || x >= width || y >= height;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell
{
    [Header("Cell Properties")]
    [SerializeField] private HexOrientation orientation;
    [field: SerializeField] private Transform terrain;
    [field: SerializeField] public HexGrid Grid { get; set; }
    [field: SerializeField] public float Size { get; set; }
    [field: SerializeField] public TerrainType TerrainType { get; set; }
    [field: SerializeField] public Vector2 OffsetCoordinates { get; set; }
    [field: SerializeField] public Vector3 CubeCoordinates { get; set; }
    [field: SerializeField] public Vector2 AxialCoordinates { get; set; }
    [field: SerializeField] public List<HexCell> Neighbours { get; set; }
}

// public enum TerrainType
// {
//     Grass,
//     Water,
//     Mountain,
//     Desert,
//     Forest,
//     Snow
// }

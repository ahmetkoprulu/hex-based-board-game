using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    public void OnSceneGUI()
    {
        var hexGrid = (HexGrid)target;
        for (int z = 0; z < hexGrid.Height; z++)
        {
            for (var x = 0; x < hexGrid.Width; x++)
            {
                var center = HexHelpers.GetCenter(hexGrid.HexSize, x, z, hexGrid.Orientation) + hexGrid.transform.position;
                var cubeCoord = HexHelpers.OffsetToCube(x, z, hexGrid.Orientation);
                Handles.Label(center + Vector3.forward * 0.5f, $"[{x}, {z}]");
                Handles.Label(center, $"[{cubeCoord.x}, {cubeCoord.y}, {cubeCoord.z}]");
            }
        }
    }
}

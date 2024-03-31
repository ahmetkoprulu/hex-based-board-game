using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGridMeshGenerator))]
public class HexGridMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var meshGenerator = (HexGridMeshGenerator)target;

        if (GUILayout.Button("Generate Hex Mesh"))
            meshGenerator.CreateHexMesh();

        if (GUILayout.Button("Clear Hex Mesh"))
            meshGenerator.ClearHexGridMesh();
    }
}

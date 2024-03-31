using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexCellGenerator))]
public class HexCellGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var mapGenerator = (HexCellGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            mapGenerator.Generate();
        }
    }
}
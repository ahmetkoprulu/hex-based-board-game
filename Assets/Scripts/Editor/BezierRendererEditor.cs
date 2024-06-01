using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierRenderer))]
public class BezierRendererEditor : Editor
{
    private BezierRenderer Smoother { get; set; }
    private bool ExpandCurves { get; set; } = false;
    private List<Vector3> SmoothedPoints { get; set; }

    public bool HideGuideLines { get; set; } = false;

    private void OnEnable()
    {
        Smoother = (BezierRenderer)target;
        if (Smoother.LineRenderer == null) Smoother.LineRenderer = Smoother.GetComponent<LineRenderer>();
    }

    public override void OnInspectorGUI()
    {
        if (Smoother == null) return;

        DrawDefaultInspector();

        // Create a checkbox
        if (EditorGUILayout.Toggle("Hide Guide Lines", HideGuideLines)) HideGuideLines = true;
        else HideGuideLines = false;

        if (GUILayout.Button("Set Points"))
        {
            Smoother.LineRenderer.positionCount = Smoother.Points.Count;
            Smoother.LineRenderer.SetPositions(Smoother.Points.Select(x => (Vector3)x).ToArray());
        }

        if (GUILayout.Button("Smooth"))
        {
            Smooth();
        }

        if (GUILayout.Button("Restore Default"))
        {
            Smoother.LineRenderer.positionCount = Smoother.Points.Count;
            Smoother.LineRenderer.SetPositions(Smoother.Points.Select(x => (Vector3)x).ToArray());
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (Smoother.Points.Count < 3 || HideGuideLines) return;

        var c = Smoother.GenerateCurves();
        DrawSegments(c);
    }

    private void DrawSegments(List<BezierCurve> curves)
    {
        SmoothedPoints = Smoother.GenerateSmoothedPoints().Select(x => (Vector3)x).ToList();
        for (var i = 0; i < SmoothedPoints.Count - 1; i++)
        {
            Handles.color = Color.white;
            float color = (float)i / SmoothedPoints.Count;
            Handles.color = new Color(color, color, color);
            Handles.Label(SmoothedPoints[i], $"-----{i}");
            Handles.DrawLine(SmoothedPoints[i], SmoothedPoints[i + 1]);
            Handles.DotHandleCap(GUIUtility.GetControlID(FocusType.Passive), SmoothedPoints[i], Quaternion.identity, 0.05f, EventType.Repaint);
        }
    }

    public void Smooth()
    {
        Smoother.LineRenderer.positionCount = SmoothedPoints.Count;
        Smoother.LineRenderer.SetPositions(SmoothedPoints.ToArray());
    }
}
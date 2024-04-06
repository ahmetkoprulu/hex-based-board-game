using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [field: SerializeField] public HexOrientation Orientation { get; set; }
    [field: SerializeField] public int Width { get; set; }
    [field: SerializeField] public int Height { get; set; }
    [field: SerializeField] public float HexSize { get; set; }

    public HexCell[,] Cells { get; private set; }

    public void SetCells(HexCell[,] cells) => Cells = cells;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        MouseController.Instance.OnLeftMouseClick += OnLeftMouseClick;
        MouseController.Instance.OnRightMouseClick += OnRightMouseClick;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        MouseController.Instance.OnLeftMouseClick -= OnLeftMouseClick;
        MouseController.Instance.OnRightMouseClick -= OnRightMouseClick;
    }

    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    private void OnDrawGizmos()
    {
        for (int z = 0; z < Height; z++)
        {
            for (var x = 0; x < Width; x++)
            {
                var center = HexHelpers.GetCenter(HexSize, x, z, Orientation) + transform.position;
                var corners = HexHelpers.GetCorners(HexSize, Orientation);

                for (var c = 0; c < corners.Length; c++)
                {
                    var startPoint = center + corners[c];
                    var endPoint = center + corners[(c + 1) % 6];
                    Gizmos.DrawLine(startPoint, endPoint);
                }
            }
        }
    }

    private void OnLeftMouseClick(RaycastHit hit)
    {
        float localX = hit.point.x - transform.position.x;
        float localZ = hit.point.z - transform.position.z;
        Debug.Log($"Local X: {localX}, Local Z: {localZ}");
    }

    private void OnRightMouseClick(RaycastHit hit)
    {
        float localX = hit.point.x - transform.position.x;
        float localZ = hit.point.z - transform.position.z;

        Vector2 localtion = HexHelpers.CoordinateToOffset(localX, localZ, HexSize, Orientation);
        var center = HexHelpers.GetCenter(HexSize, (int)localtion.x, (int)localtion.y, Orientation);
        Debug.Log($"Localtion: {localtion}, Center: {center}");
        // Instantiate(explosionTest, center, Quaternion.identity);
    }
}

public enum HexOrientation : short
{
    FlatTop,
    PointyTop
}

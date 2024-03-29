using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshCollider))]
public class HexGridMeshGenerator : MonoBehaviour
{
    [field: SerializeField] public LayerMask GridLayer { get; set; }
    [field: SerializeField] public HexGrid HexGrid { get; set; }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (HexGrid == null) HexGrid = GetComponent<HexGrid>();
        if (HexGrid == null) Debug.LogError("HexGridMeshGenerator could not find a HexGrid component in its parent or itself.");
    }

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

    public void CreateHexMesh()
    {
        CreateHexMesh(HexGrid.Width, HexGrid.Height, HexGrid.HexSize, HexGrid.Orientation, GridLayer);
    }

    public void CreateHesMesh(HexGrid hexGrid, LayerMask layerMask)
    {
        HexGrid = hexGrid;
        GridLayer = layerMask;

        CreateHexMesh(HexGrid.Width, HexGrid.Height, HexGrid.HexSize, HexGrid.Orientation, GridLayer);
    }

    public void CreateHexMesh(int width, int height, float hexSize, HexOrientation orientation, LayerMask layerMask)
    {
        ClearHexGridMesh();
        var vertices = new Vector3[7 * width * height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var center = HexHelpers.GetCenter(hexSize, x, z, orientation);
                vertices[(z * width + x) * 7] = center;

                var corners = HexHelpers.GetCorners(hexSize, orientation);
                for (int c = 0; c < corners.Length; c++)
                    vertices[(z * width + x) * 7 + c + 1] = center + corners[c % 6];
            }
        }

        var triangles = new int[3 * 6 * width * height];
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                var corners = HexHelpers.GetCorners(hexSize, orientation);
                for (int c = 0; c < corners.Length; c++)
                {
                    var i = c + 2 > 6 ? c + 2 - 6 : c + 2;
                    triangles[3 * 6 * (z * width + x) + c * 3 + 0] = (z * width + x) * 7;
                    triangles[3 * 6 * (z * width + x) + c * 3 + 1] = (z * width + x) * 7 + c + 1;
                    triangles[3 * 6 * (z * width + x) + c * 3 + 2] = (z * width + x) * 7 + i;
                }
            }
        }

        var mesh = new Mesh { vertices = vertices, triangles = triangles };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        mesh.RecalculateUVDistributionMetrics();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        var gridLayerIndex = GetGridLayer(layerMask);
        Debug.Log($"Layer Index: {gridLayerIndex}");
        gameObject.layer = gridLayerIndex;
    }

    public void ClearHexGridMesh()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null) return;

        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshCollider>().sharedMesh.Clear();
    }

    private int GetGridLayer(LayerMask layerMask)
    {
        var layerMaskValue = layerMask.value;
        Debug.Log($"Layer Mask Value: {layerMaskValue}");

        for (var i = 0; i < 32; i++)
        {
            if ((layerMaskValue & 1 << i) != 0) return i;
        }

        return 0;
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

        Vector2 localtion = HexHelpers.CoordinateToOffset(localX, localZ, HexGrid.HexSize, HexGrid.Orientation);
        var center = HexHelpers.GetCenter(HexGrid.HexSize, (int)localtion.x, (int)localtion.y, HexGrid.Orientation);
        Debug.Log($"Localtion: {localtion}, Center: {center}");
        // Instantiate(explosionTest, center, Quaternion.identity);
    }
}

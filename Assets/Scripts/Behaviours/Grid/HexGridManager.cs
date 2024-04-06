using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    [field: SerializeField] public HexGrid Grid { get; set; }
    private HexCell SelectedCell { get; set; }

    void OnEnable()
    {
        MouseController.Instance.OnLeftMouseClick += OnLeftMouseClick;
        MouseController.Instance.OnRightMouseClick += OnRightMouseClick;
    }

    void OnDisable()
    {
        MouseController.Instance.OnLeftMouseClick -= OnLeftMouseClick;
        MouseController.Instance.OnRightMouseClick -= OnRightMouseClick;
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
        Vector2 localtion = HexHelpers.CoordinateToOffset(localX, localZ, Grid.HexSize, Grid.Orientation);
        var center = HexHelpers.GetCenter(Grid.HexSize, (int)localtion.x, (int)localtion.y, Grid.Orientation);
        SelectCell((int)localtion.x, (int)localtion.y);
    }

    private void SelectCell(int x, int y)
    {
        var cell = Grid.OffsetGrid[x, y];
        SelectedCell?.OnDeSelected();

        if (SelectedCell?.Equals(cell) ?? false)
        {
            SelectedCell = null;
            return;
        }

        SelectedCell = cell;
        SelectedCell.OnSelected();
    }
}
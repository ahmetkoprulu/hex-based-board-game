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

        if (cell.TerrainType.IsNotMoveable)
        {
            SelectedCell = null;
            return;
        };

        var available = FindReachableCoordinates(cell, 3, Grid.OffsetGrid.Cast<HexCell>().ToList());

        // var range = HexHelpers.GetCoordinateRange(cell.CubeCoordinates, 2)
        //     .Select(c => HexHelpers.CubeToOffset(c, Grid.Orientation))
        //     .Where(c => !HexHelpers.IsExceedingGrid((int)c.x, (int)c.y, Grid.Width, Grid.Height))
        //     .Select(x => Grid.OffsetGrid[(int)x.x, (int)x.y])
        //     .ToList();

        SelectedCell = cell;
        cell.SetReachableNeighbours(available);
        SelectedCell.OnSelected();
    }

    public List<HexCell> FindReachableCoordinates(HexCell center, int steps, List<HexCell> range)
    {
        var blocked = range.Where(x => x.TerrainType.IsNotMoveable).ToList();
        return BreadthFirstSearch(center, steps, blocked);
    }

    public List<HexCell> BreadthFirstSearch(HexCell origin, int steps, List<HexCell> blocked)
    {
        var results = new List<HexCell> { origin };
        var fringes = new List<List<HexCell>>() { new List<HexCell> { origin } };

        for (var k = 1; k < steps; k++)
        {
            fringes.Add(new List<HexCell>());
            foreach (var coord in fringes[k - 1])
            {
                for (var direction = 0; direction < 6; direction++)
                {
                    var neighbour = coord.GetNeighbour(HexHelpers.Directions[direction]);
                    if (!(blocked.Contains(neighbour) || results.Contains(neighbour)))
                    {
                        results.Add(neighbour);
                        fringes[k].Add(neighbour);
                    }
                }
            }
        }

        return results;
    }

}
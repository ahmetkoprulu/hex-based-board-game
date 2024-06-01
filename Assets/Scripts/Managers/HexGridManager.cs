using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    [field: SerializeField] public HexGrid Grid { get; set; }
    [field: SerializeField] public GameObject PathPrefab { get; set; }
    [field: SerializeField] public GameObject RangePrefab { get; set; }

    private HexCell SelectedCell { get; set; }
    private List<HexCell> ReachableCells { get; set; } = new();
    private List<HexCell> RangeCells { get; set; } = new();
    private PathLineManager PathManager { get; set; } = new();

    void OnEnable()
    {
        MouseController.Instance.OnLeftMouseClick += OnLeftMouseClick;
        MouseController.Instance.OnRightMouseClick += OnRightMouseClick;
        MouseController.Instance.OnHover += OnHover;
    }

    void OnDisable()
    {
        MouseController.Instance.OnLeftMouseClick -= OnLeftMouseClick;
        MouseController.Instance.OnRightMouseClick -= OnRightMouseClick;
        MouseController.Instance.OnHover -= OnHover;
    }

    private void SetReachableCells(List<HexCell> cells)
    {
        ReachableCells?.ForEach(x => x.OnBlur());
        cells?.ForEach(x => x.OnHighlighted());
        ReachableCells = cells;
    }

    private void SetRangeCells(List<HexCell> cells)
    {
        // RangeCells?.ForEach(x => x.OnBlur());
        // cells?.ForEach(x => x.OnHighlighted());
        PathManager.DestroyPath("Range");

        RangeCells = cells;
        var c = RangeCells.Select(x => x.GetLines()).SelectMany(x => x).ToList();

        var uniqueLines = c.GroupBy(l => l, new LineEqualityComparer())
                        .ToList();
        var count = uniqueLines.Select(x => x.Count()).ToList();
        var unique = uniqueLines.Where(x => x.Count() == 1).SelectMany(x => x).ToList();
        var ul = Line.GetContiniousLines(unique);
        var a = Line.GetContiniousNodes(ul);

        foreach (var l in ul)
        {
            Debug.DrawLine(l.nodes[0], l.nodes[1], Color.magenta, 5f);
        }

        PathManager.SetPath("Range", a, RangePrefab);
    }

    private void SetPath(List<HexCell> path)
    {
        var points = path.Select(x =>
        {
            var c = x.GetCenter();
            return new Node(c);
        }).ToList();
        PathManager.SetPath("Movement", points, PathPrefab, 0.5f, 10);
    }

    private void OnLeftMouseClick(RaycastHit hit)
    {
        if (SelectedCell == null) return;
        var cell = GetCell(hit);
        if (cell.TerrainType.IsNotMoveable) return;

        if (cell.Unit != null)
        {
            if (!RangeCells.Any(x => x.OffsetCoordinates == cell.OffsetCoordinates)) return;

            SelectedCell.Unit.AttackTarget(cell.Unit);
            return;
        }

        if (!ReachableCells.Any(x => x.OffsetCoordinates == cell.OffsetCoordinates)) return;

        MoveUnit(SelectedCell, cell);
        SelectedCell?.OnBlur();
        SelectedCell = null;
        SetReachableCells(new());
        SetRangeCells(new());
    }

    private void OnRightMouseClick(RaycastHit hit)
    {
        float localX = hit.point.x - 1;
        float localZ = hit.point.z - 1;
        Vector2 localtion = HexHelpers.CoordinateToOffset(localX, localZ, Grid.HexSize, Grid.Orientation);
        var center = HexHelpers.GetCenter(Grid.HexSize, (int)localtion.x, (int)localtion.y, Grid.Orientation);
        SelectCell((int)localtion.x, (int)localtion.y);
    }

    private void OnHover(RaycastHit hit)
    {
        float localX = hit.point.x - 1;
        float localZ = hit.point.z - 1;
        Debug.Log($"Hovering over {localX}, {localZ}");
        Vector2 localtion = HexHelpers.CoordinateToOffset(localX, localZ, Grid.HexSize, Grid.Orientation);

        var cell = ReachableCells.FirstOrDefault(x => x.OffsetCoordinates == localtion);
        if (cell?.terrain == null || cell.OffsetCoordinates == SelectedCell.OffsetCoordinates)
        {
            SetPath(new());
            return;
        }

        var pathFinder = new AStarPathFinder();
        var path = pathFinder.FindPath(SelectedCell, cell);
        SetPath(path);
    }

    private void SelectCell(int x, int y)
    {
        var cell = Grid.OffsetGrid[x, y];
        SelectedCell?.OnBlur();

        if (SelectedCell?.Equals(cell) ?? false)
        {
            SelectedCell = null;
            SetReachableCells(new());
            SetRangeCells(new());
            return;
        }

        if (cell.Unit == null)
        {
            SelectedCell = null;
            SetReachableCells(new());
            SetRangeCells(new());
            return;
        }

        if (cell.TerrainType.IsNotMoveable)
        {
            SelectedCell = null;
            SetReachableCells(new());
            SetRangeCells(new());
            return;
        };

        SetReachableCells(FindReachableCoordinates(cell, cell.Unit?.Movement + 1 ?? 3, Grid.OffsetGrid.Cast<HexCell>().ToList()));
        SetRangeCells(FindReachableCoordinates(cell, cell.Unit?.Range + 1 ?? 3, Grid.OffsetGrid.Cast<HexCell>().ToList()));

        SelectedCell = cell;
        SelectedCell.OnSelected();
    }

    public void HighlightUnmoveableCells()
    {
        foreach (var cell in Grid.OffsetGrid)
        {
            if (cell.TerrainType.IsNotMoveable)
            {
                cell.OnForbidden();
            }
        }
    }

    public void BlurUnmoveableCells()
    {
        foreach (var cell in Grid.OffsetGrid)
        {
            if (cell.TerrainType.IsNotMoveable)
            {
                cell.OnBlur();
            }
        }
    }

    public void MoveUnit(HexCell from, HexCell to)
    {
        if (to.TerrainType.IsNotMoveable) return;

        if (from.Unit == null) return;

        StartCoroutine(from.Unit.Move(PathManager.GetPoints("Movement")));
        to.SetUnit(from.Unit);
        from.SetUnit((HexUnit)default);
    }

    public HexCell GetCell(RaycastHit hit)
    {
        float localX = hit.point.x - 1;
        float localZ = hit.point.z - 1;
        Vector2 localtion = HexHelpers.CoordinateToOffset(localX, localZ, Grid.HexSize, Grid.Orientation);
        var center = HexHelpers.GetCenter(Grid.HexSize, (int)localtion.x, (int)localtion.y, Grid.Orientation) + transform.position;
        Debug.Log($"Center: {center.x}, {center.y}");
        var cell = Grid.OffsetGrid[(int)localtion.x, (int)localtion.y];

        return cell;
    }

    public List<HexCell> FindReachableCoordinates(HexCell center, int steps, List<HexCell> range)
    {
        var blocked = range.Where(x => x.TerrainType.IsNotMoveable).ToList();
        return BreadthFirstSearch(center, steps, blocked);
    }

    public List<HexCell> FindRangeCoordinates(HexCell center, int steps, List<HexCell> range)
    {
        var blocked = range.Where(x => !x.TerrainType.IsBulletProof).ToList();
        return BreadthFirstSearch(center, steps, new List<HexCell>());
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
                    if (neighbour == null) continue;
                    if (!(blocked.Any(x => x.CubeCoordinates == neighbour.CubeCoordinates) || results.Any(x => x.CubeCoordinates == neighbour.CubeCoordinates)))
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

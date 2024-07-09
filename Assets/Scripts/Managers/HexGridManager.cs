using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        // ReachableCells?.ForEach(x => x.OnBlur());
        // cells?.ForEach(x => x.OnHighlighted());
        ReachableCells = cells;
    }

    private void SetRangeCells(List<HexCell> cells)
    {
        // RangeCells?.ForEach(x => x.OnBlur());
        // cells?.ForEach(x => x.OnHighlighted());
        RangeCells = cells;

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

    private void CalcDda(Vector3 start, Vector3 end, out bool blocked)
    {
        blocked = false;
        float dx = end.x - start.x, dy = end.z - start.z;
        var steps = Math.Abs(dx) * 2 > Math.Abs(dy) * 2 ? Math.Abs(dx) * 2 : Math.Abs(dy) * 2;
        float xIncrement = dx / steps, yIncrement = dy / steps;
        float x = start.x, y = start.z;

        for (var k = 0; k < steps; k++)
        {
            x += xIncrement;
            y += yIncrement;
            Debug.DrawCircle(new Vector3(x, 0, y), 0.2f, 16, Color.red, 1);
            var cell = GetCell((int)x, (int)y);

            if (cell.TerrainType.IsBulletProof)
            {
                blocked = true;
                var center = cell.GetCenter();
                Debug.DrawCircle(new Vector3(center.x, 0, center.z), 1f, 16, Color.blue, 1);

                Debug.Log($"Bulletproof: {cell.OffsetCoordinates}");
                // tileFound = true;
                Debug.DrawCircle(new Vector3(x, 0, y), 0.2f, 16, Color.blue, 1);
                break;
            }
        }
    }

    // Check if two there are two vertices that are closest to each other and return center of the vertices. 
    private (Vector3 start, Vector3 end) GetClosestVertices(Vector3[] origins, Vector3[] targets)
    {
        LineOfSightDistance secondMinDistance = null;
        LineOfSightDistance minDistance = null;

        foreach (var origin in origins)
        {
            foreach (var target in targets)
            {
                if (origin == target)
                {
                }

                var distance = Vector3.Distance(origin, target);
                distance = distance.TruncateFloat(4);
                if (distance == 0)
                {
                }
                if (minDistance == null || distance < minDistance.Value)
                {
                    minDistance = new LineOfSightDistance { Start = origin, End = target, Value = distance };
                    secondMinDistance = null;
                    continue;
                }

                if (distance == minDistance.Value && target != minDistance.End)
                {
                    secondMinDistance = new LineOfSightDistance { Start = origin, End = target, Value = distance };
                }
            }
        }

        if (secondMinDistance != null)
        {
            var start = (secondMinDistance.Start + minDistance.Start) / 2;
            var end = (secondMinDistance.End + minDistance.End) / 2;

            return (start, end);
        }

        return (minDistance.Start, minDistance.End);
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
        var range = FindRangeCoordinates(cell, cell.Unit?.Range + 1 ?? 3, Grid.OffsetGrid.Cast<HexCell>().ToList());
        range = range.Where(x =>
        {
            var (start, end) = GetClosestVertices(cell.GetVertices(), x.GetVertices());
            Debug.DrawLine(start, end, Color.blue, 1);
            CalcDda(start, end, out var isBlocked);

            Debug.DrawCircle(start, 0.2f, 16, Color.green, 1);
            Debug.DrawCircle(end, 0.2f, 16, Color.green, 1);

            return !isBlocked;
        }).ToList();

        SetRangeCells(range);
        SelectedCell = cell;
        SelectedCell.OnSelected();

        // foreach (var item in SelectedCell.GetVertices())
        // {
        //     Debug.DrawCircle(item, 0.2f, 16, Color.red, 10);
        // }
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
        var cell = Grid.OffsetGrid[(int)localtion.x, (int)localtion.y];

        return cell;
    }

    public HexCell GetCell(int x, int y)
    {
        float localX = x - 1;
        float localZ = y - 1;
        Vector2 localtion = HexHelpers.CoordinateToOffset(localX, localZ, Grid.HexSize, Grid.Orientation);
        var center = HexHelpers.GetCenter(Grid.HexSize, (int)localtion.x, (int)localtion.y, Grid.Orientation) + transform.position;
        var cell = Grid.OffsetGrid[(int)localtion.x, (int)localtion.y];
        center = cell.GetCenter();
        Debug.DrawCircle(new Vector3(center.x, 0, center.z), 0.2f, 16, Color.yellow, 1);

        return cell;
    }

    public List<HexCell> FindReachableCoordinates(HexCell center, int steps, List<HexCell> range)
    {
        var blocked = range.Where(x => x.TerrainType.IsNotMoveable).ToList();
        return BreadthFirstSearch(center, steps, blocked);
    }

    public List<HexCell> FindRangeCoordinates(HexCell center, int steps, List<HexCell> range)
    {
        // TODO: Also block back of bulletproof cells.
        var blocked = new List<HexCell>();
        if (steps < 3) blocked = range.Where(x => x.TerrainType.IsBulletProof || x.TerrainType.IsNotMoveable).ToList();
        else blocked = range.Where(x => x.TerrainType.IsBulletProof).ToList();
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

    class LineOfSightDistance : IComparable<LineOfSightDistance>
    {
        public Vector3 Start { get; set; }
        public Vector3 End { get; set; }
        public double Value { get; set; }

        public int CompareTo(LineOfSightDistance other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}

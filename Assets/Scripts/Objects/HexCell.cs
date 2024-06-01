using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexCell
{
    [Header("Terrain Properties")]
    [SerializeField] private HexOrientation orientation;
    public Transform terrain { get; private set; }
    [field: SerializeField] public HexGrid Grid { get; set; }
    [field: SerializeField] public float Size { get; set; }
    public TerrainType TerrainType { get; set; }

    // Cordinates
    public Vector2 OffsetCoordinates { get; set; }
    public Vector3 CubeCoordinates { get; set; }
    public Vector2 AxialCoordinates { get; set; }

    // Range - Neighbours - Pathfinding
    public List<HexCell> Neighbours { get; set; } = new();
    public List<HexCell> MoveableNeighbours => Neighbours.Where(x => !x.TerrainType.IsNotMoveable).ToList();
    public List<HexCell> ReachableNeighbours { get; set; } = new();

    // Unit
    public HexUnit Unit { get; private set; }
    public bool IsOccupied => Unit != null;

    private Material ForbiddenMaterial { get; set; }
    private Material HighlightedMaterial { get; set; }

    public HexCell()
    {
        ForbiddenMaterial = Object.Instantiate(Resources.Load<Material>(@"Materials/ForbiddenMask"));
        HighlightedMaterial = Object.Instantiate(Resources.Load<Material>(@"Materials/HighlightMask"));

        ForbiddenMaterial.name = "Forbidden (Instance)";
        HighlightedMaterial.name = "Highlighted (Instance)";
    }

    public HexCell SetCoordinates(Vector2 offsetCoordinates, HexOrientation orientation)
    {
        this.orientation = orientation;
        OffsetCoordinates = offsetCoordinates;
        CubeCoordinates = HexHelpers.OffsetToCube(offsetCoordinates, orientation);
        AxialCoordinates = HexHelpers.CubeToAxial(CubeCoordinates);

        return this;
    }

    public HexCell SetTerrainType(TerrainType terrainType)
    {
        TerrainType = terrainType;
        return this;
    }

    public HexCell SetUnit(UnitType type)
    {
        Unit = HexUnit.Create(type, this);
        return this;
    }

    public HexCell SetUnit(HexUnit unit)
    {
        Unit = unit;
        return this;
    }

    public void SetNeighbours(List<HexCell> neighbours) => Neighbours = neighbours;

    public HexCell GetNeighbour(Vector3 direction)
    {
        var coord = CubeCoordinates + direction;
        var neighbour = Neighbours.FirstOrDefault(x => x.CubeCoordinates == coord);
        return neighbour;
    }

    public Vector3 GetCenter()
    {
        return HexHelpers.GetCenter(
            Size,
            (int)OffsetCoordinates.x,
            (int)OffsetCoordinates.y, orientation
        ) + Grid.transform.position;
    }

    public Vector3[] GetVertices(Vector3[] corners = null)
    {
        corners ??= HexHelpers.GetCorners(Grid.HexSize, Grid.Orientation);
        var center = GetCenter();
        return corners.Select(corner => center + corner).ToArray();
    }

    public IEnumerable<Line> GetLines()
    {
        var center = GetCenter();
        var corners = HexHelpers.GetCorners(Grid.HexSize, Grid.Orientation);
        return Enumerable
            .Range(0, corners.Length)
            .Select(x =>
            {
                var start = new Node((center + corners[x]).Round());
                var end = new Node((center + corners[(x + 1) % 6]).Round());

                var max = Node.Max(start, end);
                var min = Node.Min(start, end);

                return new Line(max, min);
            });
    }

    public void CreateTerrain()
    {
        Vector3 centrePosition = HexHelpers.GetCenter(
            Size,
            (int)OffsetCoordinates.x,
            (int)OffsetCoordinates.y, orientation
            ) + Grid.transform.position;

        terrain = Object.Instantiate(
            TerrainType.Prefab,
            centrePosition,
            Quaternion.identity,
            Grid.transform
            );
        // terrain.gameObject.layer = LayerMask.NameToLayer("Grid");

        //TODO: Adjust the size of the prefab to the size of the grid cell

        if (orientation == HexOrientation.PointyTop)
        {
            terrain.Rotate(new Vector3(0, 30, 0));
        }

        int randomRotation = Random.Range(0, 6);
        terrain.Rotate(new Vector3(0, randomRotation * 60, 0));
    }

    public void ClearTerrain()
    {
        if (terrain != null)
        {
            Terrain hexTerrrain = terrain.GetComponent<Terrain>();
            // hexTerrrain.OnMouseEnterAction -= OnMouseEnter;
            // hexTerrrain.OnMouseExitAction -= OnMouseExit;
            Object.Destroy(terrain.gameObject);
            Object.Destroy(ForbiddenMaterial);
            Object.Destroy(HighlightedMaterial);
        }
    }

    public static HexCell Create(HexGrid grid, float size)
    {
        var cell = new HexCell
        {
            Grid = grid,
            Size = size,
        };

        return cell;
    }

    public void OnSelected()
    {
        var renderer = terrain.GetComponent<MeshRenderer>();

        // renderer.materials[2].SetInt("_Active", 1);
        // renderer.materials[1].SetColor("_Color", Color.blue);
    }

    public void OnHighlighted(Color color = default)
    {
        var renderer = terrain.GetComponent<Renderer>();
        if (renderer.sharedMaterials.Any(x => x.name == HighlightedMaterial.name)) return;

        var materials = renderer.sharedMaterials.ToList();
        materials.Add(HighlightedMaterial);

        renderer.materials = materials.ToArray();
    }

    public void OnForbidden()
    {
        var renderer = terrain.GetComponent<Renderer>();
        if (renderer.materials.Any(x => x.name == ForbiddenMaterial.name)) return;

        var materials = renderer.sharedMaterials.ToList();
        materials.Add(ForbiddenMaterial);

        renderer.materials = materials.ToArray();
    }

    public void OnHovered()
    {
        // var renderer = terrain.GetComponent<Renderer>();
        // renderer.materials[2].SetInt("_Active", 1);
    }

    public void OnBlur()
    {
        var renderer = terrain.GetComponent<Renderer>();
        var materials = renderer.sharedMaterials.ToList();

        materials.Remove(ForbiddenMaterial);
        materials.Remove(HighlightedMaterial);

        renderer.materials = materials.ToArray();
    }

    public bool Equals(HexCell other)
    {
        return OffsetCoordinates == other.OffsetCoordinates || CubeCoordinates == other.CubeCoordinates || AxialCoordinates == other.AxialCoordinates;
    }
}

// public enum TerrainType
// {
//     Grass,
//     Water,
//     Mountain,
//     Desert,
//     Forest,
//     Snow
// }

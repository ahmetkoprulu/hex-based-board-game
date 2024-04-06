using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell
{
    [Header("Cell Properties")]
    [SerializeField] private HexOrientation orientation;
    [field: SerializeField] private Transform terrain;
    [field: SerializeField] public HexGrid Grid { get; set; }
    [field: SerializeField] public float Size { get; set; }
    [field: SerializeField] public TerrainType TerrainType { get; set; }
    [field: SerializeField] public Vector2 OffsetCoordinates { get; set; }
    [field: SerializeField] public Vector3 CubeCoordinates { get; set; }
    [field: SerializeField] public Vector2 AxialCoordinates { get; set; }
    [field: SerializeField] public List<HexCell> Neighbours { get; set; } = new();

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

    public void SetNeighbours(List<HexCell> neighbours) => Neighbours = neighbours;

    public void ClearTerrain()
    {
        if (terrain != null)
        {
            Terrain hexTerrrain = terrain.GetComponent<Terrain>();
            // hexTerrrain.OnMouseEnterAction -= OnMouseEnter;
            // hexTerrrain.OnMouseExitAction -= OnMouseExit;
            Object.Destroy(terrain.gameObject);
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
        terrain.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        var renderer = terrain.GetComponent<Renderer>();
        renderer.materials[1].SetFloat("_Scale", 1.1f);
        renderer.materials[1].SetColor("_Color", Color.blue);

        Neighbours.ForEach(x =>
        {
            x.terrain.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
            var renderer = x.terrain.GetComponent<Renderer>();
            renderer.materials[1].SetFloat("_Scale", 1.1f);
            if (x.TerrainType.IsNotMoveable) renderer.materials[1].SetColor("_Color", Color.red);
            else renderer.materials[1].SetColor("_Color", Color.green);
        });
    }

    public void OnDeSelected()
    {
        terrain.transform.localScale = new Vector3(1, 1, 1);
        var renderer = terrain.GetComponent<Renderer>();
        renderer.materials[1].SetFloat("_Scale", 1f);

        Neighbours.ForEach(x =>
        {
            x.terrain.transform.localScale = new Vector3(1, 1, 1);
            var renderer = x.terrain.GetComponent<Renderer>();
            renderer.materials[1].SetFloat("_Scale", 1f);
        });
    }

    public bool Equals(HexCell other)
    {
        return OffsetCoordinates == other.OffsetCoordinates;
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

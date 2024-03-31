using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexCellGenerator : MonoBehaviour
{
    [field: SerializeField] public HexGrid Grid { get; set; }
    [field: SerializeField] public MapGenerator MapGenerator { get; set; }
    [field: SerializeField] public int BatchSize { get; set; } = 2;

    private List<HexCell> cells = new();

    private void Awake()
    {
        if (Grid == null) Grid = GetComponent<HexGrid>();
        if (MapGenerator == null) MapGenerator = GetComponent<MapGenerator>();
    }

    public void Generate()
    {
        if (MapGenerator.TerrainMap == null) MapGenerator.Generate();
        SetHexCellTerrainTypes(MapGenerator.TerrainMap);
    }

    private void SetHexCellTerrainTypes(TerrainType[,] terrainMap)
    {
        Debug.Log("Setting Hex Cell Terrain Types");
        ClearHexCells();

        cells = GenerateHexCells(Grid, terrainMap);
        StartCoroutine(InstantiateCells(cells));
    }

    public List<HexCell> GenerateHexCells(HexGrid grid, TerrainType[,] terrainMap) =>
        Enumerable.Range(0, grid.Height)
            .Select(y =>
                Enumerable.Range(0, grid.Width)
                    .Select(x =>
                    {
                        int flippedX = grid.Width - x - 1;
                        int flippedY = grid.Height - y - 1;
                        var center = HexHelpers.GetCenter(grid.HexSize, x, y, grid.Orientation);
                        //Vector3 centrePosition = HexMetrics.Center(HexSize, x, -y, Orientation) + gridOrigin;
                        return HexCell.Create(grid, grid.HexSize)
                            .SetCoordinates(new Vector2(x, y), grid.Orientation)
                            .SetTerrainType(terrainMap[flippedY, flippedX]);
                    })
            )
            .SelectMany(x => x)
            .ToList();

    private void ClearHexCells()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].ClearTerrain();
        }
        cells.Clear();
    }

    private IEnumerator InstantiateCells(List<HexCell> hexCells)
    {
        Debug.Log("Instantiating Hex Cells");
        int batchCount = 0;
        int totalBatches = Mathf.CeilToInt(hexCells.Count / BatchSize);
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].CreateTerrain();
            // Yield every batchSize hex cells
            if (i % BatchSize == 0 && i != 0)
            {
                batchCount++;
                // OnCellBatchGenerated?.Invoke((float)batchCount / totalBatches);
                yield return null;
            }
        }

        // OnCellInstancesGenerated?.Invoke();
    }
}
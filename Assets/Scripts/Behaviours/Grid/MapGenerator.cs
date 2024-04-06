using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(HexGrid))]
public class MapGenerator : MonoBehaviour
{
    [field: SerializeField] public HexGrid Grid { get; set; }
    [field: SerializeField] public List<TerrainType> Biomes = new();
    public TerrainType[,] TerrainMap { get; private set; }

    public event Action<HexCell[][]> OnCellsGenerated;

    private void Awake()
    {
        Grid = GetComponent<HexGrid>();
    }

    public void Generate()
    {
        TerrainMap = GenerateTerrain();
    }

    public TerrainType[,] GenerateTerrain() =>
        Enumerable.Range(0, Grid.Height)
            .Select(z => Enumerable
                .Range(0, Grid.Width)
                .Select(x =>
                {
                    return GetRandomBiome();
                })
            ).To2DArray();

    public TerrainType GetRandomBiome()
    {
        return Biomes[Random.Range(0, Biomes.Count)];
    }
}

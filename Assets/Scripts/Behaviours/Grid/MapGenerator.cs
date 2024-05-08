using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

using Random = UnityEngine.Random;

[Serializable]
public class Biome
{
    public TerrainType TerrainType;
    public int Rarity = 1;
}

[RequireComponent(typeof(HexGrid))]
public class MapGenerator : MonoBehaviour
{
    [field: SerializeField] public HexGrid Grid { get; set; }
    [field: SerializeField] public List<Biome> Biomes = new();
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
                    return GetBiomeByRarity();
                })
            ).To2DArray();

    public TerrainType GetBiomeByRarity()
    {
        var totalRarity = Biomes.Sum(x => x.Rarity);
        var random = Random.Range(0, totalRarity);
        var current = 0;

        foreach (var biome in Biomes)
        {
            current += biome.Rarity;
            if (random < current)
            {
                return biome.TerrainType;
            }
        }

        return Biomes.Last().TerrainType;
    }
}

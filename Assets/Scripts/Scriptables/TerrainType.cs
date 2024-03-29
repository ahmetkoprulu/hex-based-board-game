using UnityEngine;

[CreateAssetMenu(fileName = "TerrainType", menuName = "Scriptables/TerrainType")]
public class TerrainType : ScriptableObject
{
    [Header("General")]
    [SerializeField] private int id;
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public Color Color { get; set; }
    [field: SerializeField] public Transform Prefab { get; set; }
    [field: SerializeField] public Sprite Icon { get; set; }
}
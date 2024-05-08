using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "UnitType", menuName = "Scriptables/UnitType")]
public class UnitType : ScriptableObject
{
    [Header("General")]
    [SerializeField] private int id;
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public string Description { get; set; }
    [field: SerializeField] public int Health { get; set; }
    [field: SerializeField] public int Attack { get; set; }
    [field: SerializeField] public int Defence { get; set; }
    [field: SerializeField] public int Movement { get; set; }
    [field: SerializeField] public int Range { get; set; }
    [field: SerializeField] public Transform Canvas { get; set; }
    [field: SerializeField] public Transform Prefab { get; set; }
    [field: SerializeField] public Sprite Icon { get; set; }
}

public enum Faction
{
    Player, //Red
    Opponent, //Blue
    None,
}
using UnityEngine;

[CreateAssetMenu(fileName = "CardType", menuName = "Scriptables/CardType")]
public class CardData : ScriptableObject
{
    [Header("Card graphics")]
    public Sprite cardImage;

    [Header("List of Placeables")]
    public UnitType[] placeablesData; //link to all the Placeables that this card spawns
    public Vector3[] relativeOffsets; //the relative offsets (from cursor) where the placeables will be dropped
}
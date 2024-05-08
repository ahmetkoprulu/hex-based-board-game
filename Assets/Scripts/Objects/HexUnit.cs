using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit
{
    public UnitType UnitType { get; set; }
    private Transform UnitObject { get; set; }
    private Transform CanvasObject { get; set; }
    private Vector3 InitialRotation { get; set; }

    public int Health { get; set; }
    public int Attack { get; set; }
    public int Range { get; set; }
    public int Movement { get; set; }
    public int AttackSpeed { get; set; }

    public static HexUnit Create(UnitType type, HexCell cell)
    {
        Vector3 centrePosition = HexHelpers.GetCenter(
            cell.Size,
            (int)cell.OffsetCoordinates.x,
            (int)cell.OffsetCoordinates.y, cell.Grid.Orientation
        ) + cell.Grid.transform.position;

        var unitObject = Object.Instantiate(
           type.Prefab,
           centrePosition,
           Quaternion.identity,
           cell.Grid.transform
           );
        var initialRotation = new Vector3(unitObject.transform.rotation.x, unitObject.transform.rotation.y, unitObject.transform.rotation.z);

        var canvasObject = Object.Instantiate(
            type.Canvas,
            centrePosition + new Vector3(0, 2.3f, 0),
            Quaternion.identity,
            unitObject.transform
        );

        return new HexUnit
        {
            UnitType = type,
            UnitObject = unitObject,
            CanvasObject = canvasObject,
            InitialRotation = initialRotation,
            Health = type.Health,
            Attack = type.Attack,
            Range = type.Range,
            Movement = type.Movement,
        };
    }

    public void Move(HexCell cell)
    {
        Vector3 centrePosition = HexHelpers.GetCenter(
            cell.Size,
            (int)cell.OffsetCoordinates.x,
            (int)cell.OffsetCoordinates.y, cell.Grid.Orientation
        ) + cell.Grid.transform.position;

        UnitObject.position = centrePosition;
    }

    public IEnumerator Move(List<Vector3> path)
    {
        Animator animator = UnitObject.GetComponent<Animator>();
        animator.SetBool("IsMoving", true);

        foreach (var point in path)
        {
            Rotate(point - UnitObject.position, UnitObject.transform);
            UnitObject.position = point;

            yield return null;
        }

        UnitObject.transform.rotation = Quaternion.LookRotation(InitialRotation);
        animator.SetBool("IsMoving", false);
    }

    public void Rotate(Vector3 direction, Transform transform)
    {
        var rotationSpeed = 5f;
        // direction.y = 0f; // Optional: Ensure rotation is only in the horizontal plane
        // Calculate the rotation needed to point towards the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public string Id;
    public double X;
    public double Y;
    public double Cos; // Used for middlepoint calculations
    public Vector3 Point;

    public static Node Zero => new(0, 0);

    public Node(double x, double y)
    {
        X = x;
        Y = y;
        Id = Guid.NewGuid().ToString();
    }

    public Node(Vector3 point)
    {
        X = point.x;
        Y = point.z;
        Id = Guid.NewGuid().ToString();
        Point = point;
    }

    public static Node operator -(Node a, Node b)
    {
        return new Node(a.X - b.X, a.Y - b.Y);
    }

    public static Node operator +(Node a, Node b)
    {
        return new Node(a.X + b.X, a.Y + b.Y);
    }

    public static Node operator +(Node a, Vector3 b)
    {
        return new Node(a.X + b.x, a.Y + b.z);
    }

    public static Node operator *(Node a, double b)
    {
        return new Node(a.X * b, a.Y * b);
    }

    public static Node operator /(Node a, double b)
    {
        return new Node(a.X / b, a.Y / b);
    }

    public static Node operator *(float a, Node b)
    {
        return new Node(a * b.X, a * b.Y);
    }

    public static bool operator >(Node a, Node b)
    {
        return a.X + a.Y > b.X + b.Y;
    }

    public static bool operator <(Node a, Node b)
    {
        return a.X + a.Y < b.X + b.Y;
    }

    public static bool operator ==(Node a, Node b)
    {
        // Debug.Log($"Comparing {a.X} == {b.X} and {a.Y} == {b.Y} = {a.X == b.X && a.Y == b.Y}");
        return a.Point == b.Point;
    }

    public static bool operator !=(Node a, Node b)
    {
        return a.Point != b.Point;
    }

    // Put Grid 0.5 forward to avoid (7,0,-0.5) and (7,0,0.5) to be considered different otherwise referering to origin for distance will be failed
    public static Node Max(Node a, Node b)
    {
        // calculate distance from 0,0 to a and b
        var distanceA = Math.Sqrt(a.X * a.X + a.Y * a.Y);
        var distanceB = Math.Sqrt(b.X * b.X + b.Y * b.Y);

        return distanceA > distanceB ? a : b;
    }

    public static Node Min(Node a, Node b)
    {
        // calculate distance from 0,0 to a and b
        var distanceA = Math.Sqrt(a.X * a.X + a.Y * a.Y);
        var distanceB = Math.Sqrt(b.X * b.X + b.Y * b.Y);

        return distanceA < distanceB ? a : b;
    }

    public static implicit operator Vector3(Node node)
    {
        return new Vector3((float)node.X, 0, (float)node.Y);
    }

    public override int GetHashCode()
    {
        return Convert.ToInt32(X) + Convert.ToInt32(Y);
    }
}

public class NodeEqualityComparer : IEqualityComparer<Node>
{
    public static NodeEqualityComparer Default { get; } = new NodeEqualityComparer();

    public bool Equals(Node x, Node y)
    {
        var a = x.X == y.X && x.Y == y.Y;
        // Debug.Log($"Comparing {x.X} == {y.X} and {x.Y} == {y.Y} = {a}");
        return a;
    }

    // implement max and min for x and y


    public int GetHashCode(Node obj)
    {
        return $"{obj.X}, {obj.Y}".GetHashCode();
    }
}
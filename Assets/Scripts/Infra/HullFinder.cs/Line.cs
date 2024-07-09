using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Line
{
    public Node[] nodes = new Node[2];
    public Line next;
    public Node Start => nodes[0];
    public Node End => nodes[1];

    public Line(Node n1, Node n2)
    {
        nodes[0] = n1;
        nodes[1] = n2;
    }

    public static double getLength(Node node1, Node node2)
    {
        /* It actually calculates relative length */
        double length;
        length = Math.Pow(node1.Y - node2.Y, 2) + Math.Pow(node1.X - node2.X, 2);
        //length = Math.sqrt(Math.Pow(node1.y - node2.y, 2) + Math.Pow(node1.x - node2.x, 2));
        return length;
    }

    public static List<Node> GetSortedNodes(HashSet<Node> nodes)
    {
        var sorted = new List<Node>();
        if (nodes.Count == 0) return sorted;

        var current = nodes.First();
        sorted.Add(current);
        nodes.Remove(current);

        while (nodes.Count > 0)
        {
            var next = nodes.OrderBy(x => getLength(current, x)).First();
            sorted.Add(next);
            nodes.Remove(next);
            current = next;
        }

        return sorted;
    }

    public static List<Line> GetContiniousLines(List<Line> lines)
    {
        if (lines == null || lines.Count == 0) return new();

        var positions = new List<Line>();
        var starting = lines[0];
        positions.Add(starting);
        lines.Remove(starting);

        while (lines.Count > 0)
        {
            var next = lines.FirstOrDefault(x => x.Start == starting.End || x.End == starting.End);
            if (next == null) break;

            var line = next.Start == starting.End ? next : new Line(next.End, next.Start);
            positions.Add(line);
            lines.Remove(next);
            starting = line;

            if (line.End == positions[0].Start) break;
        }

        return positions;
    }

    public static List<Node> GetContiniousNodes(List<Line> lines)
    {
        if (lines == null || lines.Count == 0) return null;

        var nodes = lines.Select(x => x.Start).ToList();

        return nodes;
    }

    public static List<Line> GetSortedLines(List<Line> lines)
    {
        if (lines == null || lines.Count == 0)
            return null;

        List<Line> orderedLines = new List<Line>();
        Line currentLine = lines[0];
        orderedLines.Add(currentLine);
        lines.RemoveAt(0);

        while (lines.Count > 0)
        {
            bool foundNext = false;
            for (int i = 0; i < lines.Count; i++)
            {
                Line line = lines[i];
                if (line.Start == currentLine.End)
                {
                    currentLine = line;
                    orderedLines.Add(line);
                    lines.RemoveAt(i);
                    foundNext = true;
                    break;
                }
                else if (line.End == currentLine.End)
                {
                    currentLine = new Line(line.End, line.Start); // Reverse the line
                    orderedLines.Add(currentLine);
                    lines.RemoveAt(i);
                    foundNext = true;
                    break;
                }
            }

            if (!foundNext)
            {
                // Debug.WriteLine("Cannot find the next line to continue the perimeter.");
                break;
            }
        }

        return orderedLines;
    }


    public (Node start, Node end) GetNodes()
    {
        return (nodes[0], nodes[1]);
    }

    public static Line Max(Line line1, Line line2)
    {
        return line1 > line2 ? line1 : line2;
    }

    public static bool operator >(Line v1, Line v2)
    {
        return v1.Start.X + v1.End.X > v2.Start.X + v2.End.X;
    }

    public static bool operator <(Line v1, Line v2)
    {
        return v1.Start.X + v1.End.X < v2.Start.X + v2.End.X;
    }
}

public class LineEqualityComparer : IEqualityComparer<Line>
{
    public static LineEqualityComparer Default { get; } = new LineEqualityComparer();

    public bool Equals(Line x, Line y)
    {
        var a = (x.nodes[0] == y.nodes[0] && x.nodes[1] == y.nodes[1]) || (x.nodes[0] == y.nodes[1] && x.nodes[1] == y.nodes[0]);
        // Debug.WriteLine($"Comparing {x.nodes[0].Point} == {y.nodes[0].Point} and {x.nodes[1].Point} == {y.nodes[1].Point} = {a}");
        return a;
    }

    public int GetHashCode(Line obj)
    {
        var a = obj.nodes[0].GetHashCode();
        var b = obj.nodes[1].GetHashCode();

        return a ^ b;
    }
}
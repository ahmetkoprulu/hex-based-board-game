using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HullFinder
{
    public List<Vector3> Points = new();
    public List<Node> Nodes = new();

    public HullType HullType;

    public HullFinder(HullType hullType)
    {
        HullType = hullType;
    }

    public HullFinder SetPoints(List<Vector3> points)
    {
        Points = points;
        Nodes = points.ConvertAll(x => new Node(x));

        return this;
    }

    public List<Line> getHull()
    {
        var exitLines = new List<Line>();
        var convexH = new List<Node>();
        convexH.AddRange(GrahamScan.convexHull(Nodes));

        for (int i = 0; i < convexH.Count - 1; i++)
        {
            exitLines.Add(new Line(convexH[i], convexH[i + 1]));
        }
        exitLines.Add(new Line(convexH[0], convexH[convexH.Count - 1]));
        return exitLines;
    }

    public List<Line> CalculateHull(double concavity = 0d, int scaleFactor = 0) => HullType switch
    {
        HullType.Convex => CalculateConvexHull(),
        HullType.Concave => CalculateConcaveHull(concavity, scaleFactor),
        _ => throw new ArgumentOutOfRangeException()
    };


    private List<Line> CalculateConvexHull()
    {
        var edges = getHull();
        // foreach (Line line in edges)
        // {
        //     foreach (Node node in line.nodes)
        //     {
        //         Nodes.RemoveAll(a => a.Id == node.Id);
        //     }
        // }

        return edges;
    }

    private List<Line> CalculateConcaveHull(double concavity, int scaleFactor)
    {
        /* Run setConvHull before! 
         * Concavity is a value used to restrict the concave angles 
         * It can go from -1 (no concavity) to 1 (extreme concavity) 
         * Avoid concavity == 1 if you don't want 0º angles
         * */

        var edges = CalculateConvexHull();
        Nodes = Nodes.Except(edges.SelectMany(x => x.nodes)).ToList();

        bool aLineWasDividedInTheIteration;
        do
        {
            aLineWasDividedInTheIteration = false;
            for (int linePositionInHull = 0; linePositionInHull < edges.Count && !aLineWasDividedInTheIteration; linePositionInHull++)
            {
                Line line = edges[linePositionInHull];
                List<Node> nearbyPoints = HullFunctions.getNearbyPoints(line, Nodes, scaleFactor);
                List<Line> dividedLine = HullFunctions.getDividedLine(line, nearbyPoints, edges, concavity);
                if (dividedLine.Count > 0)
                { // Line divided!
                    aLineWasDividedInTheIteration = true;
                    Nodes.Remove(Nodes.Where(n => n.Id == dividedLine[0].nodes[1].Id).FirstOrDefault()); // Middlepoint no longer free
                    edges.AddRange(dividedLine);
                    edges.RemoveAt(linePositionInHull); // Divided line no longer exists
                }
            }

            edges = edges.OrderByDescending(a => Line.getLength(a.nodes[0], a.nodes[1])).ToList();
        } while (aLineWasDividedInTheIteration);

        return edges;
    }
}

public enum HullType
{
    Convex = 0,
    Concave = 1
}
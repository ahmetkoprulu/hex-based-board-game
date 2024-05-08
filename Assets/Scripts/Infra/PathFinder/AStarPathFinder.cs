using System.Collections.Generic;
using UnityEngine;
using static HexGridManager;

public class AStarPathFinder : IPathFinder
{
    private static readonly int MOVE_COST = 1;

    public List<HexCell> FindPath(HexCell from, HexCell to)
    {
        // Implement A* utilizing priority queue
        var openSet = new PriorityQueue<double, Node>(new AscendingComparer<double>());
        var startNode = new Node(from, null, 0, GetDistance(from.CubeCoordinates, to.CubeCoordinates));
        openSet.Enqueue(0, startNode);

        var closedSet = new Dictionary<Vector3, Node> { { startNode.Cell.CubeCoordinates, startNode } };

        while (openSet.Count > 0)
        {
            var currentNode = openSet.Dequeue();

            if (currentNode.Cell.CubeCoordinates == to.CubeCoordinates)
            {
                return Backtrack(currentNode);
            }

            foreach (var neighbour in currentNode.Cell.MoveableNeighbours)
            {
                var newCost = currentNode.G + MOVE_COST; // Cost is always 1 for this case

                if (!closedSet.ContainsKey(neighbour.CubeCoordinates) || newCost < closedSet[neighbour.CubeCoordinates].G)
                {
                    var heuristicScore = newCost + GetDistance(neighbour.CubeCoordinates, to.CubeCoordinates);
                    var nextNode = new Node(neighbour, currentNode, newCost, heuristicScore);

                    openSet.Enqueue(nextNode.F, nextNode);
                    closedSet[neighbour.CubeCoordinates] = nextNode;
                }
            }
        }

        return new(); // No path found
    }

    private static List<HexCell> Backtrack(Node targetNode)
    {
        var result = new List<HexCell>();
        var node = targetNode;

        while (node.Prev != null)
        {
            result.Add(node.Cell);
            node = node.Prev;
        }

        result.Add(node.Cell); // add origin node as well

        result.Reverse();
        return result;
    }

    private static double GetDistance(Vector3 from, Vector3 to) => Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) + Mathf.Abs(from.z - to.z);

    public class Node
    {
        public HexCell Cell { get; set; }
        public double G { get; set; } // Cost score
        public double H { get; set; } // Heuristic score
        public double F => G + H; // Total score

        public Node Prev { get; set; }

        public Node(HexCell coordinates, Node prev, double costScore, double heuristicScore)
        {
            Cell = coordinates;
            Prev = prev;
            G = costScore;
            H = heuristicScore;
        }
    }
}
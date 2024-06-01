
// using UnityEngine;
// using System.Collections.Generic;
// namespace ConcaveHull
// {
//     public class Init : MonoBehaviour
//     {
//         List<Node> dot_list = new List<Node>();
//         List<Line> Edges = new List<Line>();

//         public string seed; public int scaleFactor; public int number_of_dots; public double concavity;

//         void Start()
//         {
//             setDots(number_of_dots);
//             generateHull();
//         }

//         public void generateHull()
//         {
//             var a = new HullFinder(HullType.Concave);
//             a.SetPoints(dot_list.ConvertAll(x => new Vector3((float)x.X, 0, (float)x.Y)));
//             Edges = a.CalculateHull(concavity, scaleFactor);
//         }

//         public void setDots(int number_of_dots)
//         {
//             System.Random pseudorandom = new System.Random(seed.GetHashCode());
//             for (int x = 0; x < number_of_dots; x++)
//             {
//                 dot_list.Add(new Node(pseudorandom.Next(0, 100), pseudorandom.Next(0, 100), x));
//             }

//             for (int pivot_position = 0; pivot_position < dot_list.Count; pivot_position++)
//             {
//                 for (int position = 0; position < dot_list.Count; position++)
//                 {
//                     if (dot_list[pivot_position].X == dot_list[position].X && dot_list[pivot_position].Y == dot_list[position].Y && pivot_position != position) { dot_list.RemoveAt(position); position--; }
//                 }
//             }
//         }

//         void OnDrawGizmos()
//         {
//             // Gizmos.color = Color.yellow;
//             // for (int i = 0; i < Hull.hull_edges.Count; i++)
//             // {
//             //     Vector2 left = new Vector2((float)Hull.hull_edges[i].nodes[0].X, (float)Hull.hull_edges[i].nodes[0].Y);
//             //     Vector2 right = new Vector2((float)Hull.hull_edges[i].nodes[1].X, (float)Hull.hull_edges[i].nodes[1].Y);
//             //     Gizmos.DrawLine(left, right);
//             // }

//             Gizmos.color = Color.blue;
//             for (int i = 0; i < Edges.Count; i++)
//             {
//                 Vector2 left = new Vector2((float)Edges[i].nodes[0].X, (float)Edges[i].nodes[0].Y);
//                 Vector2 right = new Vector2((float)Edges[i].nodes[1].X, (float)Edges[i].nodes[1].Y);
//                 Gizmos.DrawLine(left, right);
//             }

//             Gizmos.color = Color.red;
//             for (int i = 0; i < dot_list.Count; i++)
//             {
//                 Gizmos.DrawSphere(new Vector3((float)dot_list[i].X, (float)dot_list[i].Y, 0), 0.5f);
//             }
//         }
//     }
// }

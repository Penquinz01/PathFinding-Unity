using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;

public class PathFinding : MonoBehaviour
{
    GridSystem grid;
    public Transform seekr;
    public Transform target;

    private void Awake()
    {
        grid = GetComponent<GridSystem>();
    }
    [Button("FindPath")]
    private void GenratePath()
    {
        FindPath(seekr.position, target.position);
    }

    void FindPath(Vector2 startPos,Vector2 targetPos)
    {
        print("Started");
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Node startNode = grid.PointToNode(startPos);
        Node targetNode = grid.PointToNode(targetPos);

        Heap<Node> Openset = new Heap<Node>(grid.MaxGridSize);
        HashSet<Node> ClosedSet = new HashSet<Node>();
        Openset.Add(startNode);

        while (Openset.Count > 0)
        {
            Node currentNode = Openset.RemoveFirst();
            ClosedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                sw.Stop();
                print("path found: " + sw.ElapsedMilliseconds + "ms");
                RetracePath(startNode, targetNode);
                return;
            }
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (ClosedSet.Contains(neighbour) || !neighbour.walkable) continue;

                int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.GCost || !Openset.Contains(neighbour))
                {
                    neighbour.GCost = newMovementCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    
                    if (!Openset.Contains(neighbour))
                    {
                        Openset.Add(neighbour);
                    }
                }
            }
        }
    }
    int GetDistance(Node nodeA,Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY= Mathf.Abs(nodeA.gridY - nodeB.gridY);
        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
    void RetracePath(Node start,Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;
        while (currentNode != null) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        grid.path = path;
    }
}

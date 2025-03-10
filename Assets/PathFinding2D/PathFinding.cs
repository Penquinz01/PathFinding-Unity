using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using Sirenix.OdinInspector;
using System;
using System.Collections;

public class PathFinding : MonoBehaviour
{
    GridSystem grid;
    PathFinderManager pathFinder;
    private void Awake()
    {
        pathFinder = GetComponent<PathFinderManager>();
        grid = GetComponent<GridSystem>();
    }

    IEnumerator FindPath(Vector2 startPos,Vector2 targetPos)
    {
        print("Started");
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Vector2[] waypoints = new Vector2[0];
        bool pathSuccess = false;
        grid.path.Clear();
        Node startNode = grid.PointToNode(startPos);
        Node targetNode = grid.PointToNode(targetPos);
        if(startNode.walkable && targetNode.walkable)
        {
            Heap<Node> Openset = new Heap<Node>(grid.MaxGridSize);
            HashSet<Node> ClosedSet = new HashSet<Node>();
            Openset.Add(startNode);

            while (Openset.Count > 0)
            {
                Node currentNode = Openset.RemoveFirst();
                ClosedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("path found: " + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (ClosedSet.Contains(neighbour) || !neighbour.walkable) continue;

                    int newMovementCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    print(neighbour.movementPenalty);
                    if (newMovementCostToNeighbour < neighbour.GCost || !Openset.Contains(neighbour))
                    {
                        neighbour.GCost = newMovementCostToNeighbour;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!Openset.Contains(neighbour))
                        {
                            Openset.Add(neighbour);
                        }
                        else
                        {
                            Openset.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        pathFinder.FinishedProcessingPath(waypoints, pathSuccess);
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
    Vector2[] RetracePath(Node start,Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;
        while (currentNode != start) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(start);
        Vector2[] waypoints = SimplifyPath(path);
        path.Reverse();
        grid.path = path;
        Array.Reverse(waypoints);
        return waypoints;

    }

    Vector2[] SimplifyPath(List<Node> path)
    {
        List<Vector2> way =  new List<Vector2>();
        Vector2 dirOld = Vector2.zero;  

        for(int i = 0; i < path.Count; i++)
        {
            Vector2 dirNew = path[i].worldPosition;
            if (dirNew != dirOld)
            {
                way.Add(dirNew);              
            }
            dirOld = dirNew;
        }
        return way.ToArray();
    }

    public void StartFindPath(Vector2 pathStart, Vector2 pathEnd)
    {
        StopCoroutine("FindPath");
        StartCoroutine(FindPath(pathStart,pathEnd));
    }
}

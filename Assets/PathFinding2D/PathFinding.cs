using UnityEngine;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
    GridSystem grid;

    private void Awake()
    {
        grid = GetComponent<GridSystem>();
    }
    
    void FindPath(Vector2 startPos,Vector2 targetPos)
    {
        Node startNode = grid.PointToNode(startPos);
        Node targetNode = grid.PointToNode(targetPos);

        List<Node> Openset = new List<Node>();
        HashSet<Node> ClosedSet = new HashSet<Node>();
        Openset.Add(startNode);

        while (Openset.Count > 0)
        {
            Node currentNode = Openset[0];
            for (int i = 1; i < Openset.Count; i++)
            {
                if (Openset[i].FCost < currentNode.FCost || Openset[i].FCost==currentNode.FCost && Openset[i].HCost<currentNode.HCost)
                {
                    currentNode = Openset[i];
                }
            }
            Openset.Remove(currentNode);
            ClosedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                return;
            }
            foreach (Node node in grid.GetNeighbours(currentNode))
            {
                if (ClosedSet.Contains(node) || !node.walkable) continue;
                    
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
}

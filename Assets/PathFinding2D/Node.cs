using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector2 worldPosition;
    public int GCost;
    public int HCost;
    public int gridX;
    public int gridY;
    public Node parent;
    int heapIndex;
    public int movementPenalty;
    public Node(bool walkable, Vector2 worldPosition,int gridX,int gridY,int penalty)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
        movementPenalty = penalty;
    }

    public int FCost
    {
        get
        {
            return GCost + HCost;
        }
    }

    public int HeapIndex { get => heapIndex; set => heapIndex = value; }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if(compare == 0)
        {
            compare = HCost.CompareTo(other.HCost);
        }
        

        return -compare;
    }
}

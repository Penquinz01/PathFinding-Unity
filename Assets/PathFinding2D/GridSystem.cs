using UnityEngine;
using System.Collections.Generic;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int height = 10;
    [SerializeField] private int width = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private LayerMask _unwalkable;
    [SerializeField] private Transform player;
    Node[,] grid;
    private Vector2 topLeft;
    

    private void Start()
    {
        grid = new Node[width, height];
        topLeft = CalculateTopLeft();
        GenerateGrid();
    }
    private void GenerateGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector2 worldPoint = new(topLeft.x + x * cellSize, topLeft.y - y * cellSize);
                bool walkable = !Physics2D.OverlapCircle(worldPoint,cellSize/2,_unwalkable);
                grid[x, y] = new Node(walkable, worldPoint,x,y);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(width, height) * cellSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(topLeft, Vector3.one * (cellSize-0.1f));
        Node _player = PointToNode(player.position);
        foreach (Node n in grid)
        {
            Gizmos.color = n.walkable ? Color.white : Color.magenta;
            if (n == _player)
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawCube(n.worldPosition, Vector3.one * (cellSize - 0.1f));
        }
    }
    private Vector2 CalculateTopLeft()
    {
        float x = transform.position.x - ((cellSize / 2) * (width/2-1));
        float y = transform.position.y + ((cellSize / 2) * (height/2 -1));
        return new Vector2(x, y);
    }
    public Node PointToNode(Vector2 point)
    {
        int x = Mathf.RoundToInt((point.x - topLeft.x) / cellSize);
        int y = Mathf.RoundToInt((topLeft.y - point.y) / cellSize);
        return grid[x, y];
    }
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    neighbours.Add(grid[checkX,checkY]);
                }
            }
        }
        return neighbours;
    }
}

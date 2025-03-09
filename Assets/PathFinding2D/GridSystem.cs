using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int height = 10;
    [SerializeField] private int width = 10;
    [SerializeField][Range(0.62f,3f)] private float cellSize = 1f;
    [SerializeField] private LayerMask _unwalkable;
    [SerializeField] private Transform player;
    Node[,] grid;
    private Vector2 topLeft;
    int gridWidth;
    int gridHeight;
    private bool started = false;

    public List<Node> path;
    [Button("Generate Grid")]
    private void Start()
    {
        started = true;
        gridWidth = Mathf.FloorToInt(width / cellSize);
        gridHeight = Mathf.FloorToInt(height / cellSize);
        grid = new Node[gridWidth, gridHeight];
        topLeft = CalculateTopLeft();
        GenerateGrid();
    }
    
    private void GenerateGrid()
    {
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPoint = new(topLeft.x + x * cellSize, topLeft.y - y * cellSize);
                bool walkable = !Physics2D.OverlapCircle(worldPoint,cellSize/2,_unwalkable);
                grid[x, y] = new Node(walkable, worldPoint,x,y);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector2(width, height) * cellSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(topLeft, Vector3.one * (cellSize-0.05f));
        if (!started) return;
        Node _player = PointToNode(player.position);
        foreach (Node n in grid)
        {
            Gizmos.color = n.walkable ? Color.white : Color.magenta;
            if (n == _player && _player !=null)
            {
                Gizmos.color = Color.green;
            }
            if (path != null&&path.Contains(n))Gizmos.color = Color.black;
            Gizmos.DrawCube(n.worldPosition, Vector3.one * Mathf.Max((cellSize - 0.1f),0.04f));
        }
    }
    private Vector2 CalculateTopLeft()
    {
        float x = transform.position.x - (width/2);
        float y = transform.position.y + (height/2);
        return new Vector2(x, y);
    }
    public Node PointToNode(Vector2 point)
    {
        int x = Mathf.FloorToInt((point.x - topLeft.x) / cellSize);
        int y = Mathf.FloorToInt((topLeft.y - point.y) / cellSize);
        x = Mathf.Clamp(x, 0, gridWidth-1);
        y = Mathf.Clamp(y, 0, gridHeight-1);
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

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int height = 10;
    [SerializeField] private int width = 10;
    [SerializeField][Range(0.62f,3f)] private float cellSize = 1f;
    [SerializeField] private LayerMask _unwalkable;
    [SerializeField] private Transform player;
    public TerrainType[] walkableRegion;
    Node[,] grid;
    private Vector2 topLeft;
    int gridWidth;
    int gridHeight;
    private bool started = false;
    public bool DisplaGridGizmos = false;
    public List<Node> path = new();
    LayerMask walkableMask;
    Dictionary<int,int> walkableRegionDictionary = new Dictionary<int,int>();

    private void Awake()
    {
        started = true;
        gridWidth = Mathf.FloorToInt(width / cellSize);
        gridHeight = Mathf.FloorToInt(height / cellSize);
        grid = new Node[gridWidth, gridHeight];
        topLeft = CalculateTopLeft();
        foreach(TerrainType region in walkableRegion)
        {
            walkableMask.value |= region.terrainMask.value;
            walkableRegionDictionary.Add((int)Mathf.Log(region.terrainMask.value,2),region.terrainPenalty);
        }
        GenerateGrid();
    }
    public int MaxGridSize
    {
        get => gridHeight * gridWidth* 2;
    }
    
    private void GenerateGrid()
    {
        for(int x = 0; x < gridWidth; x++)
        {
            for(int y = 0; y < gridHeight; y++)
            {
                Vector2 worldPoint = new(topLeft.x + x * cellSize, topLeft.y - y * cellSize);
                bool walkable = !Physics2D.OverlapCircle(worldPoint,cellSize/2,_unwalkable);
                int movementPenalty = 0;

                if (walkable) {
                    RaycastHit2D hit = Physics2D.Raycast(new Vector3(worldPoint.x, worldPoint.y, 0) + Vector3.back * 50, Vector3.forward, 100, walkableMask);
                    if (hit.collider!=null)
                    {
                        walkableRegionDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                }

                grid[x, y] = new Node(walkable, worldPoint,x,y,movementPenalty);
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
        if(grid != null && DisplaGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.magenta;
                if (n == _player && _player != null)
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * Mathf.Max((cellSize - 0.1f), 0.04f));
            }
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
    [System.Serializable]
    public class TerrainType
    {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
    [Button("Print all nodes walkPenalty")]
    public void PrintPenalty(){
        foreach(Node n in grid)
        {
            Debug.Log(n.movementPenalty + " " + n.worldPosition);
        }

    }
}

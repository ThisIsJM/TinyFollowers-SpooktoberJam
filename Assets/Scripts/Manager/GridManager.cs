using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class GridManager : MonoBehaviour
{
    public Grid<PathNode> grid;
    public int width;
    public int height;
    public float cellSize;

    public static GridManager instance;

    public Tilemap floors;

    private void Awake()
    {
        instance = this;

        grid = new Grid<PathNode>(width, height, cellSize, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }
    // Start is called before the first frame update
    void Start()
    {
        floors = GameObject.FindGameObjectWithTag("Floor Level").GetComponent<Tilemap>();
        SetWalkables(floors);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetWalkables(Tilemap tilemap)
    {
        foreach (var position in tilemap.cellBounds.allPositionsWithin)
        {
            if (tilemap.HasTile(position))
            {
                continue;
            }

            grid.GetXY(position, out int x, out int y);
            PathNode node = GetNode(x, y);

            if (node != null)
            {
                node.isWalkable = false;
            }

        }
    }

    public Vector3 GetGridPosition(Vector3 worldPosition)
    {
        grid.GetXY(worldPosition, out int x, out int y);
        Vector3 gridPosition = grid.GetWorldPosition(x, y) + new Vector3(grid.GetCellSize(), grid.GetCellSize()) * .5f;

        return gridPosition;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    public List<PathNode> GetNeighbourNode(PathNode currentNode)
    {
        List<PathNode> neighbourNode = new List<PathNode>();
        //Left
        if (currentNode.x - 1 >= 0)
        {
            neighbourNode.Add(GetNode(currentNode.x - 1, currentNode.y));
        }
        //Right
        if (currentNode.x + 1 < grid.GetWidth())
        {
            neighbourNode.Add(GetNode(currentNode.x + 1, currentNode.y));
        }
        //Down
        if (currentNode.y - 1 >= 0)
        { 
            neighbourNode.Add(GetNode(currentNode.x, currentNode.y - 1)); 
        }
        //Up
        if (currentNode.y + 1 < grid.GetHeight())
        {
            neighbourNode.Add(GetNode(currentNode.x, currentNode.y + 1));
        }

        return neighbourNode;
    }
}

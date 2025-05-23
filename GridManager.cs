using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridSize = 4;
    public float cellSize = 2f;
    public GameObject cellPrefab;
    
    private GameObject[,] grid;

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new GameObject[gridSize, gridSize];
        
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                cell.name = $"Cell_{x}_{z}";
                grid[x, z] = cell;
            }
        }
    }
}
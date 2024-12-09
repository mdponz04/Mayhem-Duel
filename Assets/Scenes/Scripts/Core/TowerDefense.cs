using System.Collections.Generic;
using UnityEngine;

public class TowerDefense : MonoBehaviour
{
    private Grid3D grid;

    [Header("Map")]
    [SerializeField] int width = 24;
    [SerializeField] int depth = 15;
    [SerializeField] int height = 0;
    [SerializeField] float cellSize = 1f;
    [SerializeField] int horizontalSegmentLength;
    [SerializeField] int verticalSegmentLength;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject pathPrefab;

    [Header("Enemy")]
    //[SerializeField] private EnemySpawner enemySpawner;

    [Header("Economy")]
    [SerializeField] float money = 100f;
    [SerializeField] float towerCost = 50f;

    [Header("UI")]
    //[SerializeField] TowerDefenseOverlay towerDefenseOverlay;

    [Header("Health")]
    [SerializeField] float health = 100f;

    private List<Vector3[]> listPathPositions = new List<Vector3[]>();
    private Vector3[] pathPositions;

    private void Start()
    {
        grid = new Grid3D(width, height, depth, cellSize, new Vector3(-15, 0, 0));

        if (pathPositions == null)
        {
            GeneratePathPositions();
        }

        //CreatePath();
        CreateGridTiles();

        //towerDefenseOverlay.SetMoneyAmount((int)money);
        //towerDefenseOverlay.SetHealthAmount((int)health);

        Debug.Log($"Path generated with {pathPositions.Length} positions");

        InitializeEnemySpawner();
    }

    private void InitializeEnemySpawner()
    {
        //if (enemySpawner == null)
        //{
        //    enemySpawner = gameObject.AddComponent<EnemySpawner>();
        //}
        //enemySpawner.Initialize(listPathPositions);
    }

    private void CreatePath()
    {
        GameObject pathParent = new GameObject("Path");
        pathParent.transform.SetParent(transform);

        foreach (Vector3 position in pathPositions)
        {
            GameObject pathTile = Instantiate(pathPrefab, position, Quaternion.identity, pathParent.transform);
            Vector3 scale = new Vector3(1, 0.1f, 1) * cellSize;
            pathTile.transform.localScale = scale;
        }
    }

    private void CreateGridTiles()
    {
        GameObject gridParent = new GameObject("Grid");
        gridParent.transform.SetParent(transform);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (grid.GetValue(x, y, z) == 0)  // Only create tiles where towers can be placed
                    {
                        Vector3 scale = new Vector3(1, 0.1f, 1) * cellSize;
                        Vector3 position = grid.GetWorldPosition(x, y, z, scale);
                        GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity, gridParent.transform);
                        tile.transform.localScale = scale;
                    }
                }
            }
        }
    }

    private void Update()
    {
        // VR-specific input handling can be added here
        // For example, using VR controller input to place towers
    }

    void GenerateRandomPath(out int[,,] path)
    {
        path = new int[width, height, depth];
        System.Random random = new System.Random();

        List<Vector3> positions = new List<Vector3>();
        int x = 0;
        int y = 0;
        int z = 0;
        int direction = 0; // 0 = forward, 1 = left, 2 = right

        int maxIterations = 1000;
        int iterations = 0;

        for (int segment = 1; segment <= verticalSegmentLength; segment++)
        {
            x = random.Next((width / verticalSegmentLength) * (segment - 1), (width / verticalSegmentLength) * segment - 1);
            if (z == 0)
            {
                path[x, y, z] = 1;
                positions.Add(grid.GetWorldPosition(x, y, z, Vector3.one));
            }

            while (z < depth - 1 && iterations < maxIterations)
            {
                // Move forward for the horizontal segment length
                for (int i = 0; i < horizontalSegmentLength && z < depth - 1; i++)
                {
                    z++;
                    path[x, y, z] = 1;
                    positions.Add(grid.GetWorldPosition(x, y, z, Vector3.one));
                }

                // Change direction after each segment
                if (z < depth - 1)
                {
                    if (x <= 1) // Too close to top, force downward
                        direction = 2;
                    else if (x >= width - 1) // Too close to bottom, force upward
                        direction = 1;
                    else
                        direction = random.Next(1, 3); // If moving right, choose up or down; otherwise, move right
                    switch (direction)
                    {
                        case 1: // Move left
                            int temp = x;
                            x = Mathf.Max(x - random.Next(0, 3), 0);
                            for (int b = temp; b >= x; b--)
                            {
                                path[b, y, z] = 1;
                                positions.Add(grid.GetWorldPosition(b, y, z, Vector3.one));
                            }
                            break;
                        case 2: // Move right
                            temp = x;
                            x = Mathf.Min(x + random.Next(0, 3), width - 1);
                            for (int b = temp; b <= x; b++)
                            {
                                path[b, y, z] = 1;
                                positions.Add(grid.GetWorldPosition(b, y, z, Vector3.one));
                            }
                            break;
                    }
                    path[x, y, z] = 1;
                    positions.Add(grid.GetWorldPosition(x, y, z, Vector3.one));
                }

                iterations++;
            }

            // Ensure the path reaches the end of each segment
            path[x, y, Mathf.Min(z, depth - 1)] = 1;
            positions.Add(grid.GetWorldPosition(x, y, Mathf.Min(z, depth - 1), Vector3.one));
            z = 0;
        }

        pathPositions = positions.ToArray();
        listPathPositions.Add(pathPositions);
    }
    void GeneratePathPositions()
    {
        int[,,] pathGrid;
        GenerateRandomPath(out pathGrid);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    if (pathGrid[x, y, z] == 1)
                    {
                        grid.SetValue(x, y, z, 1);
                    }
                }
            }
        }
    }

    public float Money
    {
        get { return money; }
        set
        {
            money = value;
            //towerDefenseOverlay.SetMoneyAmount((int)money);
        }
    }

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            //towerDefenseOverlay.SetHealthAmount((int)health);
            if (health <= 0)
            {
                Debug.Log("Game Over");
                // FindAnyObjectByType<PauseMenu>().GameOver();
            }
        }
    }

    public void SpawnTower(string towerName, Vector3 position)
    {
        int x, y, z;
        grid.GetXYZ(position, out x, out y, out z);

        if (x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth && money >= towerCost)
        {
            if (grid.GetValue(x, y, z) == 0)
            {
                grid.SetValue(x, y, z, 2); // 2 represents a tower
                Vector3 spawnPosition = grid.GetWorldPosition(x, y, z, Vector3.one);
                // Instantiate(GameAssets.GetTowerPrefab(towerName), spawnPosition, Quaternion.identity);
                money -= towerCost;
                //towerDefenseOverlay.SetMoneyAmount((int)money);
            }
        }
    }

    public Grid3D GetGrid()
    {
        return grid;
    }

    public float GetCellSize()
    {
        return cellSize;
    }
}
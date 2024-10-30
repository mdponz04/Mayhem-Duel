using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheCastle
{
    //The grid system take the bottom right corner as its root
    public class GridSystem : MonoBehaviour
    {
        private const float cellSize = 1f;
        public GridCell[,] grid {  get; private set; }
        
        private BoxCollider boxCollider;
        public Vector3 cubeSize { get; private set; }
        private int gridSizeX;
        private int gridSizeZ;
        public Vector3 cubeTop { get; private set; }
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
            

            GenerateGrid();
        }
        private void GenerateGrid()
        {
            cubeSize = Vector3.Scale(boxCollider.size, transform.localScale);
            gridSizeX = Mathf.FloorToInt(cubeSize.x / cellSize);
            gridSizeZ = Mathf.FloorToInt(cubeSize.z / cellSize);
            cubeTop = transform.position + boxCollider.center + new Vector3(0, cubeSize.y / 2f, 0);

            /*Debug.Log("Grid length and width = " + gridSizeX + ", " + gridSizeZ + ", Cube Size = " + cubeSize);*/
            grid = new GridCell[gridSizeX, gridSizeZ];

            for(int x = 0; x < gridSizeX; x++)
            {
                for(int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 centerPosition = new Vector3(
                        x * cellSize + cellSize / 2f - cubeSize.x / 2f, 
                        0,
                        z * cellSize + cellSize / 2f - cubeSize.z / 2f);
                    /*Debug.Log("at: " + x + "," + z + " => center position = " + centerPosition);*/

                    grid[x, z] = new GridCell(centerPosition);
                }
            }
        }
        //return the x, y of the cell and the value of the cell
        public GridCell GetXY(Vector3 worldPosition, out int x, out int z)
        {
            // Convert the world position to the local position relative to the cube's top center
            Vector3 localPosition = worldPosition - cubeTop;    //cubeTop = (cubeSize.x + cubeSize.z) / 2f

            // Map the local position to the grid's X and Z indices
            x = Mathf.FloorToInt((localPosition.x + cubeSize.x / 2f) / cellSize);
            z = Mathf.FloorToInt((localPosition.z + cubeSize.z / 2f) / cellSize);
            // Check if the calculated indices are out of bounds
            if (x < 0 || x >= gridSizeX || z < 0 || z >= gridSizeZ)
            {
                // Return null if the position is outside the grid
                return null;
            }

            return GetCell(x, z);
        }
        //return the cell value
        private GridCell GetCell(int x, int z)
        {
            if (x >= 0 && x < gridSizeX && z >= 0 && z < gridSizeZ)
            {
                return grid[x, z];
            }
            else
            {
                return null; // Return null if the indices are out of bounds
            }
        }
        
        //Debug draw cannot use like UI
        private void OnDrawGizmos()
        {
            if (grid != null)
            {
                // Visualize the grid
                for (int x = 0; x < gridSizeX; x++)
                {
                    for (int z = 0; z < gridSizeZ; z++)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(grid[x, z].gridCenterPosition + cubeTop, 0.1f);
                        Gizmos.color = Color.blue;
                        //draw x
                        Gizmos.DrawLine(
                            grid[x, z].gridCenterPosition + cubeTop + new Vector3(-cellSize / 2f, 0f, -cellSize / 2f),
                            grid[x, z].gridCenterPosition + cubeTop + new Vector3(cellSize / 2f, 0f, -cellSize / 2f));
                        //draw z
                        Gizmos.DrawLine(
                            grid[x, z].gridCenterPosition + cubeTop + new Vector3(-cellSize / 2f, 0f, -cellSize / 2f),
                            grid[x, z].gridCenterPosition + cubeTop + new Vector3(-cellSize / 2f, 0f, cellSize / 2f));
                    }
                }
            }
        }
    }
}

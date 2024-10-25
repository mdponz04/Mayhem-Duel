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
        private Vector3 cubeSize;
        private int gridSizeX;
        private int gridSizeZ;
        private Vector3 cubeTop;
        private void Start()
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
                //Draw the top line
                Gizmos.DrawLine(grid[0, gridSizeZ - 1].gridCenterPosition + cubeTop + new Vector3(-cellSize / 2f, 0f, -cellSize / 2f),
                            grid[gridSizeX - 1, gridSizeZ - 1].gridCenterPosition + cubeTop + new Vector3(cellSize / 2f, 0f, -cellSize / 2f));
                //Draw the right most line
                Gizmos.DrawLine(
                            grid[gridSizeX -1, 0].gridCenterPosition + cubeTop + new Vector3(-cellSize / 2f, 0f, -cellSize / 2f),
                            grid[gridSizeX - 1, gridSizeZ - 1].gridCenterPosition + cubeTop + new Vector3(-cellSize / 2f, 0f, cellSize / 2f));
            }
        }
    }
}

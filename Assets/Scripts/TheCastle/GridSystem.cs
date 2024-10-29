using Assets.Scripts.TheCastle;
using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheCastle
{
    //The grid system take the bottom right corner as its root
    public class GridSystem : MonoBehaviour
    {
        private const float cellSize = 1f;

        public static GridSystem Instance { get; private set; }

        public event EventHandler<OnGridObjectChangeEventArgs> OnGridObjectChange;
        public class OnGridObjectChangeEventArgs : EventArgs
        {
            public int x;
            public int z;
        }

        [Header("Turrets")]
        [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList = null;
        private PlacedObjectTypeSO placedObjectTypeSO;

        [Header("Player")]
        [SerializeField] private Transform playerPosition;

        public GridCell[,] grid { get; private set; }

        private BoxCollider boxCollider;
        private Vector3 cubeSize;
        private int gridSizeX;
        private int gridSizeZ;
        private Vector3 cubeTop;



        private void Awake()
        {
            Instance = this;

            placedObjectTypeSO = placedObjectTypeSOList[0]; //null
        }

        private void Start()
        {
            boxCollider = GetComponent<BoxCollider>();


            GenerateGrid();
        }

        private void Update()
        {
            //place holder input
            if (Input.GetKeyDown(KeyCode.P) && placedObjectTypeSO != null)
            {
                var placedGridCell = GetXZ(playerPosition.position, out int x, out int z);

                bool canBuild = true;
                if (placedGridCell == null || !placedGridCell.CanBuild())
                {
                    canBuild = false;
                }

                if (canBuild)
                {
                    PlacedObject_Done placedObject = PlacedObject_Done.Create(GetWorldPosition(x, z), placedObjectTypeSO);

                    placedGridCell.SetPlacedObject(placedObject);
                } else
                {
                    UtilsClass.CreateWorldTextPopup(null, "Cannot build here!", playerPosition.position, 15, Color.red, playerPosition.position + new Vector3(0, 10), 1f);
                }
            }

            //place holder input
            if (Input.GetKeyDown(KeyCode.O))
            {
                var clearedGridCell = GetXZ(playerPosition.position, out int x, out int z);
                if(clearedGridCell != null)
                {
                    PlacedObject_Done placedObject = clearedGridCell.GetPlacedObject();
                    if(placedObject != null)
                    {
                        placedObject.DestroySelf();

                        clearedGridCell.ClearPlacedObject();
                    }
                }
            }
        }

        private void GenerateGrid()
        {
            cubeSize = Vector3.Scale(boxCollider.size, transform.localScale);
            gridSizeX = Mathf.FloorToInt(cubeSize.x / cellSize);
            gridSizeZ = Mathf.FloorToInt(cubeSize.z / cellSize);
            cubeTop = transform.position + boxCollider.center + new Vector3(0, cubeSize.y / 2f, 0);

            /*Debug.Log("Grid length and width = " + gridSizeX + ", " + gridSizeZ + ", Cube Size = " + cubeSize);*/
            grid = new GridCell[gridSizeX, gridSizeZ];

            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 centerPosition = new Vector3(
                        x * cellSize + cellSize / 2f - cubeSize.x / 2f,
                        0,
                        z * cellSize + cellSize / 2f - cubeSize.z / 2f);
                    /*Debug.Log("at: " + x + "," + z + " => center position = " + centerPosition);*/

                    grid[x, z] = new GridCell(centerPosition, this, x, z);
                }
            }

            //debugging
            bool showDebug = true;
            if (showDebug)
            {
                TextMesh[,] debugTextArray = new TextMesh[gridSizeX, gridSizeZ];
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int z = 0; z < grid.GetLength(1); z++)
                    {
                        debugTextArray[x, z] = UtilsClass.CreateWorldText(grid[x, z].ToString(), null, GetWorldPosition(x, z) + new Vector3(-cellSize, 1, 0) * 0.5f, 5, Color.white);

                    }
                }

                OnGridObjectChange += (object sender, OnGridObjectChangeEventArgs eventArgs) =>
                {
                    debugTextArray[eventArgs.x, eventArgs.z].text = grid[eventArgs.x, eventArgs.z]?.ToString();
                };

            }
        }
        //return the x, y of the cell and the value of the cell
        private GridCell GetXZ(Vector3 worldPosition, out int x, out int z)
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

        public void TriggerGridObjectChanged(int x, int z)
        {
            OnGridObjectChange?.Invoke(this, new OnGridObjectChangeEventArgs { x = x, z = z });
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            return grid[x, z].gridCenterPosition + cubeTop;
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

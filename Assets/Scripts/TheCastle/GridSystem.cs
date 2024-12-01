using Assets.Scripts.TheCastle;
using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

namespace TheCastle
{
    [RequireComponent(typeof(ServerTurretManager))]
    //The grid system take the bottom right corner as its root
    public class GridSystem : NetworkBehaviour
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
        [SerializeField] private Vector3 turretScalling = new Vector3(0.4f, 0.4f, 0.4f);
        private PlacedObjectTypeSO placedObjectTypeSO;
        private ServerTurretManager serverTurretManager;

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
            serverTurretManager = GetComponent<ServerTurretManager>();

            GenerateGrid();
        }

        private void Update()
        {
            //9 - 1
            //10 - 1
            //11 - 1
            //place holder input
            if (Input.GetKeyDown(KeyCode.P) && placedObjectTypeSO != null)
            {
                Vector3[] placePositions = new Vector3[3];
                placePositions[0] = GetWorldPosition(9, 1);
                placePositions[1] = GetWorldPosition(10, 1);
                placePositions[2] = GetWorldPosition(11, 1);
                TurretType[] turrets = new TurretType[3];
                turrets[0] = TurretType.MachineGun;
                turrets[1] = TurretType.SingleTarget;
                turrets[2] = TurretType.Artillery;

                for (int i = 0; i < placePositions.Length; i++)
                {
                    GridObjectPlaceServerRpc(placePositions[i], turrets[i]);
                }
            }

            //place holder input
            if (Input.GetKeyDown(KeyCode.O))
            {
                Vector3[] placePositions = new Vector3[3];
                placePositions[0] = GetWorldPosition(9, 1);
                placePositions[1] = GetWorldPosition(10, 1);
                placePositions[2] = GetWorldPosition(11, 1);

                for (int i = 0; i < placePositions.Length; i++)
                {

                    var clearedGridCell = GetXZ(placePositions[i], out int x, out int z);
                    if (clearedGridCell != null)
                    {
                        PlacedObject_Done placedObject = clearedGridCell.GetPlacedObject();
                        if (placedObject != null)
                        {
                            placedObject.DestroySelf();

                            clearedGridCell.ClearPlacedObject();
                        }
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
            bool showDebug = false;
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

        public Vector3 GetWorldSnappedPosition(Vector3 worldPosition, out bool validPosition)
        {
            validPosition = false;
            Vector3 snappedPosition = worldPosition;
            var gridCell = GetXZ(worldPosition, out int x, out int z);
            //Debug.Log($"X:{x}, Z:{z}");
            if (gridCell != null)
            {
                if (gridCell.CanBuild())
                {
                    snappedPosition = GetWorldPosition(x, z);
                    validPosition = true;
                }
            }

            return snappedPosition;
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

        public void GridObjectPlace(Vector3 placePosition, PlacedObjectTypeSO objectToPlace)
        {
            GridCell placedGridCell = GetXZ(placePosition, out int x, out int z);

            bool canBuild = true;
            if (placedGridCell == null || !placedGridCell.CanBuild())
            {
                canBuild = false;
            }

            if (canBuild)
            {
                PlacedObject_Done placedObject = PlacedObject_Done.Create(GetWorldPosition(x, z), objectToPlace, turretScalling);
                placedGridCell.SetPlacedObject(placedObject);
            }
            else
            {
                Vector3 gridCellGlobalPosition = GetWorldPosition(x, z);
                UtilsClass.CreateWorldTextPopup(null, "Cannot build here!", gridCellGlobalPosition, 15, Color.red, gridCellGlobalPosition + new Vector3(0, 10), 1f);
            }
        }

        [Rpc(SendTo.Server)]
        public void GridObjectPlaceServerRpc(Vector3 placePosition, TurretType turretType)
        {
            GridCell placedGridCell = GetXZ(placePosition, out int x, out int z);
            PlacedObjectTypeSO objectToPlace = serverTurretManager.GetTurretPf(turretType);

            string errMessage = "";
            bool isValidTurret = true;
            bool canBuild = true;
            if (placedGridCell == null || !placedGridCell.CanBuild())
            {
                canBuild = false;
                errMessage += " Invalid build position";
            }
            if (objectToPlace == null)
            {
                isValidTurret = false;
                errMessage += " Invalid turret";
            }
            if (canBuild && isValidTurret)
            {
                PlacedObject_Done placedObject = PlacedObject_Done.Create(GetWorldPosition(x, z), objectToPlace, turretScalling);
                placedGridCell.SetPlacedObject(placedObject);
            }
            else
            {
                Vector3 gridCellGlobalPosition = GetWorldPosition(x, z);
                UtilsClass.CreateWorldTextPopup(null, errMessage, gridCellGlobalPosition, 15, Color.red, gridCellGlobalPosition + new Vector3(0, 10), 1f);
            }
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            return grid[x, z].gridCenterPosition + cubeTop;
        }

        public PlacedObjectTypeSO GetPlacedObjectTypeSO()
        {
            return placedObjectTypeSO;
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

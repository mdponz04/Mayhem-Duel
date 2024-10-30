using System.Collections;
using System.Collections.Generic;
using TheCastle;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public GridSystem gridSystem; // Reference to your GridSystem component
    public float x = -9.76f;
    public float z = 44.36f;

    private void Start()
    {
        // Reference to cubeTop for positioning on the grid
        Vector3 cubeTop = gridSystem.cubeTop;
        Debug.Log(cubeTop);
        // Define some test positions relative to cubeTop
        Vector3[] testPositions = {
            cubeTop,  // Center of cube top
            new Vector3(x , cubeTop.y, z)
        };

        // Loop through test positions and log results
        foreach (var pos in testPositions)
        {
            if (gridSystem.GetXY(pos, out int x, out int z) != null)
            {
                Debug.Log($"World Position: {pos} -> Grid Cell: ({x}, {z})");
            }
            else
            {
                Debug.Log($"World Position: {pos} is out of grid bounds.");
            }
        }
    }
}

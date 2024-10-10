using UnityEngine;

public class Grid3D
{
    private int width;
    private int height;
    private int depth;
    private float cellSize;
    private int[,,] gridArray;
    private TextMesh[,,] debugTextArray;
    private Vector3 originPosition;
    private Vector3 offset = Vector3.zero;

    public Grid3D(int width, int height, int depth, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, height, depth];
        debugTextArray = new TextMesh[width, height, depth];
        offset = new Vector3(-0.5f, 0.5f, 0.5f);
    }

    public Vector3 GetWorldPosition(int x, int y, int z, Vector3 localScale)
    {
        offset.y = localScale.y / 2;
        return new Vector3(x, y, z) * cellSize + originPosition + offset;
    }

    public void GetXYZ(Vector3 worldPosition, out int x, out int y, out int z)
    {
        Vector3 adjustedPosition = worldPosition - originPosition - offset;
        x = Mathf.FloorToInt(adjustedPosition.x / cellSize);
        y = Mathf.FloorToInt(adjustedPosition.y / cellSize);
        z = Mathf.FloorToInt(adjustedPosition.z / cellSize);
    }

    public void SetValue(int x, int y, int z, int value)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth)
        {
            gridArray[x, y, z] = value;
            UpdateDebugText(x, y, z);
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        int x, y, z;
        GetXYZ(worldPosition, out x, out y, out z);
        SetValue(x, y, z, value);
    }

    public int GetValue(int x, int y, int z)
    {
        if (x >= 0 && y >= 0 && z >= 0 && x < width && y < height && z < depth)
        {
            return gridArray[x, y, z];
        }
        else
        {
            return 0;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        int x, y, z;
        GetXYZ(worldPosition, out x, out y, out z);
        return GetValue(x, y, z);
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    private void UpdateDebugText(int x, int y, int z)
    {
        if (debugTextArray[x, y, z] != null)
        {
            debugTextArray[x, y, z].text = gridArray[x, y, z].ToString();
        }
    }

}
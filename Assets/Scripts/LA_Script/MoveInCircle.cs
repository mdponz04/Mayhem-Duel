using UnityEngine;

public class MoveInCircle : MonoBehaviour
{
    public float Radius = 5f;
    public float Speed = 10f;
    public Transform ObjToMove;

    private float angle = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        angle += Time.deltaTime * Speed;

        float x = Mathf.Cos(angle) * Radius + transform.position.x;
        float z = Mathf.Sin(angle) * Radius + +transform.position.z;

        ObjToMove.transform.position = new Vector3(x, transform.position.y, z);
    }
}

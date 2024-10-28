using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public static void Create(Vector3 position, Transform direction, float speed)
    {
        Transform bulleTransform = Instantiate(GameAssets.i.pfBullet, position, Quaternion.identity);
        Bullet bullet = bulleTransform.GetComponent<Bullet>();
        bullet.SetUp(direction, speed);
    }

    protected void SetUp(Transform direction, float speed)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction.forward * speed;
    }

}

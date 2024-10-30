using TheDamage;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamageSource
{
    private Rigidbody rb;
    private DamageDealer damageDealer;
    public float attackDamage = 10;
    public static void Create(Vector3 position, Transform direction, float speed)
    {
        LayerMask layerMask = LayerMask.GetMask("Damageable");
        Transform bulleTransform = Instantiate(GameAssets.i.pfBullet, position, Quaternion.identity);
        Bullet bullet = bulleTransform.GetComponent<Bullet>();
        bullet.SetUp(direction, speed);
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    protected void SetUp(Transform direction, float speed)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction.forward * speed;
        damageDealer = GetComponent<DamageDealer>();
        damageDealer.SetUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damageable"))
        {
            Vulnerable damageable = other.GetComponent<Vulnerable>();
            if (damageable != null)
            {
                Debug.Log("Bullet hit " + damageable);
                damageDealer.DoDamage(damageable);
            }
            Destroy(gameObject);
        }
    }
}

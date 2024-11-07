using TheDamage;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamageSource
{
    private Rigidbody rb;
    private DamageDealer damageDealer;
    public float attackDamage = 10;
    public static void Create(Vector3 position, Transform direction, float speed, float damage)
    {
        Transform bulleTransform = Instantiate(GameAssets.i.pfBullet, position, Quaternion.identity);
        Bullet bullet = bulleTransform.GetComponent<Bullet>();
        bullet.SetUp(direction, speed, damage);
    }

    public float GetAttackDamage()
    {
        return attackDamage;
    }

    protected void SetUp(Transform direction, float speed, float damage)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction.forward * speed;
        attackDamage = damage;
        damageDealer = GetComponent<DamageDealer>();
        damageDealer.SetUp();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
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

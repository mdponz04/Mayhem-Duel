using Unity.Netcode;
using TheDamage;
using UnityEngine;
using System.Collections;

public class Bullet : NetworkBehaviour, IDamageSource
{
    private Rigidbody rb;
    private DamageDealer damageDealer;
    public NetworkVariable<float> attackDamage = new NetworkVariable<float>(10f);

    // Factory method for creating bullets
    public static void Create(Vector3 position, Transform direction, float speed, float damage)
    {
        Transform bulleTransform = Instantiate(GameAssets.i.pfBullet, position, Quaternion.identity);
        Bullet bullet = bulleTransform.GetComponent<Bullet>();

        // Request server to spawn and set up the bullet
        if (bullet.IsOwner)
        {
            bullet.SetUpServerRpc(direction.forward, speed, damage);
        }
    }

    [ServerRpc(RequireOwnership = true)]
    protected void SetUpServerRpc(Vector3 directionForward, float speed, float damage)
    {
        // Server-side setup and synchronization
        attackDamage.Value = damage;
        SetUpClientRpc(directionForward, speed, damage);
    }

    [ClientRpc]
    private void SetUpClientRpc(Vector3 directionForward, float speed, float damage)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = directionForward * speed;

        damageDealer = GetComponent<DamageDealer>();
        damageDealer.SetUp();
    }

    public float GetAttackDamage()
    {
        return attackDamage.Value;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only the server should handle damage
        if (!IsServer) return;

        if (other.CompareTag("Enemy"))
        {
            Vulnerable damageable = other.GetComponent<Vulnerable>();
            if (damageable != null)
            {
                Debug.Log("Bullet hit " + damageable);
                damageDealer.DoDamage(damageable);
            }

            // Despawn the bullet across the network
            GetComponent<NetworkObject>().Despawn();
        }
    }
}

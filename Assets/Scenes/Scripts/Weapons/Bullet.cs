using Unity.Netcode;
using TheDamage;
using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class Bullet : NetworkBehaviour, IDamageSource
{
    private Rigidbody rb;
    private DamageDealer damageDealer;
    public NetworkVariable<float> attackDamage = new NetworkVariable<float>(10f);

    [ServerRpc(RequireOwnership = false)]
    public static void CreateServerRpc(Vector3 position, Transform direction, float speed, float damage)
    {
        // Server-side bullet creation
        Transform bulletTransform = Instantiate(GameAssets.i.pfBullet, position, Quaternion.identity);
        Bullet bullet = bulletTransform.GetComponent<Bullet>();
        bullet.SetUpClientRpc(direction.forward, speed, damage);
        bulletTransform.GetComponent<NetworkObject>().Spawn();
    }

    [ClientRpc]
    private void SetUpClientRpc(Vector3 directionForward, float speed, float damage)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = directionForward * speed;
        attackDamage.Value = damage;
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

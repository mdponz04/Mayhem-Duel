using System.Collections;
using TheDamage;
using Unity.Netcode;
using UnityEngine;

public class Rocket : Bullet
{
    [SerializeField] private GameObject explosionEffect;

    // Factory method for creating rockets
    public new static void Create(Vector3 position, Transform direction, float speed, float damage)
    {
        Transform rocketTransform = Instantiate(GameAssets.i.pfRocket, position, Quaternion.identity);
        Rocket rocket = rocketTransform.GetComponent<Rocket>();

        // Request server to spawn and set up the rocket
        if (rocket.IsOwner)
        {
            rocket.SetUpServerRpc(direction.forward, speed, damage);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only the server should handle explosion
        if (!IsServer) return;

        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        // Disable rendering for all clients
        ExplodeClientRpc();

        // Optional: Implement area damage
        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Vulnerable damageable))
            {
                // Potential network-safe damage method
                DealAreaDamageClientRpc(collider.transform.position);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Despawn the rocket across the network
        GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    private void ExplodeClientRpc()
    {
        GetComponent<MeshRenderer>().enabled = false;
        explosionEffect.SetActive(true);
    }

    [ClientRpc]
    private void DealAreaDamageClientRpc(Vector3 position)
    {
        // You might implement a more robust damage system here
        Debug.Log($"Dealing area damage at {position}");
    }
}

using Unity.Netcode;
using UnityEngine;

namespace TheDamage
{
    public class DamageDealer : NetworkBehaviour
    {
        private IDamageSource damageSource;
        private float attackDamage;

        public void SetUp()
        {
            damageSource = GetComponent<IDamageSource>();
            attackDamage = damageSource.GetAttackDamage();
        }

        // Do damage in a single-player setup
        public void DoDamage(Vulnerable opponent)
        {
            opponent.TakeDamge(attackDamage);
        }

        // Try to do damage in multiplayer (works for both server and client)
        public void TryDoDamage(Vulnerable opponent)
        {
            /*Debug.Log(this.name + " attempts to deal: " + attackDamage);*/

            if (IsServer)
            {
                // Server processes the damage directly
                opponent.TakeDamge(attackDamage);
            }
            else
            {
                // Client sends a request to the server to apply damage
                TryDoDamageServerRpc(opponent.NetworkObjectId);
            }
        }

        // ServerRpc to process damage on the server
        [ServerRpc(RequireOwnership = false)]
        private void TryDoDamageServerRpc(ulong opponentNetworkId)
        {
            NetworkObject opponentObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[opponentNetworkId];
            if (opponentObject != null && opponentObject.TryGetComponent(out Vulnerable opponent))
            {
                /*Debug.Log(this.name + " deals: " + attackDamage + " to " + opponent.name);*/
                opponent.TakeDamge(attackDamage);
            }
            else
            {
                Debug.LogWarning("Opponent not found or does not have a Vulnerable component.");
            }
        }
    }
}


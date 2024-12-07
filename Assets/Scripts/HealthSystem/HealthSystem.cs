using System;
using Unity.Netcode;
using UnityEngine;

namespace TheHealth
{
    public class HealthSystem : NetworkBehaviour
    {
        public event EventHandler OnHealthChange;
        public event EventHandler OnDeath;
        public class OnHealthChangeEventArgs : EventArgs
        {
            public float currentHealth { get; set; }
        }
        [SerializeField] public float maxHealth { get; set; }
        [SerializeField] public float currentHealth { get; set; }
        //Network variable to handle health change
        public NetworkVariable<float> networkHealth = new NetworkVariable<float>(
            writePerm: NetworkVariableWritePermission.Server,
            readPerm: NetworkVariableReadPermission.Everyone
        );
        public void SetUp(float maxHealth)
        {
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
            // Set initial health only on the server

        }
        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                networkHealth.Value = maxHealth; // Ensure networkHealth is in sync on spawn
            }
        }
        // Method to take damage
        public void TakeDamage(float amount)
        {
            currentHealth -= amount;

            // Sync with networked variable
            networkHealth.Value = currentHealth;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
            OnHealthChange?.Invoke(this, new OnHealthChangeEventArgs { currentHealth = currentHealth });
            Debug.Log("Remaining health of " + this.name + " = " + currentHealth);
        }

        // Method to heal
        public void Heal(float amount)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            OnHealthChange?.Invoke(this, new OnHealthChangeEventArgs { currentHealth = currentHealth });
            Debug.Log("Remaining health = " + currentHealth);
        }

        // Check if entity is alive
        public bool IsAlive()
        {
            return currentHealth > 0;
        }

        // Trigger death behavior
        private void Die()
        {
            Debug.Log(gameObject.name + " has died!");
            OnDeath?.Invoke(this, EventArgs.Empty);

        }

        public float GetNetworkHealth()
        {
            return networkHealth.Value; // Return the synchronized health
        }
    }
}



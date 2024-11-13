using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheHealth
{
    public class HealthSystem : MonoBehaviour
    {
        public event EventHandler OnHealthChange;
        public event EventHandler OnDeath;
        public class OnHealthChangeEventArgs : EventArgs
        {
            public float currentHealth { get; set; }
        }
        public float maxHealth { get; set; }
        public float currentHealth { get; set; }

        public void SetUp(float maxHealth)
        {
            this.maxHealth = maxHealth;
            currentHealth = maxHealth;
        }
        // Method to take damage
        public void TakeDamage(float amount)
        {
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
            OnHealthChange?.Invoke(this, new OnHealthChangeEventArgs {currentHealth = currentHealth});
            Debug.Log("Remaining health = " + currentHealth);
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
            // Add your death behavior here, like destroying the object, triggering animations, etc.
        }
    }
}



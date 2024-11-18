using System.Collections;
using System.Collections.Generic;
using TheHealth;
using UnityEngine;

namespace TheDamage
{
    public class Vulnerable : MonoBehaviour
    {
        private HealthSystem healthSystem;

        private void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
        }
        
        public virtual void TakeDamge(float amount)
        {
            healthSystem.TakeDamage(amount);
        }

    }
}


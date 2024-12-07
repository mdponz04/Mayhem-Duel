using TheHealth;
using UnityEngine;

namespace TheCastle
{
    public class Castle : MonoBehaviour
    {
        private float maxHealth = 1000f;
        private BoxCollider hitbox;
        private HealthSystem healthSystem;
        
        private void Awake()
        {
            hitbox = GetComponent<BoxCollider>();
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.SetUp(maxHealth);

        }
    }
}

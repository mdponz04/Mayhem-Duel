using TheHealth;
using Unity.VisualScripting;
using UnityEngine;

namespace TheCastle
{
    public class Castle : MonoBehaviour
    {
        private float maxHealth = 1000f;
        private BoxCollider hitbox;
        private HealthSystem healthSystem;
        public static Castle Singleton { get; private set; }
        private void Awake()
        {
            Singleton = this;
            hitbox = GetComponent<BoxCollider>();
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.SetUp(maxHealth);
            healthSystem.OnHealthChange += OnBeHit;
        }

        private void OnBeHit(object sender, System.EventArgs e)
        {
            NetworkManagerUI.Singleton.UpdateCastleHealthUI(healthSystem.currentHealth);
        }

    }
}

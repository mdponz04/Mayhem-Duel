using TheHealth;
using Unity.Netcode;

namespace TheDamage
{
    public class Vulnerable : NetworkBehaviour
    {
        private HealthSystem healthSystem;
        private void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
        }
        public virtual void TakeDamge(float amount)
        {
            if (IsServer && healthSystem != null)
            {
                healthSystem.TakeDamage(amount);
            }
        }
    }
}


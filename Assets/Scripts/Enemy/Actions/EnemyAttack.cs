using TheDamage;
using UnityEngine;

namespace TheEnemy
{
    public class EnemyAttack
    {
        private float attackCooldown;
        private float nextTimeAttack;
        private float attackRange;
        private LayerMask layerMask;
        private DamageDealer damageDealer;

        public EnemyAttack(float attackCooldown, float attackRange, LayerMask layerMask, DamageDealer damageDealer)
        {
            this.attackCooldown = attackCooldown;
            this.attackRange = attackRange;
            this.layerMask = layerMask;
            this.damageDealer = damageDealer;
        }

        public void HandleAttack(Vector3 position)
        {
            if (Time.time >= nextTimeAttack)
            {
                float heightOffset = 1f;
                Vector3 sphereCenter = position + new Vector3(0f, heightOffset, 0f);
                Collider[] attackHits = Physics.OverlapSphere(sphereCenter, attackRange, layerMask);

                foreach (Collider hit in attackHits)
                {
                    if (hit.CompareTag("Player") || hit.CompareTag("Damageable"))
                    {
                        Vulnerable vulnerableComponent = hit.GetComponent<Vulnerable>();
                        if (vulnerableComponent != null)
                        {
                            damageDealer.DoDamage(vulnerableComponent);
                            nextTimeAttack = Time.time + attackCooldown;
                        }
                    }
                }
            }
        }
    }
}


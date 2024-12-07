using TheDamage;
using UnityEngine;

namespace TheEnemy
{
    public class RangeEnemy : EnemyBase
    {
        [SerializeField] private Projectile projectile;

        protected override void Start()
        {
            UpdateStats();
            layerMask = LayerMask.GetMask("Player", "Damageable");
            pathfinding = GetComponent<Pathfinding>();
            damageDealer = GetComponent<DamageDealer>();
            damageDealer.SetUp();
            base.Start();
            enemyAttack.OnAttack += EnemyAttack_OnAttackProjectile;
        }
        private void EnemyAttack_OnAttackProjectile(object sender, EnemyAttack.OnAttackEventArgs e)
        {
            projectile.HandleShootingProjectile(e.targetPosition);
            enemyVFX.PlaySphereProjectileEffect();
            /*Debug.Log("Shoot projectile to : " + e.targetPosition);*/
        }

        private void UpdateStats()
        {
            attackDamage = 5f;
            maxHealth = 50f;
            attackCooldown = 3f;
            attackRange = 10f;
            nextTimeAttack = 0f;
        }
    }
}


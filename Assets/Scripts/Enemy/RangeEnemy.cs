using TheDamage;
using Unity.Netcode;
using UnityEngine;

namespace TheEnemy
{
    public class RangeEnemy : EnemyBase
    {
        [SerializeField] private Projectile projectile;

        protected override void Start()
        {
            UpdateStats();
            layerMask = LayerMask.GetMask("Player", "Damageable", "Network Player Collider");
            pathfinding = GetComponent<Pathfinding>();
            damageDealer = GetComponent<DamageDealer>();
            damageDealer.SetUp();
            base.Start();
            enemyAttack.OnAttack += OnAttackProjectile;
            healthSystem.OnDeath += OnDeathStopVFX;
        }

        private void OnDeathStopVFX(object sender, System.EventArgs e)
        {
            enemyVFX.StopAllEffectsRangeEnemy();
            StopVFXClientRpc();
        }
        [ClientRpc]
        private void StopVFXClientRpc()
        {
            enemyVFX.StopAllEffectsRangeEnemy();
        }
        private void OnAttackProjectile(object sender, EnemyAttack.OnAttackEventArgs e)
        {
            
            projectile.HandleShootingProjectile(e.targetPosition);
            enemyVFX.PlaySphereProjectileEffect();
            TriggerNormalAttackClientRpc(e.targetPosition);
        }

        [ClientRpc]
        private void TriggerNormalAttackClientRpc(Vector3 targetPosition)
        {
            // Clients mirror death event
            if (!IsServer) // Prevent double-execution on server
            {
                projectile.HandleShootingProjectile(targetPosition);
                enemyVFX.PlaySphereProjectileEffect();
            }
        }
        private void UpdateStats()
        {
            attackDamage = 5f;
            maxHealth = 50f;
            attackCooldown = 3f;
            attackRange = 10f;
            nextTimeAttack = 0f;
            aggroRange = 15f;
        }
    }
}


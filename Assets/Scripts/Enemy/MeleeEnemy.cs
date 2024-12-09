using TheDamage;
using Unity.Netcode;
using UnityEngine;

namespace TheEnemy
{
    public class MeleeEnemy : EnemyBase
    {
        protected override void Start()
        {
            UpdateStats();


            layerMask = LayerMask.GetMask("Player", "Damageable");

            pathfinding = GetComponent<Pathfinding>();
            damageDealer = GetComponent<DamageDealer>();
            damageDealer.SetUp();
            base.Start();
            enemyAttack.OnAttack += OnNormalAttack;
            healthSystem.OnDeath += OnDeathStopVFX;
        }

        private void OnDeathStopVFX(object sender, System.EventArgs e)
        {
            enemyVFX.StopAllEffectMeleeEnemy();
        }

        private void OnNormalAttack(object sender, EnemyAttack.OnAttackEventArgs e)
        {
            enemyVFX.PlayClawAttackEffect();
            TriggerNormalAttackClientRpc();
        }
        [ClientRpc]
        private void TriggerNormalAttackClientRpc()
        {
            // Clients mirror death event
            if (!IsServer) // Prevent double-execution on server
            {
                enemyVFX.PlayClawAttackEffect();
            }
        }
        private void UpdateStats()
        {
            attackDamage = 10f;
            maxHealth = 100f;
            attackCooldown = 2f;
            attackRange = 2f;
            nextTimeAttack = 0f;
        }
    }
}


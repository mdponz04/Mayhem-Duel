using System.Collections;
using System.Collections.Generic;
using TheDamage;
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
        }

        private void UpdateStats()
        {
            attackDamage = 10f;
            maxHealth = 100f;
            attackCooldown = 1.5f;
            attackRange = 2f;
            nextTimeAttack = 0f;
        }
    }
}


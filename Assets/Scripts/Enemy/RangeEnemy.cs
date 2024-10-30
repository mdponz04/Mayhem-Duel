using System.Collections;
using System.Collections.Generic;
using TheDamage;
using UnityEngine;

namespace TheEnemy
{
    public class RangeEnemy : EnemyBase
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
            attackDamage = 5f;
            maxHealth = 50f;
            attackCooldown = 2f;
            attackRange = 10f;
            nextTimeAttack = 0f;
        }
    }
}


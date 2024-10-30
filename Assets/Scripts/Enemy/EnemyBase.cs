using System.Collections;
using System.Collections.Generic;
using TheDamage;
using Unity.VisualScripting;
using UnityEngine;

namespace TheEnemy
{
    public class EnemyBase : MonoBehaviour, IDamageSource
    {
        private EnemyAttack enemyAttack;
        private EnemyMovement enemyMoving;

        public LayerMask layerMask { get; set; }
        public float maxHealth { get; set; }
        public float attackDamage { get; set; }
        public float attackCooldown { get; set; }
        public float nextTimeAttack { get; set; }
        public Pathfinding pathfinding { get; set; }
        public DamageDealer damageDealer { get; set; }
        public float attackRange { get; set; }

        [SerializeField] private SphereCollider aggroRange;
        protected virtual void Start()
        {
            enemyAttack = new EnemyAttack(attackCooldown, attackRange, layerMask, damageDealer);
            enemyMoving = new EnemyMovement(pathfinding);
        }
        protected virtual void Update()
        {
            if (enemyMoving != null && enemyAttack != null)
            {
                enemyMoving.HandleMoving(enemyMoving.target, attackRange, transform);
                enemyAttack.HandleAttack(transform.position);
            }
            else
            {
                Debug.LogError("There is enemy moving or attack is null!!!");
            }
        }

        //Chase player if player enter the aggro range
        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                enemyMoving.SetTarget(other);
            }
        }
        //Chase castle after player or damageable things out of the aggro range
        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                enemyMoving.SetTarget(null);
            }
        }
         
        float IDamageSource.GetAttackDamage() => attackDamage;
    }
}


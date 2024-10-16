using System.Collections;
using System.Collections.Generic;
using TheDamage;
using UnityEngine;

namespace TheEnemy
{
    public class RangeEnemy : MonoBehaviour, IDamageSource
    {
        private LayerMask layerMask;
        public float maxHealth { get; private set; }
        public float attackDamage { get; private set; }
        public float attackCooldown = 5f;
        private float nextTimeAttack = 0f;
        private DamageDealer damageDealer;

        [SerializeField] private float attackRange = 8f;
        [SerializeField] private SphereCollider aggroRange;
        private Pathfinding pathfinding;
        private void Start()
        {
            maxHealth = 50f;
            attackDamage = 6f;

            layerMask = LayerMask.GetMask("Player", "Damageable");

            pathfinding = GetComponent<Pathfinding>();
            damageDealer = GetComponent<DamageDealer>();
            damageDealer.SetUp();
        }
        private void HandleAttack()
        {
            if (Time.time >= nextTimeAttack)
            {
                Collider[] attackHits = Physics.OverlapSphere(transform.position, attackRange, layerMask);

                foreach (Collider hit in attackHits)
                {
                    if (hit.CompareTag("Player") || hit.CompareTag("Damageable"))
                    {
                        // Perform attack
                        damageDealer.DoDamage(hit.GetComponent<Vulnerable>());

                        nextTimeAttack = Time.time + attackCooldown;  // Set next attack time
                    }
                }
            }
        }
        //Chase player if player enter the aggro range
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                pathfinding.ChaseTarget(other.transform);
            }
        }
        //Chase player if player in the aggro range but out of the attack range, stop moving when player in attack range
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                float distanceToTarget = Vector3.Distance(transform.position, other.transform.position);

                if (distanceToTarget <= attackRange)
                {
                    pathfinding.StopMoving();  // Stop chasing and attack
                    HandleAttack();
                }
                else
                {
                    pathfinding.ChaseTarget(other.transform);  // Keep chasing
                }
            }
        }
        //Change to charge to the castle if player not in aggro range
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                pathfinding.ChaseTarget(null);
            }
        }

        float IDamageSource.GetAttackDamage() => attackDamage;
    }
}


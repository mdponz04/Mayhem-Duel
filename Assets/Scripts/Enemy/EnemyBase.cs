using System.Collections;
using System.Collections.Generic;
using TheDamage;
using UnityEngine;

namespace TheEnemy
{
    public class EnemyBase : MonoBehaviour, IDamageSource
    {
        public LayerMask layerMask { get; set; }
        public float maxHealth { get; set; }
        public float attackDamage { get; set; }
        public float attackCooldown { get; set; }
        public float nextTimeAttack { get; set; }
        public Pathfinding pathfinding { get; set; }
        public DamageDealer damageDealer { get; set; }
        public float attackRange { get; set; }
        [SerializeField] private SphereCollider aggroRange;
        private Collider other;
        public void Update()
        {
            HandleMoving();
        }
        //Stop moving if attack and vice versa
        public void HandleMoving()
        {
            if(other != null)
            {
                float bufferDistance = 0.5f;
                float distanceToTarget = GetEdgeDistance(other, attackRange);
                Debug.Log("distance to target = " + distanceToTarget);
                if (distanceToTarget <= 0f)
                {
                    pathfinding.StopMoving();
                    HandleAttack();
                }
                else if (distanceToTarget <= bufferDistance)
                {
                    pathfinding.ResumeMoving();
                }
            }
        }
        //Create a sphere cast to check if there is collider inside and damage it
        public void HandleAttack()
        {
            if (Time.time >= nextTimeAttack)
            {
                float heightOffset = 1f;
                Vector3 sphereCenter = transform.position + new Vector3(0f, heightOffset, 0f);
                Collider[] attackHits = Physics.OverlapSphere(sphereCenter, attackRange, layerMask);

                foreach (Collider hit in attackHits)
                {
                    if (hit.CompareTag("Player") || hit.CompareTag("Damageable"))
                    {
                        // Perform attack
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
        //Chase player if player enter the aggro range
        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                pathfinding.ChaseTarget(other.transform);
                this.other = other;
            }
        }
        //Chase castle after player or damageable things out of the aggro range
        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                pathfinding.ChaseTarget(null);
                this.other = null;
            }
        }
        //Use to calculate the distance between my collider edge to other collider edge
        private float GetEdgeDistance(Collider other, float myColliderRadius)
        {
            // Get the closest point on the other collider to your character
            Vector3 closestPointOnOther = other.ClosestPoint(transform.position);

            // Calculate the distance from the character's position to the closest point on the other collider
            float distanceToClosestPoint = Vector3.Distance(transform.position, closestPointOnOther);

            // Subtract the radius of your character's collider to get edge-to-edge distance
            float distanceEdgeToEdge = distanceToClosestPoint - myColliderRadius;

            return distanceEdgeToEdge;
        }
        float IDamageSource.GetAttackDamage() => attackDamage;
    }
}


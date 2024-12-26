using System.Collections;
using System.Collections.Generic;
using TheDamage;
using TheHealth;
using Unity.Netcode;
using UnityEngine;

namespace TheEnemy
{
    public class EnemyBase : NetworkBehaviour, IDamageSource
    {
        public EnemyAttack enemyAttack { get; private set; }
        public EnemyMove enemyMove { get; private set; }
        public EnemyVisual enemyVisual { get; private set; }
        public EnemyVFX enemyVFX { get; private set; }
        public LayerMask layerMask { get; set; }
        public float maxHealth { get; set; }
        public float attackDamage { get; set; }
        public float attackCooldown { get; set; }
        public float nextTimeAttack { get; set; }
        public Pathfinding pathfinding { get; set; }
        public DamageDealer damageDealer { get; set; }
        public float attackRange { get; set; }
        /*[SerializeField] private SphereCollider aggroRange;*/
        public HealthSystem healthSystem { get; private set; }
        private List<Collider> targetsInAggro = new();
        public float aggroRange {  get; set; }
        protected virtual void Start()
        {
            enemyAttack = new EnemyAttack(attackCooldown, attackRange, layerMask, damageDealer);
            enemyMove = new EnemyMove(pathfinding);
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.SetUp(maxHealth);
            enemyVisual = GetComponentInChildren<EnemyVisual>();
            enemyVFX = GetComponentInChildren<EnemyVFX>();

            // Events trigger on both server and clients
            enemyAttack.OnAttack += OnNormalAttack;
            healthSystem.OnHealthChange += OnBeHit;
            healthSystem.OnDeath += OnDeath;
        }
        private void OnDeath(object sender, System.EventArgs e)
        {
            if (IsServer)
            {
                // Server-side logic
                enemyMove.StopMovingInstantly();
                enemyAttack.StopAttackingInstantly();
                DisableCollider();
                TriggerDeathClientRpc(); // Notify all clients
            }

            // Local (client and server) logic
            enemyVisual.TriggerDied();
            enemyVFX.StopAllEffects();
            StartCoroutine(DelayOnDeath());
        }
        private void DisableCollider()
        {
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        [ClientRpc]
        private void TriggerDeathClientRpc()
        {
            // Clients mirror death event
            if (!IsServer) // Prevent double-execution on server
            {
                enemyVisual.TriggerDied();
            }
        }

        private IEnumerator DelayOnDeath()
        {
            yield return new WaitForSeconds(10f);
            if (IsServer)
            {
                Destroy(gameObject);
            }
        }

        private void OnBeHit(object sender, System.EventArgs e)
        {
            if (!healthSystem.IsAlive()) return;

            if (IsServer)
            {
                // Server-side logic
                enemyAttack.StopAttackingInstantly();
                TriggerHitClientRpc(); // Notify all clients
            }

            // Local (client and server) logic
            enemyVisual.TriggerHit();
            enemyVFX.PlayBloodBurstEffect();
            StartCoroutine(DelayOnContinueAtk());
        }
        private IEnumerator DelayOnContinueAtk()
        {
            yield return new WaitForSeconds(0.2f);
            if (IsServer)
            {
                enemyAttack.ResumeAttacking();
            }
        }
        [ClientRpc]
        private void TriggerHitClientRpc()
        {
            // Clients mirror hit event
            if (!IsServer) // Prevent double-execution on server
            {
                enemyVisual.TriggerHit();
                enemyVFX.PlayBloodBurstEffect();
            }
        }

        private void OnNormalAttack(object sender, EnemyAttack.OnAttackEventArgs e)
        {
            if (!healthSystem.IsAlive()) return;
            if (IsServer)
            {
                // Server-side logic
                TriggerAttackClientRpc(); // Notify all clients
            }

            // Local (client and server) logic
            enemyVisual.TriggerNormalAttack();
        }

        [ClientRpc]
        private void TriggerAttackClientRpc()
        {
            // Clients mirror attack event
            if (!IsServer) // Prevent double-execution on server
            {
                enemyVisual.TriggerNormalAttack();
            }
        }

        protected void Update()
        {
            if (IsServer)
            {
                // Server handles core logic
                if (enemyMove != null && enemyVisual != null)
                {
                    enemyMove.HandleMoving(enemyMove.target, attackRange, transform);
                    enemyVisual.HandleMoving(enemyMove.IsMoving());
                }
                if (enemyAttack != null)
                {
                    enemyAttack.HandleAttack(transform.position);
                }
                CheckTargetsInAggro();
            }
        }
        //Using trigger collision to handle aggro
        /*protected void OnTriggerEnter(Collider other)
        {
            if (!IsServer) return;
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                targetsInAggro.Add(other);
                Debug.Log("The targets in aggro: " + other.name);
                if (enemyMove.target == null)
                {
                    enemyMove.SetTarget(targetsInAggro[0]);
                }
                else if (other.CompareTag("Player") && enemyMove.target.CompareTag("Damageable"))
                {
                    enemyMove.SetTarget(targetsInAggro[0]);
                }
            }
        }
        protected void OnTriggerExit(Collider other)
        {
            if (!IsServer) return;
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                targetsInAggro.Remove(other);
                Debug.Log("The targets out aggro: " + other.name);
                if (enemyMove.target != null)
                {
                    if (targetsInAggro.Count > 0 && targetsInAggro[0] != null)
                    {
                        enemyMove.SetTarget(targetsInAggro[0]);
                    }
                    else
                    {
                        enemyMove.SetTarget(null);
                    }
                }
            }
        }*/

        protected void CheckTargetsInAggro()
        {
            // Find all colliders in the aggro range that match the layer mask
            Collider[] hits = Physics.OverlapSphere(transform.position, aggroRange, layerMask);
            targetsInAggro.Clear();

            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player") || hit.CompareTag("Damageable"))
                {
                    targetsInAggro.Add(hit);
                }
            }

            // Handle target selection
            if (targetsInAggro.Count > 0)
            {
                if (enemyMove.target == null ||
                    (targetsInAggro[0].CompareTag("Player") && enemyMove.target.CompareTag("Damageable")))
                {
                    enemyMove.SetTarget(targetsInAggro[0]);
                }
            }
            else
            {
                targetsInAggro.Clear();
                enemyMove.SetTarget(null);
            }
        }

        float IDamageSource.GetAttackDamage() => attackDamage;
    }
}


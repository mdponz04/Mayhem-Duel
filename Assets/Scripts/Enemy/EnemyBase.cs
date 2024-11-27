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

        [SerializeField] private SphereCollider aggroRange;
        public HealthSystem healthSystem { get; set; }
        protected virtual void Start()
        {
            enemyAttack = new EnemyAttack(attackCooldown, attackRange, layerMask, damageDealer);
            enemyMove = new EnemyMove(pathfinding);
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.SetUp(maxHealth);
            enemyVisual = GetComponentInChildren<EnemyVisual>();
            enemyVFX = GetComponentInChildren<EnemyVFX>();
            enemyAttack.OnAttack += OnNormalAttack;
            healthSystem.OnHealthChange += OnBeHit;
            healthSystem.OnDeath += OnDeath;
        }

        private void OnDeath(object sender, System.EventArgs e)
        {
            enemyMove.StopMovingInstantly();
            enemyAttack.StopAttackingInstantly();
            enemyVisual.TriggerDied();
            StartCoroutine(DelayOnDeath());
        }
        private IEnumerator DelayOnDeath()
        {
            yield return new WaitForSeconds(10f);
            Destroy(this.gameObject);
        }
        private IEnumerator DelayResumeAttack()
        {
            yield return new WaitForSeconds(0.5f);
            enemyAttack.ResumeAttacking();
        }
        private void OnBeHit(object sender, System.EventArgs e)
        {
            enemyAttack.StopAttackingInstantly();
            enemyVisual.TriggerHit();
            enemyVFX.PlayBloodBurstEffect();
            StartCoroutine(DelayResumeAttack());
        }

        private void OnNormalAttack(object sender, EnemyAttack.OnAttackEventArgs e)
        {
            enemyVisual.TriggerNormalAttack();
        }
        protected void Update()
        {
            // Move and attack behavior handled per frame
            if (enemyMove != null && enemyVisual != null)
            {
                enemyMove.HandleMoving(enemyMove.target, attackRange, transform);
                enemyVisual.HandleMoving(enemyMove.IsMoving());
            }
            if (enemyAttack != null)
            {
                enemyAttack.HandleAttack(transform.position);
            }
        }

        //Chase player if player enters the aggro range
        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                Debug.Log("Thing on trigger enter: " + other.name);
                enemyMove.SetTarget(other);
            }
        }


        //Stop chasing when player or damageable object exits the aggro range
        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Damageable"))
            {
                Debug.Log("Thing on trigger enter: " + other.name);
                enemyMove.SetTarget(null);
            }
        }

        float IDamageSource.GetAttackDamage() => attackDamage;
    }
}


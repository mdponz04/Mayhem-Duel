using System;
using TheDamage;
using UnityEngine;

namespace TheEnemy
{
    public class EnemyAttack
    {
        public event EventHandler<OnAttackEventArgs> OnAttack;
        public class OnAttackEventArgs : EventArgs
        {
            public Vector3 targetPosition { get; set; }
            public OnAttackEventArgs(Vector3 targetPosition)
            {
                this.targetPosition = targetPosition;
            }
        }
        private float attackCooldown;
        private float nextTimeAttack;
        private float attackRange;
        private LayerMask layerMask;
        private DamageDealer damageDealer;
        private bool isAttackingEnabled = true;

        public EnemyAttack(float attackCooldown, float attackRange, LayerMask layerMask, DamageDealer damageDealer)
        {
            this.attackCooldown = attackCooldown;
            this.attackRange = attackRange;
            this.layerMask = layerMask;
            this.damageDealer = damageDealer;
        }

        public void HandleAttack(Vector3 currentPosition)
        {
            if (!isAttackingEnabled) return;

            if (Time.time >= nextTimeAttack)
            {
                float heightOffset = 1f;
                Vector3 sphereCenter = currentPosition + new Vector3(0f, heightOffset, 0f);
                Collider[] attackHits = Physics.OverlapSphere(sphereCenter, attackRange, layerMask);
                if(attackHits.Length > 0)
                {
                    if (attackHits[0].CompareTag("Player") || attackHits[0].CompareTag("Damageable"))
                    {
                        Vulnerable vulnerableComponent = attackHits[0].GetComponent<Vulnerable>();
                        if (vulnerableComponent != null)
                        {
                            damageDealer.TryDoDamage(vulnerableComponent);
                            OnAttack?.Invoke(this, new OnAttackEventArgs(attackHits[0].transform.position));
                            nextTimeAttack = Time.time + attackCooldown;
                        }
                    }
                }
            }
        }
        public void StopAttackingInstantly()
        {
            isAttackingEnabled = false;
        }
        public void ResumeAttacking()
        {
            isAttackingEnabled = true;
        }
    }
}


using System;
using TheDamage;
using UnityEngine;

public class EnemyAttack
{
    public event EventHandler OnAttack;
    public float attackCooldown {  get; private set; }
    public float nextTimeAttack {  get; private set; }
    public float attackRange { get; private set; }
    public LayerMask layerMask { get; private set; }
    public DamageDealer damageDealer {  get; private set; }

    public EnemyAttack(float attackCooldown, float attackRange, LayerMask layerMask, DamageDealer damageDealer)
    {
        this.attackCooldown = attackCooldown;
        this.attackRange = attackRange;
        this.layerMask = layerMask;
        this.damageDealer = damageDealer;
    }
    public void HandleAttack(Vector3 position)
    {
        if (Time.time >= nextTimeAttack)
        {
            float heightOffset = 1f;
            Vector3 sphereCenter = position + new Vector3(0f, heightOffset, 0f);
            Collider[] attackHits = Physics.OverlapSphere(sphereCenter, attackRange, layerMask);

            foreach (Collider hit in attackHits)
            {
                if (hit.CompareTag("Player") || hit.CompareTag("Damageable"))
                {
                    Vulnerable vulnerableComponent = hit.GetComponent<Vulnerable>();
                    if (vulnerableComponent != null)
                    {
                        damageDealer.DoDamage(vulnerableComponent);
                        OnAttack?.Invoke(this, EventArgs.Empty);
                        nextTimeAttack = Time.time + attackCooldown;
                    }
                }
            }
        }
    }
}

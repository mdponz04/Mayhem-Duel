using System.Collections;
using System.Collections.Generic;
using TheHealth;
using UnityEngine;

namespace TheDamage
{
    public class DamageDealer : MonoBehaviour
    {
        private IDamageSource damageSource;
        private float attackDamage;

        public void SetUp()
        {
            damageSource = GetComponent<IDamageSource>();
            attackDamage = damageSource.GetAttackDamage();
        }

        public void DoDamage(Vulnerable opponent)
        {
            opponent.TakeDamge(attackDamage);
        }
    }
}


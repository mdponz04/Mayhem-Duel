using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace TheEnemy
{
    public class EnemyVFX : MonoBehaviour
    {
        [SerializeField] private VisualEffect bloodBurstEffect;
        [SerializeField] private List<VisualEffect> leftArmAttackEffect;
        [SerializeField] private List<VisualEffect> rightArmAttackEffect;
        [SerializeField] private VisualEffect sphereProjectileEffect;
        private bool isStoppingClawEffects = false;
        public void PlaySphereProjectileEffect()
        {
            sphereProjectileEffect.Play();
        }
        public void PlayBloodBurstEffect()
        {
            bloodBurstEffect.Play();
        }
        public void PlayClawAttackEffect()
        {
            if (isStoppingClawEffects) return;
            PlayRightArmClawAtkEffect();
            StartCoroutine(DelayOnPlayLeftArmClawAtkEffect());
        }
        private void PlayLeftArmClawAtkEffect()
        {
            foreach (var effect in leftArmAttackEffect)
            {
                effect.Play();
            }
        }
        private void PlayRightArmClawAtkEffect()
        {
            foreach (var effect in rightArmAttackEffect)
            {
                effect.Play();
            }
        }
        private IEnumerator DelayOnPlayLeftArmClawAtkEffect()
        {
            yield return new WaitForSeconds(0.5f);
            PlayLeftArmClawAtkEffect();
            
        }
        public void StopAllEffects()
        {
            // Stop individual effects
            bloodBurstEffect.Stop();
        }
        public void StopAllEffectsRangeEnemy()
        {
            sphereProjectileEffect.Stop();
        }
        public void StopAllEffectMeleeEnemy()
        {
            isStoppingClawEffects = true;
            foreach (var effect in leftArmAttackEffect)
            {
                effect.Stop();
            }
            foreach (var effect in rightArmAttackEffect)
            {
                effect.Stop();
            }
        }
    }
}


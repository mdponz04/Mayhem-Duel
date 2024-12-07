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
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheEnemy
{
    public class EnemyVisual : MonoBehaviour
    {
        private static string IS_RUNNING_PARAMETER = "isRunning";
        private static string NORMAL_ATTACK_PARAMETER = "normalAttack";
        private static string HIT_PARAMETER = "hit";
        private static string DIED_PARAMETER = "died";
        [SerializeField] private Animator animator;

        public void HandleMoving(bool isMoving)
        {
            animator.SetBool(IS_RUNNING_PARAMETER, isMoving);
        }
        public void TriggerNormalAttack()
        {
            animator.SetTrigger(NORMAL_ATTACK_PARAMETER);
        }
        public void TriggerHit()
        {
            animator.SetTrigger(HIT_PARAMETER);
        }
        public void TriggerDied()
        {
            animator.SetTrigger(DIED_PARAMETER);
        }
    }
}


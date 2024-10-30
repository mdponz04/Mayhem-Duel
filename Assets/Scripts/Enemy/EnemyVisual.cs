using System.Collections;
using System.Collections.Generic;
using TheDamage;
using Unity.VisualScripting;
using UnityEngine;

namespace TheEnemy
{ 
    public class EnemyVisual : MonoBehaviour
    {
        private Animator animator;
        //Animator parameters' name
        private static string IS_RUNNING_PARAMETER = "isRunning";
        private static string NORMAL_ATTACK_PARAMETER = "normalAttackTrigger";
        private static string HIT_PARAMETER = "hitTrigger";
        private static string DIED_PARAMETER = "diedTrigger";

        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
        }
        private void HandleRun()
        {
            animator.GetBool(IS_RUNNING_PARAMETER);
        }
    }
}


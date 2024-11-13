using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace TheEnemy
{
    public class EnemyVFX : MonoBehaviour
    {
        [SerializeField]private VisualEffect bloodBurstEffect;

        public void PlayBloodBurstEffect()
        {
            bloodBurstEffect.Play();
        }
    }
}


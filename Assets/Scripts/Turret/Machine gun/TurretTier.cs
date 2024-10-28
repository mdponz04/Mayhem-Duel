using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Turret.Machine_gun
{
    [CreateAssetMenu(fileName="NewTurretTier", menuName = "Turret/TurretTier")]
    public class TurretTier : ScriptableObject
    {
        public string TierName;
        public float FireRate;
        public float FireRange;
        public float Damage;

        public Material TierMaterial;

        
    }
}

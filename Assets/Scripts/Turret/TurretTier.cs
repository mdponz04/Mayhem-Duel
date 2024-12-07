using UnityEngine;

namespace Assets.Scripts.Turret
{
    [CreateAssetMenu(fileName = "NewTurretTier", menuName = "Turret/TurretTier")]
    public class TurretTier : ScriptableObject
    {
        public string TierName;
        public float FireRate;
        public float FireRange;
        public float Damage;

        public Material[] TierMaterial;


    }
}

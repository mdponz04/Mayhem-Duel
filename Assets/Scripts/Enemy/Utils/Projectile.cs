using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheEnemy
{
    public class Projectile : MonoBehaviour
    {
        private float orbitRadius = 1f;
        private float orbitSpeed = 180f;
        private float homingSpeed = 5f;
        [SerializeField] private Transform projectileTransform;

        private Vector3 orbitOffset;      

        private void Start()
        {
            // Set the initial orbit offset perpendicular to the direction toward the target
            orbitOffset = transform.right * orbitRadius;
        }

        public void HandleShootingProjectile(Vector3 targetPosition)
        {
            if (targetPosition == null) return;

            // Move towards the target position with a Lerp speed
            projectileTransform.position = Vector3.MoveTowards(projectileTransform.position, targetPosition, homingSpeed * Time.deltaTime);

            // Rotate orbit offset around the Y-axis (or desired axis) to create orbiting effect
            orbitOffset = Quaternion.Euler(0, orbitSpeed * Time.deltaTime, 0) * orbitOffset;

            // Update position with orbit offset
            projectileTransform.position = targetPosition + orbitOffset;
        }
    }
}


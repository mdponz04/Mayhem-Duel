using System.Collections;
using UnityEngine;

namespace TheEnemy
{
    public class Projectile : MonoBehaviour
    {
        private float projectileSpeed = 20f;
        private Vector3 parentPosition;
        private void Start()
        {
            parentPosition = transform.position;

        }

        public void HandleShootingProjectile(Vector3 targetPosition)
        {
            EnableProjectile();
            if (targetPosition == null) return;


            StartCoroutine(MoveProjectile(targetPosition));
        }
        private IEnumerator MoveProjectile(Vector3 targetPosition)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    projectileSpeed * Time.deltaTime
                );
                yield return null;
            }

            DisableProjectile();
            transform.position = parentPosition;
        }
        private void EnableProjectile()
        {
            transform.gameObject.SetActive(true);
        }
        private void DisableProjectile()
        {
            transform.gameObject.SetActive(false);
        }
    }
}


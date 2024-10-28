using System.Collections;
using System.Collections.Generic;
using TheDamage;
using UnityEditor;
using UnityEngine;

namespace TheEnemy
{
    public class SpawnEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject meleeEnemyPrefab;
        [SerializeField] private GameObject rangeEnemyPrefab;
        private float spawnInterval = 3f;
        [SerializeField] private Transform spawnAreaMin;
        [SerializeField] private Transform spawnAreaMax;
        private Vector3 minSpawnArea;
        private Vector3 maxSpawnArea;
        private void Start()
        {
            minSpawnArea = spawnAreaMin.position;
            maxSpawnArea = spawnAreaMax.position;
            InvokeRepeating(nameof(Spawn), spawnInterval, spawnInterval);
        }
        private void Spawn()
        {
            //Random position to spawn an enemy
            Vector3 randomPosition = new Vector3(
                RandomFloat(minSpawnArea.x, maxSpawnArea.x), 
                0f, 
                RandomFloat(minSpawnArea.z, maxSpawnArea.z));
            //Spawn the enemy in the position
            Instantiate(RandomEnemy(),randomPosition, Quaternion.identity);
        }
        private float RandomFloat(float min, float max)
        {
            return Random.Range(min, max);
        }
        private GameObject RandomEnemy()
        {
            
            if(RandomFloat(1f, 10f) <= 5)
            {
                return meleeEnemyPrefab;
            }
            else
            {
                return rangeEnemyPrefab;
            }
        }
        private void OnDrawGizmos()
        {
            // Draw a wireframe cube in the Editor to show the spawnable area
            Gizmos.color = Color.green;
            Vector3 center = (minSpawnArea + maxSpawnArea) / 2;
            Vector3 size = minSpawnArea - maxSpawnArea;
            Gizmos.DrawWireCube(center, size);
        }
    }
}


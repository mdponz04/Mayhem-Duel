using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TheEnemy
{
    public class SpawnEnemy : MonoBehaviour
    {
        [SerializeField] private GameObject meleeEnemyPrefab;
        [SerializeField] private GameObject rangeEnemyPrefab;
        private float spawnInterval = 30f;
        private float spawnStartTime = 3f;
        [SerializeField] private Transform spawnAreaMin;
        [SerializeField] private Transform spawnAreaMax;
        private Vector3 minSpawnArea;
        private Vector3 maxSpawnArea;
        private List<GameObject> spawnedEnemyList;
        private void Start()
        {
            minSpawnArea = spawnAreaMin.position;
            maxSpawnArea = spawnAreaMax.position;
            InvokeRepeating(nameof(Spawn), spawnStartTime, spawnInterval);
            gameObject.SetActive(false);
        }
        private void Spawn()
        {
            int waveOrder = (int)Mathf.Ceil(Time.time / spawnInterval);
            if (waveOrder <= 5)
            {
                
                RandomPositionSpawn(rangeEnemyPrefab, waveOrder * 2);
                RandomPositionSpawn(meleeEnemyPrefab, waveOrder * 3);
            }
        }
        //Spawn prefab in random positions for prefab amount
        private void RandomPositionSpawn(GameObject prefab, int prefabAmount)
        {
            for (int x = 1; x <= prefabAmount; x++)
            {
                //Random position to spawn an enemy
                Vector3 randomPosition = new Vector3(
                    RandomFloat(minSpawnArea.x, maxSpawnArea.x),
                    0f,
                    RandomFloat(minSpawnArea.z, maxSpawnArea.z));
                //Spawn the enemy in the position
                GameObject enemyInstance = Instantiate(prefab, randomPosition, Quaternion.identity);
                spawnedEnemyList.Add(enemyInstance);
            }
        }
        private float RandomFloat(float min, float max)
        {
            return Random.Range(min, max);
        }
        
    }
}


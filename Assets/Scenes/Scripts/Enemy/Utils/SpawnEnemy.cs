using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TheEnemy
{
    public class SpawnEnemy : NetworkBehaviour
    {
        public event EventHandler OnSpawn;
        [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
        [SerializeField] private float spawnInterval = 30f;
        [SerializeField] private float spawnStartTime = 3f;
        [SerializeField] private Transform spawnAreaMin;
        [SerializeField] private Transform spawnAreaMax;
        
        private Vector3 minSpawnArea;
        private Vector3 maxSpawnArea;
        
        private void Start()
        {
            minSpawnArea = spawnAreaMin.position;
            maxSpawnArea = spawnAreaMax.position;
        }
        private void Spawn()
        {
            int waveOrder = (int)Mathf.Ceil(Time.time / spawnInterval);
            if (waveOrder <= 5)
            {
                RandomPositionSpawn(enemyPrefabs[0], waveOrder);
                RandomPositionSpawn(enemyPrefabs[1], waveOrder);
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
                NetworkObject enemyNetwork = enemyInstance.GetComponent<NetworkObject>();
                enemyNetwork.Spawn();
            }
            OnSpawn?.Invoke(this, EventArgs.Empty);
        }
        private float RandomFloat(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }
        public void StartSpawning()
        {
            if (IsServer)
            {
                InvokeRepeating(nameof(Spawn), spawnStartTime, spawnInterval);
                Debug.LogWarning($"StartSpawning triggered by {(IsServer ? "Server" : "Client")}");
            }
        }
    }
}


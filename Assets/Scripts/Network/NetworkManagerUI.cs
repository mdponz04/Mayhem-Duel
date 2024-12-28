using System;
using System.Collections.Generic;
using TheCastle;
using TheEnemy;
using TheHealth;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button testButton;
    [SerializeField] private TMP_Text networkEnemyCountText;
    [SerializeField] private TMP_Text castleHealthText;
    [SerializeField] private SpawnEnemy spawnableLand;
    [SerializeField] private HealthSystem castleHealthSystem;
    [SerializeField] private List<GameObject> enemyPrefabs;

    private NetworkVariable<float> enemyCount = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Server, // Only server can modify
        readPerm: NetworkVariableReadPermission.Everyone  // All clients can read
    );
    private NetworkVariable<float> castleHealth = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Server, // Only server can modify
        readPerm: NetworkVariableReadPermission.Everyone  // All clients can read
    );
    private void Awake()
    {
        startButton.onClick.RemoveAllListeners();
        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
        testButton.onClick.AddListener(() =>
        {
            if (IsServer)
            {
                for (int i = 0; i < enemyPrefabs.Count; i++)
                {
                    SpawnPrefabServerRpc(i);
                }
            }
            else
            {
                testButton.gameObject.SetActive(false);
            }
        });
        startButton.onClick.AddListener(() =>
        {
            if (IsServer)
            {
                spawnableLand.StartSpawning();
                startButton.gameObject.SetActive(false);
            }
        });
    }

    private void Start()
    {
        enemyCount.Value = 0f;
        if (IsServer)
        {
            UpdateNetworkEnemyCount();
            NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkObjectCreated;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkObjectDestroyed;
        }
        enemyCount.OnValueChanged += OnEnemyCountChanged;
        castleHealth.OnValueChanged += OnCastleHealthChanged;
        spawnableLand.OnSpawn += OnEnemySpawn;
        castleHealthSystem.OnHealthChange += OnCastleHealthChanged;
    }

    private void OnCastleHealthChanged(object sender, System.EventArgs e)
    {
        UpdateCastleHealthUI(castleHealthSystem.currentHealth);
    }

    private void OnEnemySpawn(object sender, System.EventArgs e)
    {
        UpdateNetworkEnemyCount();
    }

    private void OnEnemyCountChanged(float oldValue, float newValue)
    {
        networkEnemyCountText.text = "Enemy remaining: " + newValue;
    }

    private void OnCastleHealthChanged(float oldValue, float newValue)
    {
        castleHealthText.text = "Castle HP: " + newValue;
    }

    public void UpdateCastleHealthUI(float currentCastleHealth)
    {
        castleHealth.Value = currentCastleHealth;
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnNetworkObjectCreated;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnNetworkObjectDestroyed;
        }
    }

    private void OnNetworkObjectDestroyed(ulong obj)
    {
        UpdateNetworkEnemyCount();
    }

    private void OnNetworkObjectCreated(ulong obj)
    {
        UpdateNetworkEnemyCount();
    }

    public void UpdateNetworkEnemyCount()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            float count = 0;

            foreach (var networkObj in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
            {
                if (networkObj.CompareTag("Enemy"))
                {
                    Debug.Log("Count: " + networkObj.name);
                    count++;
                }
            }
            enemyCount.Value = count;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPrefabServerRpc(int prefabIndex)
    {
        // Ensure the index is valid
        if (prefabIndex < 0 || prefabIndex >= enemyPrefabs.Count)
        {
            Debug.LogWarning("Invalid prefab index passed to SpawnPrefabServerRpc.");
            return;
        }
        
        GameObject enemyInstance = Instantiate(enemyPrefabs[prefabIndex]);
        enemyInstance.transform.position = Vector3.zero;
        NetworkObject networkObject = enemyInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();
        //Debug.Log("Spawned enemy with NetworkObjectId: " + networkObject.NetworkObjectId);

        //Update UI text count
        UpdateNetworkEnemyCount();
    }
}

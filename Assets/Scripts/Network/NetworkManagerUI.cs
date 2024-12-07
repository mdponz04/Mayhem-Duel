using System.Collections.Generic;
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
    [SerializeField] private TMP_Text networkObjectCountText;
    [SerializeField] private GameObject spawnableLand;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private Transform spawnPosition;

    private NetworkVariable<float> enemyCount = new NetworkVariable<float>(
        writePerm: NetworkVariableWritePermission.Server, // Only server can modify
        readPerm: NetworkVariableReadPermission.Everyone  // All clients can read
    );
    private void Awake()
    {
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
            if (IsServer) // Only allow the server to spawn objects
            {
                for (int i = 0; i < enemyPrefabs.Count; i++)
                {
                    SpawnPrefabServerRpc(i); // Pass the index instead of GameObject
                }
            }
            //testButton.gameObject.SetActive(false);
        });
        startButton.onClick.AddListener(() =>
        {
            spawnableLand.SetActive(true);
            startButton.gameObject.SetActive(false);
        });
    }


    private void Start()
    {
        enemyCount.Value = 0f;
        if (IsServer)
        {
            UpdateNetworkObjectCount();
            NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkObjectCreated;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkObjectDestroyed;
        }
        enemyCount.OnValueChanged += OnEnemyCountChanged;
    }
    private void OnEnemyCountChanged(float oldValue, float newValue)
    {
        // Update the UI on all clients when enemyCount changes
        networkObjectCountText.text = "Enemy remaining: " + newValue;
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
        UpdateNetworkObjectCount();
    }

    private void OnNetworkObjectCreated(ulong obj)
    {
        UpdateNetworkObjectCount();
    }

    private void UpdateNetworkObjectCount()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
        {
            // Reset the count before recalculating
            float count = 0;
            // Only count NetworkObjects with the tag "Enemy"
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

        // Instantiate and spawn the selected prefab
        GameObject enemyInstance = Instantiate(enemyPrefabs[prefabIndex]);
        //=========================================
        enemyInstance.transform.position = spawnPosition.position;
        //=========================================
        NetworkObject networkObject = enemyInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();
        //Debug.Log("Spawned enemy with NetworkObjectId: " + networkObject.NetworkObjectId);


        //Update UI text count
        UpdateNetworkObjectCount();
    }
}

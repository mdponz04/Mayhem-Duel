using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class NetworkSpawnTest : NetworkBehaviour
{
    [SerializeField] Transform dummyTranform;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.V))
        {
        Debug.Log("Client id: " + OwnerClientId + " press V");
            SpawnDummyServerRpc();
        }
    }


    [ServerRpc]
    public void SpawnDummyServerRpc()
    {
        if (!IsOwner) return;

        Transform dummy = Instantiate(dummyTranform);
        dummy.GetComponent<NetworkObject>().Spawn();
        Debug.Log("Client id: " + OwnerClientId + " spawn dummy");
    }
}

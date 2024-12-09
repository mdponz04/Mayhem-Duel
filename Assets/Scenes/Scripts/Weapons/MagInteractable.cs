using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;

public class NetworkedMagInteractable : NetworkBehaviour
{
    private NetworkObject networkObject;
    private Mag magComponent;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    [SerializeField] private float attachDistance = 0.1f;

    // Track the current interactor to ensure only the owner can interact
    private NetworkVariable<ulong> currentInteractorClientId = new NetworkVariable<ulong>();

    private void Awake()
    {
        networkObject = GetComponent<NetworkObject>();
        magComponent = GetComponent<Mag>();
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to XR interaction events
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Only allow interaction if the player owns this object
        if (!IsOwner) return;

        // Store the interactor's client ID
        RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        // Only process exit if the player owns this object
        if (!IsOwner) return;

        TryAttachToNearbyGunServerRpc();
    }

    [ServerRpc(RequireOwnership = true)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        // Ensure only the server processes this
        if (!IsServer) return;

        // Detach from current gun
        magComponent.DetachFromCurrentGunServerRpc();

        // Update current interactor
        currentInteractorClientId.Value = clientId;

        // Enable physics
        rb.isKinematic = false;
    }

    [ServerRpc(RequireOwnership = true)]
    private void TryAttachToNearbyGunServerRpc()
    {
        // Ensure only the server processes this
        if (!IsServer) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, attachDistance);
        foreach (Collider collider in colliders)
        {
            Gun nearbyGun = collider.GetComponent<Gun>();
            NetworkObject gunNetworkObject = collider.GetComponent<NetworkObject>();

            if (nearbyGun != null && gunNetworkObject != null &&
                nearbyGun.TryGetComponent(out NetworkObject gunObject) &&
                !nearbyGun.HasMag())
            {
                // Attempt to attach to the gun
                AttachToGunServerRpc(
                    new NetworkObjectReference(gunObject),
                    new NetworkObjectReference(nearbyGun.magAttachPoint)
                );
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = true)]
    private void AttachToGunServerRpc(NetworkObjectReference gunReference, NetworkObjectReference attachPointReference)
    {
        if (!IsServer) return;

        if (gunReference.TryGet(out NetworkObject gunObject))
        {
            Gun gun = gunObject.GetComponent<Gun>();
            if (gun != null && !gun.HasMag())
            {
                // Attach mag to gun
                magComponent.AttachToGunServerRpc(gunReference, attachPointReference);

                // Disable physics
                rb.isKinematic = true;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        // Additional network spawn logic if needed
        base.OnNetworkSpawn();
    }
}
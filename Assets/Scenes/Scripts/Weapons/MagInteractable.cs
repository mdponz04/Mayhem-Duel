using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;

public class MagInteractable :XRGrabInteractable
{
    private NetworkObject networkObject;
    private Mag magComponent;
    private Rigidbody rb;
    [SerializeField] private float attachDistance = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        networkObject = GetComponent<NetworkObject>();
        magComponent = GetComponent<Mag>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        // Ensure only the owner can interact
        if (!IsOwner) return;

        // Detach from current gun
        DetachFromCurrentGunServerRpc();

        // Enable physics
        rb.isKinematic = false;
    }

    [ServerRpc(RequireOwnership = true)]
    private void DetachFromCurrentGunServerRpc()
    {
        if (!IsServer) return;

        magComponent.DetachFromCurrentGunServerRpc();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Ensure only the owner can interact
        if (!IsOwner) return;

        // Try to attach to a nearby gun
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
                    new NetworkObjectReference(gunObject.transform)
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
}
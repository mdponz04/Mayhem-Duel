using UnityEngine;
using Unity.Netcode;

public class Mag : NetworkBehaviour
{
    [SerializeField] private int maxAmmo;

    // Network variables to sync ammo and attachment state
    private NetworkVariable<int> currentAmmo = new NetworkVariable<int>();
    private NetworkVariable<NetworkObjectReference> attachedGunReference = new NetworkVariable<NetworkObjectReference>();

    public int Ammo => currentAmmo.Value;

    private void Awake()
    {
        // Initialize ammo on awake
        if (IsServer)
        {
            currentAmmo.Value = maxAmmo;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AttachToGunServerRpc(NetworkObjectReference gunReference, NetworkObjectReference attachPointReference)
    {
        if (!IsServer) return;

        // Detach from previous gun if attached
        DetachFromCurrentGunServerRpc();

        // Attach to new gun
        if (gunReference.TryGet(out NetworkObject gunObject) &&
            attachPointReference.TryGet(out NetworkObject attachPointObject))
        {
            Transform attachPoint = attachPointObject.transform;

            // Update network transform
            transform.SetParent(attachPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // Update network references
            attachedGunReference.Value = gunReference;

            // Disable physics
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Notify clients about attachment
            AttachToGunClientRpc(gunReference, attachPointReference);
        }
    }

    [ClientRpc]
    private void AttachToGunClientRpc(NetworkObjectReference gunReference, NetworkObjectReference attachPointReference)
    {
        if (IsServer) return;

        if (attachPointReference.TryGet(out NetworkObject attachPointObject) &&
            attachPointReference.TryGet(out NetworkObject gunObject))
        {
            Transform attachPoint = attachPointObject.transform;

            transform.SetParent(attachPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetachFromCurrentGunServerRpc()
    {
        if (!IsServer) return;

        if (attachedGunReference.Value.TryGet(out NetworkObject gunObject))
        {
            // Notify the gun that mag is detached
            Gun attachedGun = gunObject.GetComponent<Gun>();
            if (attachedGun != null)
            {
                attachedGun.DetachMagServerRpc();
            }

            // Reset attachment state
            attachedGunReference.Value = new NetworkObjectReference();

            // Enable physics
            transform.SetParent(null);
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            // Notify clients
            DetachFromGunClientRpc();
        }
    }

    [ClientRpc]
    private void DetachFromGunClientRpc()
    {
        if (IsServer) return;

        transform.SetParent(null);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UseAmmoServerRpc()
    {
        if (!IsServer) return;

        if (currentAmmo.Value > 0)
        {
            currentAmmo.Value--;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReloadServerRpc()
    {
        if (!IsServer) return;

        currentAmmo.Value = maxAmmo;
    }
}
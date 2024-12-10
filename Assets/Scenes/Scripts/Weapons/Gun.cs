using Unity.Netcode;
using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;

// Base Networked Gun Class
public class Gun : NetworkBehaviour
{
    [SerializeField] protected float fireRate = 0.1f;
    [SerializeField] protected bool isAuto = false;
    [SerializeField] protected XRBaseInteractor interactor;
    [SerializeField] protected Transform barrel;
    [SerializeField] protected AudioClip gunShotSound;
    [SerializeField] protected AudioClip emptyClipSound;
    [SerializeField] protected AudioClip reloadClipSound;
    [SerializeField] public GameObject magAttachPoint;
    [SerializeField] protected float attackDamage = 10;

    // Network Variables
    protected NetworkVariable<bool> canFire = new NetworkVariable<bool>(true);
    protected NetworkVariable<bool> isTriggerPressed = new NetworkVariable<bool>(false);
    protected NetworkVariable<NetworkObjectReference> currentMagReference = new NetworkVariable<NetworkObjectReference>();

    protected Coroutine firingCoroutine;

    public Mag Mag { get;  set; }

    protected virtual void Start()
    {

        // Only run attachment logic on the server
        if (IsServer)
        {
            if (currentMagReference.Value.TryGet(out NetworkObject magObject))
            {
                Mag attachedMag = magObject.GetComponent<Mag>();
                if (attachedMag != null)
                {
                    attachedMag.AttachToGunServerRpc(
                     new NetworkObjectReference(this.NetworkObject),
                     new NetworkObjectReference(magAttachPoint)
                 );
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TriggerPressServerRpc()
    {
        Debug.Log("Trigger Pressed");
        // Only the server can change network state
        isTriggerPressed.Value = true;
        if (isAuto)
        {
            StartCoroutine(AutoFire());
        }
        else
        {
            Fire();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TriggerReleaseServerRpc()
    {
        isTriggerPressed.Value = false;
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    protected virtual void Fire()
    {
        // Ensure only server can fire
        if (!IsServer) return;

        if (canFire.Value && currentMagReference.Value.TryGet(out NetworkObject magObject))
        {
            Mag mag = magObject.GetComponent<Mag>();
            if (mag != null && mag.Ammo > 0)
            {
                // Use a ServerRpc to create a networked bullet
                CreateBulletClientRpc(barrel.position, attackDamage);

                // Play sound for all clients
                PlaySoundClientRpc("GunShot");

                // Use ammo on the server
                mag.UseAmmoServerRpc();
                StartCoroutine(FireCooldown());
            }
            else
            {
                PlaySoundClientRpc("EmptyClip");
                DetachMagServerRpc();
            }
        }
    }

    [ClientRpc]
    protected void CreateBulletClientRpc(Vector3 barrelPosition, float damage)
    {
        // Create bullet on all clients
        Bullet.Create(barrelPosition, barrel, 100, damage);
    }

    protected virtual IEnumerator AutoFire()
    {
        while (isTriggerPressed.Value && Mag!=null)
        {
            Debug.Log("Auto Firing");
            if (Mag != null && Mag.Ammo > 0)
            {
                Fire();
                yield return new WaitForSeconds(fireRate);
            }
            else
            {
                PlaySoundClientRpc("EmptyClip");
                DetachMagServerRpc();
                break;
            }
        }
    }

    protected IEnumerator FireCooldown()
    {
        canFire.Value = false;
        yield return new WaitForSeconds(fireRate);
        canFire.Value = true;
    }

    [ClientRpc]
    protected virtual void PlaySoundClientRpc(string soundName)
    {
        switch (soundName)
        {
            case "GunShot":
                AudioSource.PlayClipAtPoint(gunShotSound, transform.position);
                break;
            case "EmptyClip":
                AudioSource.PlayClipAtPoint(emptyClipSound, transform.position);
                break;
            case "Reload":
                AudioSource.PlayClipAtPoint(reloadClipSound, transform.position);
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void AttachMagServerRpc(NetworkObjectReference magReference)
    {
        if (!IsServer) return;

        // Detach existing mag if present
        if (currentMagReference.Value.TryGet(out NetworkObject existingMagObject))
        {
            DetachMagServerRpc();
        }

        // Attach new mag
        if (magReference.TryGet(out NetworkObject newMagObject))
        {
            Mag newMag = newMagObject.GetComponent<Mag>();
            if (newMag != null)
            {
                Debug.Log("Attaching mag to gun");
                Mag = newMag;
                currentMagReference.Value = magReference;

                // Directly update the mag's attached gun reference without calling back
                newMag.attachedGunReference.Value = new NetworkObjectReference(this.NetworkObject);

                PlaySoundClientRpc("Reload");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DetachMagServerRpc()
    {
        if (!IsServer) return;

        if (currentMagReference.Value.TryGet(out NetworkObject magObject))
        {
            Mag mag = magObject.GetComponent<Mag>();
            if (mag != null)
            {
                mag.DetachFromCurrentGunServerRpc();
            }
            currentMagReference.Value = new NetworkObjectReference();
        }
    }

    public bool HasMag()
    {
        return currentMagReference.Value.TryGet(out NetworkObject magObject);
    }
}

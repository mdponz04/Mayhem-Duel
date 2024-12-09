using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;
using Unity.Collections;

public class Gun : NetworkBehaviour
{
    [SerializeField] protected float fireRate = 0.1f;
    [SerializeField] protected bool isAuto = false;
    [SerializeField] protected XRBaseInteractor interactor;
    [SerializeField] protected Transform barrel;
    [SerializeField] protected AudioClip gunShotSound;
    [SerializeField] protected AudioClip emptyClipSound;
    [SerializeField] protected AudioClip reloadClipSound;
    [SerializeField] protected Transform magAttachPoint;
    [SerializeField] protected float attackDamage = 10;

    // Network Variables
    protected NetworkVariable<bool> canFire = new NetworkVariable<bool>(true);
    protected NetworkVariable<bool> isTriggerPressed = new NetworkVariable<bool>(false);
    protected NetworkVariable<NetworkObjectReference> currentMagReference = new NetworkVariable<NetworkObjectReference>();

    protected Coroutine firingCoroutine;
    public Mag Mag { get; private set; }

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
                    attachedMag.AttachToGunServerRpc(magAttachPoint);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TriggerPressServerRpc()
    {
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
    public virtual void TriggerReleaseServerRpc()
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
                mag.UseAmmo();
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
    protected virtual void CreateBulletClientRpc(Vector3 barrelPosition, float damage)
    {
        // Create bullet on all clients
        Bullet.Create(barrelPosition, barrel, 100, damage);
    }

    protected virtual IEnumerator AutoFire()
    {
        while (isTriggerPressed.Value && currentMagReference.Value.TryGet(out NetworkObject magObject))
        {
            Mag mag = magObject.GetComponent<Mag>();
            if (mag != null && mag.Ammo > 0)
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
                currentMagReference.Value = magReference;
                newMag.AttachToGun(magAttachPoint);
                PlaySoundClientRpc("Reload");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void DetachMagServerRpc()
    {
        if (!IsServer) return;

        if (currentMagReference.Value.TryGet(out NetworkObject magObject))
        {
            Mag mag = magObject.GetComponent<Mag>();
            if (mag != null)
            {
                mag.DetachFromGun();
            }
            currentMagReference.Value = new NetworkObjectReference();
        }
    }

    public virtual bool HasMag()
    {
        return currentMagReference.Value.IsValid();
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Gun : MonoBehaviour
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

    protected bool canFire = true;
    protected bool isTriggerPressed = false;
    protected Coroutine firingCoroutine;
    [SerializeField] public Mag currentMag;
    public Mag Mag => currentMag;

    protected virtual void Start()
    {
        if (currentMag != null)
        {
            currentMag.AttachToGun(magAttachPoint);
        }
    }

    public void TriggerPress()
    {
        isTriggerPressed = true;
        if (isAuto)
        {
            firingCoroutine = StartCoroutine(AutoFire());
        }
        else
        {
            Fire();
        }
    }

    public void TriggerRelease()
    {
        isTriggerPressed = false;
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
    }

    protected virtual void Fire()
    {
        if (canFire && currentMag != null && currentMag.Ammo > 0)
        {
            Bullet.Create(barrel.position, barrel, 100, attackDamage);
            PlaySound("GunShot");
            currentMag.UseAmmo();
            StartCoroutine(FireCooldown());
        }
        else if (currentMag == null || currentMag.Ammo <= 0)
        {
            PlaySound("EmptyClip");
            DetachMag();
        }
    }

    protected virtual IEnumerator AutoFire()
    {
        while (isTriggerPressed && (currentMag != null && currentMag.Ammo > 0))
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }
        if (currentMag == null || currentMag.Ammo <= 0)
        {
            PlaySound("EmptyClip");
            DetachMag();
        }
    }

    protected IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    protected virtual void PlaySound(string soundName)
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

    public virtual void AttachMag(Mag mag)
    {
        if (currentMag != null)
        {
            DetachMag();
        }
        currentMag = mag;
        mag.AttachToGun(magAttachPoint);
        PlaySound("Reload");
    }

    public void DetachMag()
    {
        if (currentMag != null)
        {
            currentMag.DetachFromGun();
            currentMag = null;
        }
    }
    public bool HasMag()
    {
        return currentMag != null;
    }
}

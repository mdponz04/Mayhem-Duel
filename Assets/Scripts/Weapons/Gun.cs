using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Gun : MonoBehaviour
{
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private bool isAuto = false;
    [SerializeField] private XRBaseInteractor interactor;
    [SerializeField] private Transform barrel;
    [SerializeField] private AudioClip gunShotSound;
    [SerializeField] private AudioClip emptyClipSound;
    [SerializeField] private AudioClip reloadClipSound;
    [SerializeField] private Transform magAttachPoint;

    private bool canFire = true;
    private bool isTriggerPressed = false;
    private Coroutine firingCoroutine;
    [SerializeField] private Mag currentMag;
    public Mag Mag => currentMag;

    private void Start()
    {
        if(currentMag != null)
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

    private void Fire()
    {
        if (canFire && currentMag != null && currentMag.Ammo > 0)
        {
            Bullet.CreateBullet(barrel.position, barrel, 10);
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

    private IEnumerator AutoFire()
    {
        while (isTriggerPressed && (currentMag != null && currentMag.Ammo > 0))
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }
        if(currentMag == null || currentMag.Ammo <= 0)
        {
            PlaySound("EmptyClip");
            DetachMag();                   
        }
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }

    private void PlaySound(string soundName)
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

    public void AttachMag(Mag mag)
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

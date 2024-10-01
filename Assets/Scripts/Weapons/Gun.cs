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

    private bool canFire = true;
    private bool isTriggerPressed = false;
    private Coroutine firingCoroutine;

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
        if (canFire)
        {
            Bullet.CreateBullet(barrel.position, barrel, 10);
            StartCoroutine(FireCooldown());
        }
    }

    private IEnumerator AutoFire()
    {
        while (isTriggerPressed)
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}

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

    public void Fire()
    {
        if (canFire)
        {
           Bullet.CreateBullet(barrel.position, barrel , 20);
        }
    }
    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}

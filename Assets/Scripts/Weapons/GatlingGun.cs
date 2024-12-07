using System.Collections;
using UnityEngine;

public class GatlingGun : Gun
{
    [SerializeField] private Transform gatlingBarrel;


    protected override void Fire()
    {
        if (canFire && currentMag != null && currentMag.Ammo > 0)
        {
            Bullet.Create(barrel.position, barrel, 75, attackDamage);
            PlaySound("GunShot");
            currentMag.UseAmmo();
        }
        else if (currentMag == null || currentMag.Ammo <= 0)
        {
            PlaySound("EmptyClip");
            DetachMag();
            Debug.Log("Mag detached");
        }
    }

    protected override IEnumerator AutoFire()
    {
        int fireCount = 0;
        StartCoroutine(SpinWarmUp());
        yield return new WaitForSeconds(0.5f);
        while (isTriggerPressed)
        {
            Fire();
            fireCount++;
            yield return new WaitForSeconds(fireRate);
            if(fireCount >= 20)
            {
                fireRate = Mathf.Lerp(0.1f, 0.05f, 0.5f);
            }
        }
        fireRate = 0.1f;
    }

    private IEnumerator SpinWarmUp()
    {
        float spinTime = 0.5f;
        float elapsedTime = 0;
        while (isTriggerPressed)
        {
            float spinSpeed = Mathf.Lerp(0, 360f / spinTime * 2,elapsedTime);
            gatlingBarrel.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}

using System.Collections;
using UnityEngine;

public class GatlingGun : Gun
{
    [SerializeField] private Transform gatlingBarrel;

    protected override void Fire()
    {
        if (canFire && currentMag != null && currentMag.Ammo > 0)
        {
            Bullet.CreateBullet(barrel.position, barrel, 10);
            PlaySound("GunShot");
            currentMag.UseAmmo();
        }
    }

    protected override IEnumerator AutoFire()
    {
        StartCoroutine(SpinWarmUp());
        yield return new WaitForSeconds(0.5f);
        while (isTriggerPressed && (currentMag != null && currentMag.Ammo > 0))
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }
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

    private void Update()
    {
    }
}

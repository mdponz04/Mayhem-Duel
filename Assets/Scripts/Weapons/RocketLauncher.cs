using UnityEngine;

public class RocketLauncher : Gun
{
    
    protected override void Fire()
    {
        if (canFire && currentMag != null && currentMag.Ammo > 0)
        {
            Rocket.Create(barrel.position, barrel, 10);
            PlaySound("GunShot");
            currentMag.UseAmmo();
            StartCoroutine(FireCooldown());
        }
    }
}

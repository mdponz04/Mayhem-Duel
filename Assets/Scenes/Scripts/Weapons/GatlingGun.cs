using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class GatlingGun : Gun
{
    [SerializeField] private Transform gatlingBarrel;

    protected override void Fire()
    {
        if (!IsServer) return;

        if (canFire.Value && currentMagReference.Value.TryGet(out NetworkObject magObject))
        {
            Mag mag = magObject.GetComponent<Mag>();
            if (mag != null && mag.Ammo > 0)
            {
                CreateBulletClientRpc(barrel.position, attackDamage);
                PlaySoundClientRpc("GunShot");
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

    protected override IEnumerator AutoFire()
    {
        int fireCount = 0;
        StartCoroutine(SpinWarmUp());
        yield return new WaitForSeconds(0.5f);
        while (isTriggerPressed.Value)
        {
            Fire();
            fireCount++;
            yield return new WaitForSeconds(fireRate);
            if (fireCount >= 20)
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
        while (isTriggerPressed.Value)
        {
            float spinSpeed = Mathf.Lerp(0, 360f / spinTime * 2, elapsedTime);
            gatlingBarrel.Rotate(Vector3.forward, spinSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

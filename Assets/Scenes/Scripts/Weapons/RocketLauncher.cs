using Unity.Netcode;
using UnityEngine;

public class RocketLauncher : Gun
{
    protected override void Fire()
    {
        if (!IsServer) return;

        if (canFire.Value &&
            currentMagReference.Value.TryGet(out NetworkObject magObject) &&
            magObject.GetComponent<Mag>().Ammo > 0)
        {
            Mag mag = magObject.GetComponent<Mag>();

            // Create rocket on server and replicate to clients
            CreateRocketClientRpc(barrel.position, attackDamage);

            PlaySoundClientRpc("GunShot");
            mag.UseAmmoServerRpc();
            StartCoroutine(FireCooldown());
        }
    }

    [ClientRpc]
    private void CreateRocketClientRpc(Vector3 position, float damage)
    {
        Rocket.Create(position, barrel, 10, damage);
    }
}

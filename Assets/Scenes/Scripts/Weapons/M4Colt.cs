using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;

public class M4Colt : Rifle
{
    [SerializeField] private int rocketMag;
    [SerializeField] private Transform rocketBarrel;
    private NetworkVariable<bool> rocketReady = new NetworkVariable<bool>(false);

    protected override void Start()
    {
        base.Start();
        rocketMag = 3;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActiveRocketServerRpc()
    {
        rocketReady.Value = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactiveRocketServerRpc()
    {
        rocketReady.Value = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void LaunchRocketServerRpc()
    {
        if (rocketReady.Value && rocketMag > 0)
        {
            CreateRocketClientRpc(rocketBarrel.position, attackDamage);
            PlaySoundClientRpc("GunShot");
            rocketMag--;
            StartCoroutine(RocketCoolDown());
        }
    }
    
    [ClientRpc]
    private void CreateRocketClientRpc(Vector3 barrelPosition, float damage)
    {
        Rocket.Create(barrelPosition, rocketBarrel, 10, damage);
    }
    
    private IEnumerator RocketCoolDown()
    {
        rocketReady.Value = false;
        yield return new WaitForSeconds(1f);
        rocketReady.Value = true;
    }
}
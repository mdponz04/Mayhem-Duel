using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class M4Colt : Rifle
{
    [SerializeField] private int rocketMag;
    [SerializeField] private Transform rocketBarrel;
    private bool rocketReady = false;

    protected override void Start()
    {
        base.Start();
        rocketMag = 3;
    }

    public void ActiveRocket()

    {
        rocketReady = true;
    }

    public void DeactiveRocket()
    {
        rocketReady = false;
    }
    public void LaunchRocket()
    {
        if(rocketReady && rocketMag> 0)
        {
            Rocket.Create(rocketBarrel.position, rocketBarrel, 10);
            PlaySound("GunShot");
            rocketMag--;
            StartCoroutine(RocketCoolDown());
        }
    }    
    
    private IEnumerator RocketCoolDown()
    {
        rocketReady = false;
        yield return new WaitForSeconds(1f);
        rocketReady = true;
    }



}

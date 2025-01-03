﻿using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ModifyMachineGun : TurretBase
{
    [Header("Machine gun part")]
    // Gameobjects need to control rotation and aiming
    [SerializeField] Transform go_baseRotation;
    [SerializeField] Transform go_GunBody;
    [SerializeField] Transform go_barrel;
    [SerializeField] float barrleRotationDamping = 5f;
    [SerializeField] float headAimOffSet = -0.5f;

    [Space(5)]
    [Header("Particle system")]
    // Particle system for the muzzel flash
    [SerializeField] ParticleSystem muzzelFlash;
    [SerializeField] ParticleSystem bulletShell;
    [SerializeField] ParticleSystem bulletTraser;
    [SerializeField] Transform bulletImpact;
    List<ParticleCollisionEvent> bulletCollisionEvent;

    float barrelRotationSpeed;
    float currentRotationSpeed;

    protected override void Start()
    {
        base.Start();
        bulletCollisionEvent = new List<ParticleCollisionEvent>();
    }

    private void Awake()
    {
        turretBase = go_baseRotation;
        turretHead = go_GunBody;
    }

    public void OnParticleCollision(GameObject other)
    {
        int numberOfCollisionEvent = bulletTraser.GetCollisionEvents(other, bulletCollisionEvent);
        int i = 0;
        while (i < numberOfCollisionEvent)
        {
            DoDamage(bulletCollisionEvent[i].colliderComponent.gameObject, parameters.damage);
            BulletImpactFVX(bulletCollisionEvent[i].intersection, bulletImpact);
            i++;
        }
    }
    protected override void Shooting()
    {
        base.Shooting();
        if (parameters.canFire)
        {
            if (!muzzelFlash.isPlaying)
            {
                //muzzelFlash.Play();
                TurnOnParticleVisualClientRpc();
            }
            ShotVFX();
        }
        else
        {
            // stop the particle system
            if (muzzelFlash.isPlaying)
            {
                //muzzelFlash.Stop();
                TurnOffParticleVisualClientRpc();
            }
        }

        //RefreshCurrentTarget();
        CancelInvoke();
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected void TurnOnParticleVisualClientRpc()
    {
        muzzelFlash.Play();
    }
    [Rpc(SendTo.ClientsAndHost)]
    protected void TurnOffParticleVisualClientRpc()
    {
        muzzelFlash.Stop();
    }
    protected override void Aiming()
    {

        if (targeting.currentTarget == null)
        {
            return;
        }
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);
        // if can fire turret activates
        if (parameters.canFire)
        {
            // start rotation
            currentRotationSpeed = barrelRotationSpeed;

            // aim at enemy
            RotateTurretBaseTorwardTarget(go_baseRotation, targeting.currentTarget.transform, targeting.aimingSpeed);
            RotateTurretHeadAimAtTarget(go_GunBody, targeting.currentTarget.transform, targeting.aimingSpeed, headAimOffSet);
        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, barrleRotationDamping * Time.deltaTime);
        }
    }

    #region Upgrade
    public override void TierChange()

    {
        base.TierChange();
        barrelRotationSpeed = parameters.FireRate / 3 * 360;

        var emission = muzzelFlash.emission;
        emission.rateOverTime = parameters.FireRate;
        emission = bulletShell.emission;
        emission.rateOverTime = parameters.FireRate;
        emission = bulletTraser.emission;
        emission.rateOverTime = parameters.FireRate;

    }
    #endregion

}
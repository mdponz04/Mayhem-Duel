using Assets.Scripts.Turret;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class ModifyMachineGun : TurretBase
{
    [Header("Machine gun part")]
    // Gameobjects need to control rotation and aiming
    [SerializeField] Transform go_baseRotation;
    [SerializeField] Transform go_GunBody;
    [SerializeField] Transform go_barrel;
    [SerializeField] float barrleRotationDamping = 5f;

    [Space(5)]
    [Header("Particle system")]
    // Particle system for the muzzel flash
    [SerializeField] ParticleSystem muzzelFlash;
    [SerializeField] ParticleSystem bulletShell;
    [SerializeField] ParticleSystem bulletTraser;
    List<ParticleCollisionEvent> bulletCollisionEvent;

    float barrelRotationSpeed;
    float currentRotationSpeed;

    protected override void Start()
    {
        base.Start();
        bulletCollisionEvent = new List<ParticleCollisionEvent>();
    }

    public void OnParticleCollision(GameObject other)
    {
        int numberOfCollisionEvent = bulletTraser.GetCollisionEvents(other, bulletCollisionEvent);
        int i = 0;
        while (i < numberOfCollisionEvent)
        {
            DoDamage(bulletCollisionEvent[i].colliderComponent.gameObject, parameters.damage);
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
                muzzelFlash.Play();
            }
            ShotFX();
        } else
        {
            // stop the particle system
            if (muzzelFlash.isPlaying)
            {
                muzzelFlash.Stop();
            }
        }

        ClearTargets();
        CancelInvoke();
    }

    protected override void Aiming()
    {

        if (targeting.target == null)
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
            RotateTurretBaseTorwardTarget(go_baseRotation, targeting.target.transform, targeting.aimingSpeed);

            RotateTurretHeadAimAtTarget(go_GunBody, targeting.target.transform, targeting.aimingSpeed, 0.5f);
        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, barrleRotationDamping * Time.deltaTime);
        }
    }

    public override void TierChange()
    {
        base.TierChange();

        var emission = muzzelFlash.emission;
        emission.rateOverTime = parameters.FireRate;
        emission = bulletShell.emission;
        emission.rateOverTime = parameters.FireRate;
        emission = bulletTraser.emission;
        emission.rateOverTime = parameters.FireRate;

    }

}
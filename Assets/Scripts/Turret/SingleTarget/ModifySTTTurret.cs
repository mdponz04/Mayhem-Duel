using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifySTTTurret : TurretBase
{
    [Header("Sinper gun part")]
    // Gameobjects need to control rotation and aiming
    [SerializeField] Transform baseRotation;
    [SerializeField] ParticleSystem tracerBullet;

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        bool debug = false;
        if (debug)
        {
            Debug.DrawLine(VFX.muzzle.transform.position , VFX.muzzle.transform.forward * parameters.fireRangeRadius + VFX.muzzle.transform.position);
        }
    }

    protected override void Aiming()
    {
        if(targeting.target == null)
        {
            return;
        }

        RotateTurretBaseTorwardTarget(baseRotation, targeting.target.transform, targeting.aimingSpeed);

    }

    protected override void ShotFX()
    {
        base.ShotFX();
        tracerBullet.Play();
    }

    protected override void Shooting()
    {
        if (targeting.target == null)
        {
            return;
        }

        if (parameters.canFire == false)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(VFX.muzzle.position, VFX.muzzle.transform.forward, out hit, parameters.fireRangeRadius))
        {
            if (CheckTags(hit.collider) || CheckLayer(hit.collider))
            {
                ShotFX();
                DoDamage(hit.collider.gameObject, parameters.damage);
            }

            ClearTargets();
            CancelInvoke();
        }
    }
}

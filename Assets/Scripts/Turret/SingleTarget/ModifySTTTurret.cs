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
    [SerializeField] Transform bulletImpact;

    protected Animator animator;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isDebug)
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

    protected override void ShotVFX()
    {
        base.ShotVFX();
        tracerBullet.Emit(1);
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
                animator.SetTrigger("Shot");
                ShotVFX();
                BulletImpactFVX(hit.point, bulletImpact);
                DoDamage(hit.collider.gameObject, parameters.damage);
            }

            ClearTargets();
            CancelInvoke();
        }
    }
}

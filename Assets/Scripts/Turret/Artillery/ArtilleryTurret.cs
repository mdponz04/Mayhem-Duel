using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArtilleryTurret : TurretBase
{
    [Header("Artillery turret part")]
    // Gameobjects need to control rotation and aiming
    [SerializeField] Transform baseRotation;
    [SerializeField] Transform gunBody;
    [SerializeField] Transform muzzlePosition;

    [Header("Projectile")]
    [SerializeField] Transform pfProjectile;
    [SerializeField][Range(-90, 90)] float projectileFireAngle;
    [SerializeField] float deadZoneRadious;

    [SerializeField]
    protected float projectileSpeed;
    protected float currentFireRateCoolDown;


    protected override void Aiming()
    {

        if (targeting.target == null)
        {
            return;
        }
        // if can fire turret activates
        if (parameters.canFire)
        {
            // aim at enemy
            RotateTurretBaseTorwardTarget(baseRotation, targeting.target.transform, targeting.aimingSpeed);
            RotateTurretBaseTorwardTarget(gunBody, targeting.target.transform, targeting.aimingSpeed);
            RotateTurretHeadByDegree(gunBody, projectileFireAngle, 0, 0, targeting.aimingSpeed, 0);

            projectileSpeed = CalculateProjectileSpeed(targeting.target.transform, muzzlePosition.transform, Physics.gravity.magnitude, projectileFireAngle);

            Shooting();
        }
    }

    protected override void Shooting()
    {
        base.Shooting();
        if (currentFireRateCoolDown <= 0 && targeting.target != null)
        {
            Transform tempProjectile = Instantiate(pfProjectile, muzzlePosition.position, Quaternion.LookRotation(muzzlePosition.transform.forward, Vector3.up));

            ArtilleryProjectile artilleryProjectile = tempProjectile.GetComponent<ArtilleryProjectile>();
            artilleryProjectile.SetUp(muzzlePosition, projectileSpeed, targeting.target.transform);
            currentFireRateCoolDown = parameters.FireCoolDown;
        }
        currentFireRateCoolDown -= Time.deltaTime;
    }

    protected float CalculateProjectileSpeed(Transform target, Transform muzzlePosition, float gravity, float fireAngleInDegree)
    {
        var turretXZ = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        var targetXZ = new Vector3(target.position.x, 0, target.position.z);
        float distantToTarget = Vector3.Distance(turretXZ, targetXZ);
        float heightOffset = target.position.y - targetXZ.y;
        return CalculateProjectileSpeed(distantToTarget, gravity, fireAngleInDegree, heightOffset);
    }

    protected float CalculateProjectileSpeed(float distantToTarget, float gravity, float fireAngleInDegree, float heightOffset)
    {
        //sin(2*alpha) = d*g/(v0)^2
        float angleInRadian = Mathf.Abs(fireAngleInDegree) * Mathf.Deg2Rad;
        float speed = Mathf.Sqrt((distantToTarget * gravity) / (Mathf.Sin(2 * angleInRadian)));

        speed -= Mathf.Sqrt(Mathf.Abs(heightOffset));
        return speed;
    }

    protected override void ClearTargets()
    {
        base.ClearTargets();

        foreach (Collider target in targeting.targets.ToList())
        {
            var distantToTarget = Vector3.Distance(gameObject.transform.position, target.transform.position);
            if (distantToTarget < deadZoneRadious)
            {
                targeting.targets.Remove(target);
            }
        }
    }
    protected void OnValidate()
    {
        deadZoneRadious = Mathf.Clamp(deadZoneRadious, 0, parameters.fireRangeRadius);
    }
}

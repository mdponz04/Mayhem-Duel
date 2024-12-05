using System;
using System.Linq;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ArtilleryTurret : TurretBase
{
    [Header("Artillery turret part")]
    // Gameobjects need to control rotation and aiming
    [SerializeField]
    Transform baseRotation;

    [SerializeField]
    Transform gunBody;

    [SerializeField]
    Transform muzzlePosition;

    [Header("Projectile")]
    [SerializeField]
    Transform pfProjectile;

    [SerializeField]
    [Range(-90, 90)]
    float projectileFireAngle;

    [SerializeField]
    float deadZoneRadious;

    [SerializeField]
    protected float projectileSpeed;
    protected float currentFireRateCoolDown;

    [SerializeField] protected Vector3 projectileScale;

    [Header("Animator")]
    Animator animator;
    NetworkAnimator networkAnimator;
    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkAnimator = GetComponent<NetworkAnimator>();
    }
    private void Awake()
    {
        turretBase = baseRotation;
        turretHead = gunBody;
    }
    private void LateUpdate()
    {
        //==============================================================
        if (isDebug)
        {
            DebugExtension.DebugWireSphere(transform.position, Color.green, deadZoneRadious);
        }
        //==============================================================

    }

    protected override void Aiming()
    {
        if (targeting.currentTarget == null)
        {
            return;
        }
        // if can fire turret activates
        if (parameters.canFire)
        {
            // aim at enemy
            RotateTurretBaseTorwardTarget(
                baseRotation,
                targeting.currentTarget.transform,
                targeting.aimingSpeed
            );
            RotateTurretBaseTorwardTarget(
                gunBody,
                targeting.currentTarget.transform,
                targeting.aimingSpeed
            );
            RotateTurretHeadByDegree(gunBody, projectileFireAngle, 0, 0, targeting.aimingSpeed, 0);
            projectileSpeed = CalculateProjectileSpeed(
                targeting.currentTarget.transform,
                muzzlePosition.transform,
                Physics.gravity.magnitude,
                projectileFireAngle
            );

            //Debug.Log(projectileSpeed);
            //Shooting();
        }
    }

    protected override void Shooting()
    {
        base.Shooting();
        if (currentFireRateCoolDown <= 0 && targeting.currentTarget != null)
        {
            animator.Play("Artillery_Shoot");
            ShotVFX();

            SpawnProjectile();
            currentFireRateCoolDown = parameters.FireCoolDown;
        }
        currentFireRateCoolDown -= Time.deltaTime;
    }

    //[Rpc(SendTo.ClientsAndHost)]
    protected void SpawnProjectile()
    {
        Transform tempProjectile = Instantiate(
            pfProjectile,
            muzzlePosition.position,
            Quaternion.LookRotation(muzzlePosition.transform.forward, Vector3.up)
        );
        tempProjectile.localScale = projectileScale;
        ArtilleryProjectile artilleryProjectile =
            tempProjectile.GetComponent<ArtilleryProjectile>();
        artilleryProjectile.SetUp(muzzlePosition, projectileSpeed, targeting.currentTarget.transform);
        artilleryProjectile.GetComponent<NetworkObject>().Spawn();
    }
    protected float CalculateProjectileSpeed(
        Transform target,
        Transform muzzlePosition,
        float gravity,
        float fireAngleInDegree
    )
    {
        var turretXZ = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        var targetXZ = new Vector3(target.position.x, 0, target.position.z);
        float distantToTarget = Vector3.Distance(turretXZ, targetXZ);
        float heightOffset = target.position.y - targetXZ.y;
        return CalculateProjectileSpeed(distantToTarget, gravity, fireAngleInDegree, heightOffset);
    }

    protected float CalculateProjectileSpeed(
        float distantToTarget,
        float gravity,
        float fireAngleInDegree,
        float heightOffset
    )
    {
        //sin(2*alpha) = d*g/(v0)^2
        //distantToTarget -= Mathf.Abs(heightOffset);
        //distantToTarget *= 0.9f;

        float angleInRadian = Mathf.Abs(fireAngleInDegree) * Mathf.Deg2Rad;
        float speed = Mathf.Sqrt((distantToTarget * gravity) / (Mathf.Sin(2 * angleInRadian)));

        //speed -= Mathf.Sqrt(Mathf.Abs(heightOffset));
        return speed;
    }

    protected override void SetCurrentTarget()
    {
        base.SetCurrentTarget();
        var farthestTarget = GetFarthestTarget();
        if (farthestTarget != null)
        {
            targeting.currentTarget = farthestTarget;
        }
    }

    protected override bool IsTargetValid(Collider target)
    {
        if (!base.IsTargetValid(target))
        {
            return false;
        }

        bool isTargetInRange = true;
        var distantToTarget = Vector3.Distance(
            gameObject.transform.position,
            target.transform.position
        );
        if (distantToTarget < deadZoneRadious)
        {
            isTargetInRange = false;
        }

        return isTargetInRange;
    }

    protected void OnValidate()
    {
        deadZoneRadious = Mathf.Clamp(deadZoneRadious, 0, parameters.fireRangeRadius);
    }
}

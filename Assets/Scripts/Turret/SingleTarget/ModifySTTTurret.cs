using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ModifySTTTurret : TurretBase
{
    [Header("Single target gun part")]
    // Gameobjects need to control rotation and aiming
    [SerializeField] Transform baseRotation;
    [SerializeField] ParticleSystem tracerBullet;
    [SerializeField] Transform bulletImpact;

    [Header("Network part")]
    NetworkAnimator networkAnimator;
    protected Animator animator;

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
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (isDebug)
        {
            Debug.DrawLine(VFX.muzzle.transform.position, VFX.muzzle.transform.forward * parameters.fireRangeRadius + VFX.muzzle.transform.position);
        }
    }

    protected override void Aiming()
    {
        if (targeting.currentTarget == null)
        {
            return;
        }

        RotateTurretBaseTorwardTarget(baseRotation, targeting.currentTarget.transform, targeting.aimingSpeed);

    }

    //protected override void ShotVFX()
    //{
    //    base.ShotVFX();
    //    tracerBullet.Emit(1);
    //}

    protected override void Shooting()
    {
        if (targeting.currentTarget == null)
        {
            return;
        }

        if (parameters.canFire == false)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(VFX.muzzle.position, VFX.muzzle.transform.forward, out hit, parameters.fireRangeRadius, targeting.layersToFire))
        {
            if (CheckTags(hit.collider) || CheckLayer(hit.collider))
            {
                animator.Play("Shot Animation");
                ShotVFX();
                //tracerBullet.Emit(1);
                //BulletImpactFVX(hit.point, bulletImpact);
                SpawnShootParticleClientRpc(hit.point);
                DoDamage(hit.collider.gameObject, parameters.damage);
            }

            //RefreshCurrentTarget();
            CancelInvoke();
        }
    }
    [Rpc(SendTo.ClientsAndHost)]
    protected void SpawnShootParticleClientRpc(Vector3 hitPosition)
    {
        tracerBullet.Emit(1);
        BulletImpactFVX(hitPosition, bulletImpact);
    }
}

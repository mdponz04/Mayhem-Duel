using Assets.Scripts.Turret;
using CodeMonkey.Utils;
using System.Collections.Generic;
using System.Linq;
using TheDamage;
using TheHealth;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public class TurretParameters
{
    [Header("Status")]
    [Tooltip("Activate or deactivate the Turret")]
    public bool active;
    public bool canFire;

    [Header("Shooting")]
    [Tooltip("Burst the force when hit")]
    public float damage;

    [Tooltip("Fire rate per second")]
    [Range(0.1f, 20)]
    public float FireRate;
    public float FireCoolDown
    {
        get { return 1f / FireRate; }
    }

    [Tooltip("Radius of the turret view")]
    public float fireRangeRadius;
}

[System.Serializable]
public class TurretFX
{
    [Tooltip("Muzzle transform position")]
    public Transform muzzle;

    [Tooltip("Spawn this GameObject when shooting")]
    public GameObject shotFX;
}

[System.Serializable]
public class TurretAudio
{
    public AudioClip shotClip;
}

[System.Serializable]
public class TurretTargeting
{
    [Tooltip("Speed of aiming at the target")]
    public float aimingSpeed;

    [Tooltip("Pause before the aiming")]
    public float aimingDelay;

    [Tooltip("GameObject with folowing tags will be identify as enemy")]
    public string[] tagsToFire;
    public LayerMask layersToFire;
    public List<Collider> targets = new List<Collider>();
    public Collider currentTarget;
}

[System.Serializable]
public class TurretUpgrade
{
    public TurretTier[] availableTiers;
    public int minimunTierIndex = 0;
    public int currentTierIndex = 0;

    public int maximunTierIndex
    {
        get { return availableTiers.Length - 1; }
    }

    public TurretTier currentTier
    {
        get { return availableTiers[currentTierIndex]; }
    }
}

[System.Serializable]
public class TurretMeshes
{
    public MeshRenderer[] meshes;
}
public enum TurretType
{
    MachineGun,
    SingleTarget,
    Artillery
}

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class TurretBase : NetworkBehaviour
{
    public bool isDebug = false;

    [Space(5)]
    [Header("Mesh control")]
    [SerializeField]
    TurretMeshes[] turretPart;

    public TurretParameters parameters;
    public TurretTargeting targeting;
    public TurretFX VFX;
    public TurretAudio SFX;
    public TurretUpgrade upgrade;

    protected Transform turretBase;
    protected Transform turretHead;

    //private DamageDealer damageDealer;

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    protected virtual void Start()
    {
        TierChange();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            //UpgradeTier();
            UpgradeTierServerRpc();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            //DowngradeTier();
            DowngradeTierServerRpc();
        }
        //==============================================================
        if (isDebug)
        {
            DebugExtension.DebugWireSphere(transform.position, Color.green, parameters.fireRangeRadius);
        }
        //==============================================================

        if (IsTargetValid(targeting.currentTarget))
        {
            parameters.canFire = true;
        }
        else
        {
            parameters.canFire = false;
        }
        RefreshCurrentTarget();

    }

    protected virtual void FixedUpdate()
    {
        if (!IsServer)
        {
            return;
        }
        if (parameters.active == false)
        {
            return;
        }


        //parameters.canFire = true;
        Aiming();
        Invoke("Shooting", 1f / parameters.FireRate);
    }

    #region Aiming and Shooting

    protected virtual void ShotVFX()
    {
        //GetComponent<AudioSource>().PlayOneShot(SFX.shotClip, Random.Range(0.75f, 1));
        //GameObject newShotFX = Instantiate(VFX.shotFX, VFX.muzzle);
        //Destroy(newShotFX, 2);
        ShotVFXVisualClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected virtual void ShotVFXVisualClientRpc()
    {
        GetComponent<AudioSource>().PlayOneShot(SFX.shotClip, Random.Range(0.75f, 1));
        GameObject newShotFX = Instantiate(VFX.shotFX, VFX.muzzle);
        Destroy(newShotFX, 2);
    }
    public static void BulletImpactFVX(Vector3 impactPosition, Transform impactVFX)
    {
        Transform vfx = Instantiate(impactVFX, impactPosition, Quaternion.identity);
        //vfx.transform.parent = gameObject.transform;
    }

    protected virtual void Shooting()
    {
        //Implement logic here
    }

    protected virtual void Aiming()
    {
        //Implement logic here
    }

    #endregion

    #region Targeting
    protected virtual void RefreshCurrentTarget()
    {
        if (!IsTargetValid(targeting.currentTarget))
        {
            targeting.targets.Remove(targeting.currentTarget);
            targeting.currentTarget = null;
        }

        RefreshTargetList();

        SetCurrentTarget();
    }

    protected void RefreshTargetList()
    {
        if (!IsTargetAvailable())
        {
            return;
        }

        var targets = targeting.targets.ToList();
        foreach (Collider target in targets)
        {
            if (!IsTargetValid(target))
            {
                targeting.targets.Remove(target);
            }
        }
    }

    /// <summary>
    /// Condition for setting target's priority (closest, farthest, lowest hp)
    /// </summary>
    protected virtual void SetCurrentTarget()
    {
        if (IsTargetAvailable())
        {
            targeting.currentTarget = targeting.targets.First();
        }
        else
        {
            targeting.currentTarget = null;
        }

        var closestTarget = GetClosestTarget();
        if (closestTarget != null)
        {
            targeting.currentTarget = closestTarget;
        }
    }

    /// <summary>
    /// Condition for a target to be shootable
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected virtual bool IsTargetValid(Collider target)
    {
        if (target == null)
        { return false; }
        bool isTargetValid = true;
        if (target.GetComponent<Collider>() == false)
        {
            Debug.Log($"{target.name} invalid collider");
            isTargetValid = false;
        }

        //var distance = Vector3.Distance(transform.position, target.gameObject.transform.position);
        //if (distance > parameters.fireRangeRadius)
        //{
        //    isTargetValid = false;
        //}

        var healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            if (healthSystem.currentHealth <= 0)
            {
                Debug.Log($"{target.name} invalid health");
                isTargetValid = false;
            }
        }

        return isTargetValid;
    }

    protected bool IsTargetAvailable()
    {
        bool isTargetAvailable = false;
        if (targeting.targets.Count != 0)
        {
            isTargetAvailable = true;
        }
        return isTargetAvailable;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (parameters.active == false)
        {
            return;
        }

        //ClearTargetsList();

        if (CheckTags(other) || CheckLayer(other))
        {
            //Debug.Log($"{other.gameObject.name} enter {gameObject.name}'s collider");
            //if (targeting.targets.Count == 0)
            //{
            //    targeting.currentTarget = other.GetComponent<Collider>();
            //}
            if (IsTargetValid(other))
            {
                targeting.targets.Add(other);
            }
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (parameters.active == false)
    //    {
    //        return;
    //    }

    //    ClearTargets();

    //    if (CheckTags(other) || CheckLayer(other))
    //    {
    //        if (!targeting.targets.Contains(other.GetComponent<Collider>()))
    //        {
    //            targeting.targets.Add(other.GetComponent<Collider>());
    //        }
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (parameters.active == false)
        {
            return;
        }

        //ClearTargetsList();

        if (CheckTags(other) || CheckLayer(other))
        {
            //Debug.Log($"{other.gameObject.name} exit {gameObject.name}'s collider");
            targeting.targets.Remove(other);
            //if (targeting.targets.Count != 0)
            //{
            //    targeting.currentTarget = targeting.targets.First();
            //}
            //else
            //{
            //    targeting.currentTarget = null;
            //}
        }
    }

    protected bool CheckTags(Collider toMatch)
    {
        bool match = false;

        for (int i = 0; i < targeting.tagsToFire.Length; i++)
        {
            if (toMatch.CompareTag(targeting.tagsToFire[i]))
            {
                match = true;
            }
        }

        return match;
    }

    protected bool CheckLayer(Collider toMatch)
    {
        bool match = false;
        if (UtilsClass.IsLayerInLayerMask(toMatch.gameObject.layer, targeting.layersToFire))
        {
            match = true;
        }
        return match;
    }

    protected Collider GetClosestTarget()
    {
        if (!IsTargetAvailable())
        {
            return null;
        }
        Collider closestTarget = null;
        float closestDistance = Mathf.Infinity;

        List<Collider> targets = targeting.targets.ToList();
        foreach (Collider target in targets)
        {
            float distantToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distantToTarget < closestDistance)
            {
                closestDistance = distantToTarget;
                closestTarget = target;
            }
        }
        return closestTarget;
    }

    protected Collider GetFarthestTarget()
    {
        if (!IsTargetAvailable())
        {
            return null;
        }
        Collider farthestTarget = null;
        float farthestDistance = Mathf.NegativeInfinity;

        List<Collider> targets = targeting.targets.ToList();
        foreach (Collider target in targets)
        {
            float distantToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (distantToTarget > farthestDistance)
            {
                farthestDistance = distantToTarget;
                farthestTarget = target;
            }
        }
        return farthestTarget;
    }
    #endregion

    #region Upgrades
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void UpgradeTierServerRpc()
    {
        UpgradeTierClientRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void UpgradeTierClientRpc()
    {
        UpgradeTier();
    }
    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void DowngradeTierServerRpc()
    {
        DowngradeTierClientRpc();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void DowngradeTierClientRpc()
    {
        DowngradeTier();
    }
    public void UpgradeTier()
    {
        if (upgrade.currentTierIndex < upgrade.minimunTierIndex)
        {
            upgrade.currentTierIndex = upgrade.minimunTierIndex;
            return;
        }

        upgrade.currentTierIndex++;

        if (upgrade.currentTierIndex > upgrade.maximunTierIndex)
        {
            upgrade.currentTierIndex = upgrade.maximunTierIndex;
        }

        TierChange();
    }

    public void DowngradeTier()
    {
        if (upgrade.currentTierIndex > upgrade.maximunTierIndex)
        {
            upgrade.currentTierIndex = upgrade.maximunTierIndex;
            return;
        }

        upgrade.currentTierIndex--;

        if (upgrade.currentTierIndex < upgrade.minimunTierIndex)
        {
            upgrade.currentTierIndex = upgrade.minimunTierIndex;
        }

        TierChange();
    }

    public virtual void TierChange()
    {
        if (upgrade.maximunTierIndex <= 0)
        {
            return;
        }
        parameters.FireRate = upgrade.currentTier.FireRate;
        parameters.fireRangeRadius = upgrade.currentTier.FireRange;
        parameters.damage = upgrade.currentTier.Damage;

        if (upgrade.currentTier.TierMaterial != null)
        {
            for (int i = 0; i < turretPart.Length; i++)
            {
                for (int j = 0; j < turretPart[i].meshes.Length; j++)
                {
                    var material = upgrade.currentTier.TierMaterial[i];
                    if (material != null)
                    {
                        turretPart[i].meshes[j].material = material;
                    }
                }
            }
        }

        SetSphereColliderRadius(parameters.fireRangeRadius);
    }

    protected void SetSphereColliderRadius(float fireRangeRadius)
    {

        this.GetComponent<SphereCollider>().radius = parameters.fireRangeRadius / transform.lossyScale.x;
    }

    #endregion

    #region Turret Part Rotation

    protected void RotateTurretBaseTorwardTarget(
        Transform turretBase,
        Transform target,
        float rotationDamping
    )
    {
        Vector3 targetPosition = new Vector3(
            target.position.x,
            turretBase.position.y,
            target.position.z
        );
        var rotationToTarget = Quaternion.LookRotation(targetPosition - turretBase.position);

        turretBase.rotation = Quaternion.Lerp(
            turretBase.rotation,
            rotationToTarget,
            Time.deltaTime * rotationDamping
        );
        //RotateThisTurretBaseClientRpc(targetPosition, rotationToTarget, rotationDamping);
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected void RotateThisTurretBaseClientRpc(
        Vector3 turretBaseRotation,
        Quaternion rotationToTarget,
        float rotationDamping)
    {

        this.turretBase.rotation = Quaternion.Lerp(
            turretBase.rotation,
            rotationToTarget,
            Time.deltaTime * rotationDamping
        );
    }

    protected void RotateTurretHeadAimAtTarget(
        Transform turretHead,
        Transform target,
        float rotationDamping,
        float turretHeadOffset
    )
    {
        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y - turretHeadOffset,
            target.position.z
        );
        var rotationToTarget = Quaternion.LookRotation(targetPosition - turretHead.position);

        //Quaternion rotationOffset = Quaternion.Euler(rotationXOffset, rotationYOffset, rotationZOffset);

        //rotationToTarget *= rotationOffset;

        turretHead.rotation = Quaternion.Lerp(
            turretHead.rotation,
            rotationToTarget,
            Time.deltaTime * rotationDamping
        );
    }

    [Rpc(SendTo.ClientsAndHost)]
    protected void RotateThisTurretHeadClientRpc(
       Vector3 targetPosition,
       Quaternion rotationToTarget,
       float rotationDamping
        )
    {

        this.turretHead.rotation = Quaternion.Lerp(
            turretHead.rotation,
            rotationToTarget,
            Time.deltaTime * rotationDamping
        );
    }

    protected void RotateTurretHeadByDegree(
        Transform turretHead,
        float degreeX,
        float degreeY,
        float degreeZ,
        float rotationDamping,
        float turretHeadOffset
    )
    {
        var rotation = turretHead.rotation;
        rotation *= Quaternion.Euler(degreeX, degreeY, degreeZ);

        turretHead.rotation = Quaternion.Lerp(
            turretHead.rotation,
            rotation,
            Time.deltaTime * rotationDamping
        );
    }

    #endregion

    #region Do damage
    protected void DoDamage(GameObject target, float damage)
    {
        if (!IsServer) { return; }
        Vulnerable damageable = target.GetComponent<Vulnerable>();

        if (damageable != null)
        {
            damageable.TakeDamge(damage);
            if (isDebug)
            {
                UtilsClass.CreateWorldTextPopup(damage.ToString(), target.transform.position);
            }
        }
    }
    #endregion

    #region Gizmos
    //void OnDrawGizmosSelected()
    //{
    //    // Draw a red sphere at the transform's position to show the firing range
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, parameters.fireRangeRadius);
    //}
    #endregion
}


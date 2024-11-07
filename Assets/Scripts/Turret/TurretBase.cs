using Assets.Scripts.Turret;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public float FireCoolDown { get { return 1f / FireRate; } }
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
    public Collider target;

}

[System.Serializable]
public class TurretUpgrade
{
    public TurretTier[] availableTiers;
    public int minimunTierIndex = 0;
    public int currentTierIndex = 0;

    public int maximunTierIndex { get { return availableTiers.Length - 1; } }

    public TurretTier currentTier { get { return  availableTiers[currentTierIndex]; } }
}

[System.Serializable]
public class TurretMeshes
{
    public MeshRenderer[] meshes;
}
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(AudioSource))]
public class TurretBase : MonoBehaviour
{

    [Space(5)]
    [Header("Mesh control")]
    [SerializeField] TurretMeshes[] turretPart;

    public TurretParameters parameters;
    public TurretTargeting targeting;
    public TurretFX VFX;
    public TurretAudio SFX;
    public TurretUpgrade upgrade;

    private void Awake()
    {

        GetComponent<SphereCollider>().isTrigger = true;
        GetComponent<SphereCollider>().radius = parameters.fireRangeRadius;
    }

    protected virtual void Start()
    {
        TierChange();
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpgradeTier();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            DowngradeTier();
        }
    }

    protected virtual void FixedUpdate()
    {

        if (parameters.active == false)
        {
            return;
        }

        if (targeting.target == null)
        {
            parameters.canFire = false;
            ClearTargets();
        }

        if (targeting.target != null)
        {
            parameters.canFire = true;
            Aiming();
            Invoke("Shooting", 1f / parameters.FireRate);
        }
    }

    #region Aiming and Shooting

    protected virtual void ShotFX()
    {

        GetComponent<AudioSource>().PlayOneShot(SFX.shotClip, Random.Range(0.75f, 1));
        GameObject newShotFX = Instantiate(VFX.shotFX, VFX.muzzle);
        Destroy(newShotFX, 2);
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

    private void OnTriggerEnter(Collider other)
    {

        if (parameters.active == false)
        {
            return;
        }

        ClearTargets();

        if (CheckTags(other) || CheckLayer(other))
        {
            if (targeting.targets.Count == 0)
            {
                targeting.target = other.GetComponent<Collider>();
            }

            targeting.targets.Add(other.GetComponent<Collider>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (parameters.active == false)
        {
            return;
        }

        ClearTargets();

        if (CheckTags(other) || CheckLayer(other))
        {
            if (!targeting.targets.Contains(other.GetComponent<Collider>()))
            {
                targeting.targets.Add(other.GetComponent<Collider>());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (parameters.active == false)
        {
            return;
        }

        ClearTargets();

        if (CheckTags(other) || CheckLayer(other))
        {
            targeting.targets.Remove(other.GetComponent<Collider>());
            if (targeting.targets.Count != 0)
            {
                targeting.target = targeting.targets.First();
            }
            else
            {
                targeting.target = null;
            }
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
        if(UtilsClass.IsLayerInLayerMask(toMatch.gameObject.layer, targeting.layersToFire))
        {
            match = true;
        }
        return match;
    }

    protected virtual void ClearTargets()
    {

        if (targeting.target != null)
        {
            if (targeting.target.GetComponent<Collider>().enabled == false)
            {
                targeting.targets.Remove(targeting.target);
            }
        }

        foreach (Collider target in targeting.targets.ToList())
        {

            if (target == null)
            {
                targeting.targets.Remove(target);
            }

            if (targeting.targets.Count != 0)
            {
                targeting.target = targeting.targets.First();
            }
            else
            {
                targeting.target = null;
            }
        }
    }

    #endregion

    #region Upgrades

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
        if(upgrade.maximunTierIndex <= 0)
        {
            return;
        }
        parameters.FireRate = upgrade.currentTier.FireRate;
        parameters.fireRangeRadius = upgrade.currentTier.FireRange;
        parameters.damage = upgrade.currentTier.Damage;
        //barrelRotationSpeed = fireRate / 3 * 360;

        if (upgrade.currentTier.TierMaterial != null)
        {
            for( int i = 0; i < turretPart.Length; i++)
            {
                for ( int j = 0; j < turretPart[i].meshes.Length; j++)
                {
                    var material = upgrade.currentTier.TierMaterial[i];
                    if(material != null)
                    {
                        turretPart[i].meshes[j].material = material;
                    }
                }
            }
        }

        this.GetComponent<SphereCollider>().radius = parameters.fireRangeRadius;

    }

    #endregion

    #region Turret Part Rotation

    protected void RotateTurretBaseTorwardTarget(Transform turretBase, Transform target, float rotationDamping)
    {
        Vector3 targetPosition = new Vector3(target.position.x, turretBase.position.y, target.position.z);
        var rotationToTarget = Quaternion.LookRotation(targetPosition - turretBase.position);

        turretBase.rotation = Quaternion.Lerp(turretBase.rotation, rotationToTarget, Time.deltaTime * rotationDamping);
    }

    protected void RotateTurretHeadAimAtTarget(Transform turretHead, Transform target, float rotationDamping)
    {
        RotateTurretHeadAimAtTarget(turretHead, target, rotationDamping, 0);
    }

    protected void RotateTurretHeadAimAtTarget(Transform turretHead, Transform target, float rotationDamping, float turretHeadOffset)
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y - turretHeadOffset, target.position.z);
        var rotationToTarget = Quaternion.LookRotation(targetPosition - turretHead.position);

        //Quaternion rotationOffset = Quaternion.Euler(rotationXOffset, rotationYOffset, rotationZOffset);

        //rotationToTarget *= rotationOffset;

        turretHead.rotation = Quaternion.Lerp(turretHead.rotation, rotationToTarget, Time.deltaTime * rotationDamping);
    }

    protected void RotateTurretHeadByDegree(Transform turretHead, float degreeX, float degreeY, float degreeZ, float rotationDamping, float turretHeadOffset)
    {
        var rotation = turretHead.rotation;
        rotation *= Quaternion.Euler(degreeX, degreeY, degreeZ);  

        turretHead.rotation = Quaternion.Lerp(turretHead.rotation, rotation, Time.deltaTime * rotationDamping);
    }
    #endregion

    #region Do damage
    protected void DoDamage(GameObject target, float damage)
    {
        IEnemy enemy = UtilsClass.GetInterfaceComponent<IEnemy>(target);
        if( enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
    #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, parameters.fireRangeRadius);
    }
    #endregion
}
using Assets.Scripts.Turret.Machine_gun;
using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class ModifyMachineGun : MonoBehaviour
{
    [Header("Machine gun part")]
    // Gameobjects need to control rotation and aiming
    [SerializeField] Transform go_baseRotation;
    [SerializeField] Transform go_GunBody;
    [SerializeField] Transform go_barrel;
    [SerializeField] float rotationDamping = 5f;

    [Space(5)]
    [Header("Particle system")]
    // Particle system for the muzzel flash
    [SerializeField] ParticleSystem muzzelFlash;
    [SerializeField] ParticleSystem bulletShell;
    [SerializeField] ParticleSystem bulletTraser;
    List<ParticleCollisionEvent> bulletCollisionEvent;

    [Space(5)]
    [Header("Mesh control")]
    [SerializeField] MeshRenderer[] gunMeshes;

    [Space(5)]
    [Header("Tier")]
    [SerializeField] TurretTier[] AvailabelTiers;
    int minimunTier = 0;
    int maximunTier;
    int currentTierIndex = 0;

    [Space(5)]
    [Header("Layers to attack")]
    [SerializeField] LayerMask layersToTarget;
    // Gun barrel rotation
    public bool allowFire = true;

    float fireRate;
    float fireRange;
    float damage;
    float barrelRotationSpeed;
    float currentRotationSpeed;
    // Used to start and stop the turret firing
    bool canFire = false;

    // target the gun will aim at
    GameObject currentTarget;
    List<GameObject> targetList = new List<GameObject>();
    float closestTargetDistant = Mathf.Infinity;

    //Target turret will protect
    Transform placeToProtect;

    void Start()
    {
        // Set the firing range distance
        placeToProtect = transform;
        maximunTier = AvailabelTiers.Count() - 1;
        TierChange();
        bulletCollisionEvent = new List<ParticleCollisionEvent>();
    }

    void Update()
    {
        targetList.RemoveAll(target => target == null);

        if (targetList.Count > 0)
        {
            FindClosestTarget();
            if (currentTarget != null)
            {
                canFire = true;
                //Debug.Log(currentTarget);
            }
        }
        else if (targetList.Count == 0)
        {
            canFire = false;
        }

        //Aim();

        if (Input.GetKeyDown(KeyCode.U))
        {
            UpgradeTier();
        }
        
        if (Input.GetKeyDown(KeyCode.Y))
        {
            DowngradeTier();
        }
    }

    void FixedUpdate()
    {
        Aim();
    }


    public void OnParticleCollision(GameObject other)
    {
        int numberOfCollisionEvent = bulletTraser.GetCollisionEvents(other, bulletCollisionEvent);
        int i = 0;
        while (i < numberOfCollisionEvent)
        {
            IEnemy enemy = UtilsClass.GetInterfaceComponent<IEnemy>(bulletCollisionEvent[i].colliderComponent.gameObject);
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            i++;
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fireRange);
    }

    private void OnTriggerStay(Collider other)
    {
        if (UtilsClass.IsLayerInLayerMask(other.gameObject.layer, layersToTarget))
        {
            if (!targetList.Contains(other.gameObject))
            {
                targetList.Add(other.gameObject);
            }
        }
    }

    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (UtilsClass.IsLayerInLayerMask(other.gameObject.layer, layersToTarget))
        {
            targetList.Remove(other.gameObject);
            if (other.gameObject == currentTarget)
            {
                currentTarget = null;
            }
        }
    }

    void FindClosestTarget()
    {
        closestTargetDistant = Mathf.Infinity;

        foreach (GameObject target in targetList)
        {
            float distant = Vector3.Distance(target.transform.position, placeToProtect.transform.position);

            if (distant < closestTargetDistant)
            {
                currentTarget = target;
                closestTargetDistant = distant;
            }
        }
    }

    void Aim()
    {
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // if can fire turret activates
        if (canFire && currentTarget != null)
        {
            // start rotation
            currentRotationSpeed = barrelRotationSpeed;

            // aim at enemy
            Vector3 baseTargetPostition = new Vector3(currentTarget.transform.position.x, this.transform.position.y, currentTarget.transform.position.z);
            Vector3 gunBodyTargetPostition = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y * 0.5f, currentTarget.transform.position.z);
            var baseTargetRotationPosistion = Quaternion.LookRotation(baseTargetPostition - this.transform.position);
            var gunBodyRotationPosition = Quaternion.LookRotation(gunBodyTargetPostition - this.transform.position);

            go_baseRotation.transform.rotation = Quaternion.Lerp(go_baseRotation.transform.rotation, baseTargetRotationPosistion, Time.deltaTime * rotationDamping);
            go_GunBody.transform.rotation = Quaternion.Lerp(go_GunBody.transform.rotation, gunBodyRotationPosition, Time.deltaTime * rotationDamping);

            //go_baseRotation.transform.LookAt(baseTargetPostition);
            //go_GunBody.transform.LookAt(gunBodyTargetPostition);

            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                muzzelFlash.Play();
            }
        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);

            // stop the particle system
            if (muzzelFlash.isPlaying)
            {
                muzzelFlash.Stop();
            }
        }
    }

    public void UpgradeTier()
    {
        if (currentTierIndex < minimunTier)
        {
            currentTierIndex = minimunTier;
            return;
        }

        currentTierIndex++;

        if (currentTierIndex > maximunTier)
        {
            currentTierIndex = maximunTier;
        }

        TierChange();
    }

    public void DowngradeTier()
    {
        if (currentTierIndex > maximunTier)
        {
            currentTierIndex = maximunTier;
            return;
        }

        currentTierIndex--;

        if (currentTierIndex < minimunTier)
        {
            currentTierIndex = minimunTier;
        }

        TierChange();
    }

    void TierChange()
    {
        fireRate = AvailabelTiers[currentTierIndex].FireRate;
        fireRange = AvailabelTiers[currentTierIndex].FireRange;
        damage = AvailabelTiers[currentTierIndex].Damage;
        barrelRotationSpeed = fireRate / 3 * 360;

        if (AvailabelTiers[currentTierIndex].TierMaterial!= null)
        {
            foreach (MeshRenderer mesh in gunMeshes)
            {
                mesh.material = AvailabelTiers[currentTierIndex].TierMaterial;
            }
        }

        this.GetComponent<SphereCollider>().radius = fireRange;
        var emission = muzzelFlash.emission;
        emission.rateOverTime = fireRate;
        emission = bulletShell.emission;
        emission.rateOverTime = fireRate;
        emission = bulletTraser.emission;
        emission.rateOverTime = fireRate;

    }

}
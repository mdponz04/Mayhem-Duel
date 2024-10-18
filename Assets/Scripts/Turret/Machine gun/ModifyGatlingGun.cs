using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class ModifyGatlingGun : MonoBehaviour
{
    // target the gun will aim at
    GameObject currentTarget;
    List<GameObject> targetList = new List<GameObject>();
    float closestTargetDistant = Mathf.Infinity;

    Transform placeToProtect;

    // Gameobjects need to control rotation and aiming
    public Transform go_baseRotation;
    public Transform go_GunBody;
    public Transform go_barrel;

    public Transform projectileSpawn;

    // Gun barrel rotation
    public float barrelRotationSpeed;
    float currentRotationSpeed;

    public bool allowFire = true;

    // Distance the turret can aim and fire from
    public GameObject projectilePreFab;
    public float turretDamage = 1f;
    public float projectileSpeed = 10f;
    public float firingRange;
    public float fireRate = 0.15f;
    private float fireRateCooldown;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;
    public ParticleSystem bulletTraser;
    public List<ParticleCollisionEvent> bulletCollisionEvent;

    private Coroutine firingCoroutine;


    // Used to start and stop the turret firing
    bool canFire = false;

    void Start()
    {
        // Set the firing range distance
        placeToProtect = transform;
        this.GetComponent<SphereCollider>().radius = firingRange;
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
            }
        }
        else if (targetList.Count == 0)
        {
            canFire = false;
        }

        Aim();
        //Fire();

    }

    private void FixedUpdate()
    {
        fireRateCooldown -= Time.deltaTime;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRange);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
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
        if (other.gameObject.tag == "Enemy")
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
        if (canFire)
        {
            // start rotation
            currentRotationSpeed = barrelRotationSpeed;

            // aim at enemy
            Vector3 baseTargetPostition = new Vector3(currentTarget.transform.position.x, this.transform.position.y, currentTarget.transform.position.z);
            Vector3 gunBodyTargetPostition = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, currentTarget.transform.position.z);

            go_baseRotation.transform.LookAt(baseTargetPostition);
            go_GunBody.transform.LookAt(gunBodyTargetPostition);

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

    private void Fire()
    {
        if (fireRateCooldown <= 0) 
        {
            //firingCoroutine = StartCoroutine(AutoFire());
            AutoFire();
            fireRateCooldown = fireRate;
        }
    }

    //private IEnumerator AutoFire()
    private void AutoFire()
    {
        while (canFire && allowFire)
        {
            CreateBullet();

            //yield return new WaitForSeconds(fireRate);
        }
    }

    private void OnParticleTrigger()
    {
        Debug.Log("Particle trigger");
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle Collision");
        other.gameObject.SendMessage("TakeDamage", turretDamage, SendMessageOptions.DontRequireReceiver);
        //int numberOfCollisionEvent = bulletTraser.GetCollisionEvents(other, bulletCollisionEvent);
        //IEnemyTemp enemy = other.GetComponent<IEnemyTemp>();
        //int i = 0;
        //while(i < numberOfCollisionEvent)
        //{
        //    if(enemy != null)
        //    {
        //        enemy.TakeDamage(turretDamage);
        //    }
        //    i++;
        //}
    }

    private void CreateBullet()
    {
        GameObject bulleTransform = Instantiate(projectilePreFab, projectileSpawn.transform.position, Quaternion.identity);
        BulletTemp bullet = bulleTransform.GetComponent<BulletTemp>();
        bullet.SetUp(projectileSpawn.transform, projectileSpeed);
    }



}
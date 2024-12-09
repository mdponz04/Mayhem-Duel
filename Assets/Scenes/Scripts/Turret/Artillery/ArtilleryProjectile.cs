using CodeMonkey.Utils;
using TheDamage;
using Unity.Netcode;
using UnityEngine;

public class ArtilleryProjectile : NetworkBehaviour
{
    [Header("Stat")]
    [SerializeField] protected float homingSpeed;
    [SerializeField] protected float damage;
    [SerializeField] protected bool isAoe;
    [SerializeField] protected float damageRadious;
    [SerializeField] protected LayerMask layerToDamage;

    [Header("Explode VFX")]
    [SerializeField] protected Transform explosionVFX;
    public bool isDebug = false;
    protected float speed;
    protected Transform target;

    protected Rigidbody rb;
    private void FixedUpdate()
    {
        RotateTorwardVelocity();
        Homing();
    }

    public void SetUp(Transform direction, float speed)
    {
        SetUp(direction, speed, null);
    }

    public void SetUp(Transform direction, float speed, Transform target)
    {
        rb = GetComponent<Rigidbody>();
        this.speed = speed;
        rb.AddForce(direction.forward * speed, ForceMode.VelocityChange);
        this.target = target;
    }

    public void Homing()
    {
        if (rb == null || target == null) { return; }

        if (rb.velocity.y <= 0)
        {

            var targetXZ = new Vector3(target.transform.position.x, gameObject.transform.position.y, target.transform.position.z);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetXZ, homingSpeed * Time.deltaTime);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (UtilsClass.IsLayerInLayerMask(collision.gameObject.layer, layerToDamage))
        {
            //TurretBase.BulletImpactFVX(transform.position, explosionVFX);
            //SpawnBulletImpactVisualClientRpc(transform.position);
            DoDamage();
            Explode();
            //Destroy(gameObject);
        }
    }
    protected void Explode()
    {
        if (!IsServer) { return; }
        SpawnBulletImpactVisualClientRpc(transform.position);
        Destroy(gameObject);
    }
    [Rpc(SendTo.ClientsAndHost)]
    protected void SpawnBulletImpactVisualClientRpc(Vector3 position)
    {
        TurretBase.BulletImpactFVX(position, explosionVFX);
    }
    private void DoDamage()
    {
        if (!IsServer) { return; }
        Collider[] hit = Physics.OverlapSphere(gameObject.transform.position, damageRadious, layerToDamage);
        if (hit.Length > 0)
        {
            foreach (var enemy in hit)
            {
                var tempEnemy = enemy.gameObject.GetComponent<Vulnerable>();
                //var tempEnemy = UtilsClass.GetInterfaceComponent<IEnemy>(enemy.gameObject);
                if (tempEnemy != null)
                {
                    tempEnemy.TakeDamge(damage);
                    if (isDebug)
                    {
                        UtilsClass.CreateWorldTextPopup(damage.ToString(), enemy.gameObject.transform.position);

                    }
                }
            }
        }
    }

    private void RotateTorwardVelocity()
    {
        if (rb == null) { return; }
        Vector3 dir = rb.velocity.normalized;
        if (dir != Vector3.zero)
        {
            gameObject.transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}

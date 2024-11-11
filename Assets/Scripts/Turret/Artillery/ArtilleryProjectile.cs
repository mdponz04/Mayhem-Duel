using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryProjectile : MonoBehaviour
{
    [Header("Stat")]
    [SerializeField] protected float homingSpeed;
    [SerializeField] protected float damage;
    [SerializeField] protected bool isAoe;
    [SerializeField] protected float damageRadious;
    [SerializeField] protected LayerMask layerToDamage;

    protected float speed;
    protected Transform target;

    protected Rigidbody rb;
    private void FixedUpdate()
    {
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

        if(rb.velocity.y <= 0)
        {
           
            var targetXZ = new Vector3(target.transform.position.x, gameObject.transform.position.y, target.transform.position.z);
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetXZ, homingSpeed * Time.deltaTime);
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        if( UtilsClass.IsLayerInLayerMask(collision.gameObject.layer, layerToDamage))
        {
            Explode();
        }
    }

    private void Explode()
    {
        Collider[] hit = Physics.OverlapSphere(gameObject.transform.position, damageRadious, layerToDamage);
        if (hit.Length > 0)
        {
            foreach (var enemy in hit)
            {
                var tempEnemy = UtilsClass.GetInterfaceComponent<IEnemy>(enemy.gameObject);
                if (tempEnemy != null)
                {
                    tempEnemy.TakeDamage(damage);
                }
            }
        }
        Destroy(gameObject);
    }
}

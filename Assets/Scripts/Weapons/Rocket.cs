using System.Collections;
using UnityEngine;

public class Rocket : Bullet
{
    [SerializeField] private GameObject explosionEffect;
    public new static void Create(Vector3 position, Transform direction, float speed, float damage)
    {
        Transform bulleTransform = Instantiate(GameAssets.i.pfRocket, position, Quaternion.identity);
        Rocket rocket = bulleTransform.GetComponent<Rocket>();
        rocket.SetUp(direction, speed, damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(Explode());
    }

    private IEnumerator  Explode()
    {
        GetComponent<MeshRenderer>().enabled = false;
        explosionEffect.SetActive(true);
        //Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);
        //foreach (Collider collider in colliders)
        //{
        //    if (collider.TryGetComponent(out IDamageable damageable))
        //    {
        //        damageable.TakeDamage(damage);
        //    }
        //}
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);

    }
}

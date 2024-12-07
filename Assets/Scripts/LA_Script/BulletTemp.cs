using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTemp : MonoBehaviour
{
    private Rigidbody rb;
    private float timeToLive = 3f;

    private void Update()
    {
        if(timeToLive <= 0)
        {
            Destroy(gameObject, 2f);
        }
    }

    private void FixedUpdate()
    {
        timeToLive -= Time.deltaTime;
    }

    public void SetUp(Transform direction, float speed)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = direction.forward * speed;
    }
}

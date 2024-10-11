using System.Collections;
using UnityEngine;

public class HollowPurple : MonoBehaviour
{
    [SerializeField] private Transform transformPurple;
    [SerializeField] private float scaleSpeed = 1.1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private ParticleSystem outer;
    [SerializeField] private ParticleSystem core;
    private float particleSystemScaleSpeed = 1.25f;

    public Vector3 direction;
    public bool isScaling = true;

    private void Start()
    {
        transformPurple = GetComponent<Transform>();
    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime * moveSpeed;
        if(isScaling)
        {
            return;
        }
        StartCoroutine(ScaleSphere());
    }

    private IEnumerator ScaleSphere()
    {
        transform.localScale *= scaleSpeed;
        outer.transform.localScale *= particleSystemScaleSpeed;
        isScaling = true;
        yield return new WaitForSeconds(0.5f);
        isScaling = false;
    }

}

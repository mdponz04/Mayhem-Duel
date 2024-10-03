using System.Collections;
using UnityEngine;

public class HollowPurple : MonoBehaviour
{
    [SerializeField] private Transform transform;
    [SerializeField] private float scaleSpeed = 1.1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private ParticleSystem particleSystem;
    private float particleSystemScaleSpeed = 1.25f;

    public Vector3 direction;
    private bool isScaling = false;

    private void Start()
    {
        transform = GetComponent<Transform>();
    }

    private void Update()
    {
        transform.position += direction * Time.deltaTime * moveSpeed;
        if(isScaling)
        {
            return;
        }
        StartCoroutine(ScaleSphere());
        StartCoroutine(ScaleParticleSystem());

    }

    private IEnumerator ScaleSphere()
    {
        transform.localScale *= scaleSpeed;
        isScaling = true;
        yield return new WaitForSeconds(0.5f);
        isScaling = false;
    }

    private IEnumerator ScaleParticleSystem()
    {
        particleSystem.transform.localScale *= particleSystemScaleSpeed;
        yield return new WaitForSeconds(0.75f);
    }
}

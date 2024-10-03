using System.Collections;
using UnityEngine;

public class HollowPurple : MonoBehaviour
{
    [SerializeField] private Transform transform;
    [SerializeField] private float scaleSpeed = 1.1f;
    [SerializeField] private float moveSpeed = 5f;
    private float particleScaleSpeed = 1.025f;
    private ParticleSystem particleSystem;

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
        StartCoroutine(Scale());

    }

    private IEnumerator Scale()
    {
        transform.localScale *= scaleSpeed;
        isScaling = true;
        yield return new WaitForSeconds(0.5f);
        isScaling = false;
    }
}

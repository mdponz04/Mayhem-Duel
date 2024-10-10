using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private GameObject _target;


    [SerializeField] private MeshCollider _collider;

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = _target.GetComponent<MeshRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            _meshRenderer.material.color = Color.red;
            other.attachedRigidbody.useGravity = false;
        }
        Debug.Log("Triggered");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cube"))
        {
            _meshRenderer.material.color = Color.green;
            other.attachedRigidbody.useGravity = true;
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle Collision Detected");
    }
}

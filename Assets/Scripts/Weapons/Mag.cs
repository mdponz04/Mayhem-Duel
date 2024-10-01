using UnityEngine;

public class Mag : MonoBehaviour
{
    [SerializeField] private int maxAmmo;
    public int Ammo => currentAmmo;
    private int currentAmmo;
    [SerializeField] private Transform attachedGun;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    public void AttachToGun(Transform attachPoint)
    {
        transform.SetParent(attachPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    public void DetachFromGun()
    {
        transform.SetParent(null);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }

    public void UseAmmo()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
        }
    }

    public void Reload()
    {
        currentAmmo = maxAmmo;
    }
}

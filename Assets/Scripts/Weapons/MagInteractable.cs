using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MagInteractable : XRGrabInteractable
{
    private Gun attachedGun;
    private Mag magComponent;
    private Rigidbody rb;
    [SerializeField] private float attachDistance = 0.1f;

    protected override void Awake()
    {
        base.Awake();
        magComponent = GetComponent<Mag>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (attachedGun != null)
        {
            attachedGun.DetachMag();
            attachedGun = null;
        }
        rb.isKinematic = false;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        Collider[] colliders = Physics.OverlapSphere(transform.position, attachDistance);
        foreach (Collider collider in colliders)
        {
            Gun nearbyGun = collider.GetComponent<Gun>();
            if (nearbyGun != null && !nearbyGun.HasMag())
            {
                nearbyGun.AttachMag(magComponent);
                attachedGun = nearbyGun;
                rb.isKinematic = true;
                break;
            }
        }

        // If not attached to a gun, enable gravity
        if (attachedGun == null)
        {
            rb.isKinematic = false;
            magComponent.DetachFromGun();
        }
    }
}

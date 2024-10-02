using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MagInteractable : XRGrabInteractable
{
    private Gun attachedGun;
    private Mag magComponent;

    protected override void Awake()
    {
        base.Awake();
        magComponent = GetComponent<Mag>();
        attachedGun = GetComponentInParent<Gun>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if(attachedGun != null)
        {
            attachedGun.DetachMag();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        // Check if the magazine is near a gun
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.075f);
        foreach (Collider collider in colliders)
        {
            Gun nearbyGun = collider.GetComponent<Gun>();
            if (nearbyGun != null)
            {
                nearbyGun.AttachMag(magComponent);
                break;
            }
        }
    }
}

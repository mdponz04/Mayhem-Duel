using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;
using System.Collections;

public class LeverInteractable : XRSimpleInteractable
{
    public Rifle rifle;
    private NetworkVariable<bool> isGrabbed = new NetworkVariable<bool>(false);
    private Vector3 initialGrabPosition;

    protected override void Awake()
    {
        base.Awake();
        rifle = GetComponentInParent<Rifle>();
    }

    protected void Start()
    {
        rifle = GetComponentInParent<Rifle>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (NetworkManager.Singleton.IsServer)
        {
            initialGrabPosition = args.interactorObject.transform.position;
            isGrabbed.Value = true;
            rifle.OnLeverGrabbedServerRpc(initialGrabPosition);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (NetworkManager.Singleton.IsServer)
        {
            isGrabbed.Value = false;
            rifle.OnLeverReleasedServerRpc();
        }
    }

    private void Update()
    {
        if (isGrabbed.Value && interactorsSelecting.Count > 0)
        {
            Vector3 currentPosition = interactorsSelecting[0].transform.position;
            Vector3 movement = initialGrabPosition - currentPosition;
            rifle.MoveLeverServerRpc(movement);
            initialGrabPosition = currentPosition;
        }
        else
        {
            rifle.ReturnLeverServerRpc();
        }
    }
}

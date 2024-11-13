using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverInteractable : XRSimpleInteractable
{
    public Rifle rifle;
    private Vector3 initialGrabPosition;
    private bool isGrabbed = false;

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
        initialGrabPosition = args.interactorObject.transform.position;
        isGrabbed = true;
        rifle.OnLeverGrabbed(this);
        Debug.Log("Lever grabbed");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        isGrabbed = false;
        rifle.OnLeverReleased();
        Debug.Log("Lever released");
    }

    private void Update()
    {
        if (isGrabbed)
        {
            Vector3 currentPosition = interactorsSelecting[0].transform.position;
            Vector3 movement = initialGrabPosition - currentPosition;
            rifle.MoveLever(movement);
            initialGrabPosition = currentPosition;
        }
        else
        {
            rifle.ReturnLever();
        }
    }
}
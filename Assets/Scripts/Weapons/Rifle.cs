using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Rifle : Gun
{
    public XRGrabInteractable mainGrabInteractable;
    public Transform leverTransform;
    public Transform leverStartTransform;
    public Transform leverEndTransform;
    public AudioClip pumpSound;
    public Transform secondaryGrabPoint;

    public float leverActionSpeed = 20f;
    public float pullThreshold = 0.05f;

    private bool isPulled = true;
    private bool isLeverActionInProgress = false;
    private bool needsManualPull = false;
    private LeverInteractable leverInteractable;
    private bool isFirstMag = false;

    protected override void Start()
    {
        base.Start();
        mainGrabInteractable.selectEntered.AddListener(OnMainGrabbed);
        mainGrabInteractable.selectExited.AddListener(OnMainReleased);
        leverInteractable = GetComponentInChildren<LeverInteractable>();
    }

    private void Update()
    {
        if (!isLeverActionInProgress && !leverInteractable.isActiveAndEnabled)
        {
            ReturnLever();
        }
        CheckLeverStatus();
    }

    public void MoveLever(Vector3 movement)
    {
        float pumpDelta = Vector3.Dot(movement, leverTransform.forward);
        Vector3 newPosition = leverTransform.localPosition - leverTransform.forward * pumpDelta * leverActionSpeed;
        leverTransform.localPosition = ClampLeverPosition(newPosition);
    }

    protected override void Fire()
    {
        if (isPulled && canFire && currentMag != null && currentMag.Ammo > 0 && !isLeverActionInProgress && !needsManualPull)
        {
            base.Fire();
            if(isAuto)
            {
                StartCoroutine(AutomateLeverAction());
            }
            else
            {
                needsManualPull = true;
                canFire = false;
                isPulled = false;
            }
        }
        else if (currentMag == null || currentMag.Ammo <= 0)
        {
            PlaySound("EmptyClip");
            DetachMag();
        }
    }

    protected override IEnumerator AutoFire()
    {
        while (isTriggerPressed && (currentMag != null && currentMag.Ammo > 0))
        {
            Fire();
            yield return null;
        }
        if (currentMag == null || currentMag.Ammo <= 0)
        {
            PlaySound("EmptyClip");
            DetachMag();
        }
        yield return null;
    }

    private IEnumerator AutomateLeverAction()
    {
        isLeverActionInProgress = true;
        canFire = false;
        leverInteractable.enabled = false;

        // Pull lever back
        while (Vector3.Distance(leverTransform.localPosition, leverEndTransform.localPosition) > pullThreshold)
        {
            leverTransform.localPosition = Vector3.MoveTowards(leverTransform.localPosition, leverEndTransform.localPosition, leverActionSpeed * Time.deltaTime);
            yield return null;
        }

        //PlaySound("PumpSound");

        // Return lever to start
        while (Vector3.Distance(leverTransform.localPosition, leverStartTransform.localPosition) > pullThreshold)
        {
            leverTransform.localPosition = Vector3.MoveTowards(leverTransform.localPosition, leverStartTransform.localPosition, leverActionSpeed * Time.deltaTime);
            yield return null;
        }

        isLeverActionInProgress = false;
        isPulled = true;
        canFire = true;
        leverInteractable.enabled = true;
    }

    public void ReturnLever()
    {
        if (!needsManualPull)
        {
            Vector3 newPosition = Vector3.MoveTowards(leverTransform.localPosition, leverStartTransform.localPosition, leverActionSpeed * Time.deltaTime);
            leverTransform.localPosition = newPosition;
        }
    }

    private void CheckLeverStatus()
    {
        if (Vector3.Distance(leverTransform.localPosition, leverEndTransform.localPosition) < pullThreshold)
        {
            isPulled = true;
            needsManualPull = false;
            canFire = true;
            //PlaySound("PumpSound");
        }
    }

    private Vector3 ClampLeverPosition(Vector3 newPosition)
    {
        return new Vector3(
            leverStartTransform.localPosition.x,
            leverStartTransform.localPosition.y,
            Mathf.Clamp(newPosition.z, leverEndTransform.localPosition.z, leverStartTransform.localPosition.z)
        );
    }

    void OnMainGrabbed(SelectEnterEventArgs args)
    {
        leverInteractable.enabled = true;
    }

    void OnMainReleased(SelectExitEventArgs args)
    {
        //leverInteractable.enabled = false;
    }

    public void OnLeverGrabbed(LeverInteractable lever)
    {
        // You can add any additional logic here when the lever is grabbed
        //MoveLever(leverEndTransform.transform.position);
    }

    public void OnLeverReleased()
    {
        // You can add any additional logic here when the lever is released
        ReturnLever();
    }

    protected override void PlaySound(string soundName)
    {
        base.PlaySound(soundName);
        if (soundName == "PumpSound" && pumpSound != null)
        {
            AudioSource.PlayClipAtPoint(pumpSound, transform.position);
        }
    }

    public override void AttachMag(Mag mag)
    {
        base.AttachMag(mag);
        if(isFirstMag)
        {
            isFirstMag = false;
            needsManualPull = false;
            return;
        }
        needsManualPull = true;
    }

}
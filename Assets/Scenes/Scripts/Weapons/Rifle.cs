using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;


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

    private NetworkVariable<bool> isPulled = new NetworkVariable<bool>(true);
    private NetworkVariable<bool> isLeverActionInProgress = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> needsManualPull = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> canFire = new NetworkVariable<bool>(true);
    private LeverInteractable leverInteractable;
    private NetworkVariable<bool> isFirstMag = new NetworkVariable<bool>(false);

    protected override void Start()
    {
        base.Start();
        mainGrabInteractable.selectEntered.AddListener(OnMainGrabbed);
        mainGrabInteractable.selectExited.AddListener(OnMainReleased);
        leverInteractable = GetComponentInChildren<LeverInteractable>();
    }

    private void Update()
    {
        if (!isLeverActionInProgress.Value && !leverInteractable.isActiveAndEnabled)
        {
            ReturnLeverServerRpc();
        }
        CheckLeverStatusServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveLeverServerRpc(Vector3 movement)
    {
        float pumpDelta = Vector3.Dot(movement, leverTransform.forward);
        Vector3 newPosition = leverTransform.localPosition - leverTransform.forward * pumpDelta * leverActionSpeed;
        leverTransform.localPosition = ClampLeverPosition(newPosition);
    }

    protected override void Fire()
    {
        if (!IsServer) return;

        if (isPulled.Value && canFire.Value &&
            currentMagReference.Value.TryGet(out NetworkObject magObject) &&
            !isLeverActionInProgress.Value &&
            !needsManualPull.Value)
        {
<<<<<<< Updated upstream:Assets/Scripts/Weapons/Rifle.cs
            base.Fire();
            if (isAuto)
=======
            Mag mag = magObject.GetComponent<Mag>();
            if (mag != null && mag.Ammo > 0)
>>>>>>> Stashed changes:Assets/Scenes/Scripts/Weapons/Rifle.cs
            {
                CreateBulletClientRpc(barrel.position, attackDamage);
                PlaySoundClientRpc("GunShot");
                mag.UseAmmo();

                if (isAuto)
                {
                    StartCoroutine(AutomateLeverAction());
                }
                else
                {
                    needsManualPull.Value = true;
                    canFire.Value = false;
                    isPulled.Value = false;
                }
            }
            else
            {
                PlaySoundClientRpc("EmptyClip");
                DetachMagServerRpc();
            }
        }
    }

    private IEnumerator AutomateLeverAction()
    {
        isLeverActionInProgress.Value = true;
        canFire.Value = false;
        leverInteractable.enabled = false;

        // Pull lever back
        while (Vector3.Distance(leverTransform.localPosition, leverEndTransform.localPosition) > pullThreshold)
        {
            leverTransform.localPosition = Vector3.MoveTowards(leverTransform.localPosition, leverEndTransform.localPosition, leverActionSpeed * Time.deltaTime);
            yield return null;
        }

        // Return lever to start
        while (Vector3.Distance(leverTransform.localPosition, leverStartTransform.localPosition) > pullThreshold)
        {
            leverTransform.localPosition = Vector3.MoveTowards(leverTransform.localPosition, leverStartTransform.localPosition, leverActionSpeed * Time.deltaTime);
            yield return null;
        }

        isLeverActionInProgress.Value = false;
        isPulled.Value = true;
        canFire.Value = true;
        leverInteractable.enabled = true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReturnLeverServerRpc()
    {
        if (!needsManualPull.Value)
        {
            Vector3 newPosition = Vector3.MoveTowards(leverTransform.localPosition, leverStartTransform.localPosition, leverActionSpeed * Time.deltaTime);
            leverTransform.localPosition = newPosition;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CheckLeverStatusServerRpc()
    {
        if (Vector3.Distance(leverTransform.localPosition, leverEndTransform.localPosition) < pullThreshold)
        {
            isPulled.Value = true;
            needsManualPull.Value = false;
            canFire.Value = true;
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

    [ServerRpc(RequireOwnership = false)]
    public void OnLeverGrabbedServerRpc(Vector3 grabPosition)
    {
        // Server-side lever grabbed logic
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnLeverReleasedServerRpc()
    {
        ReturnLeverServerRpc();
    }

    void OnMainGrabbed(SelectEnterEventArgs args)
    {
        leverInteractable.enabled = true;
    }

    void OnMainReleased(SelectExitEventArgs args)
    {
        // Lever interactable remains enabled
    }

    [ClientRpc]
    protected override void PlaySoundClientRpc(string soundName)
    {
        base.PlaySoundClientRpc(soundName);
        if (soundName == "PumpSound" && pumpSound != null)
        {
            AudioSource.PlayClipAtPoint(pumpSound, transform.position);
        }
    }

    public override void AttachMagServerRpc(NetworkObjectReference magReference)
    {
<<<<<<< Updated upstream:Assets/Scripts/Weapons/Rifle.cs
        base.AttachMag(mag);
        if (isFirstMag)
=======
        base.AttachMagServerRpc(magReference);

        if (isFirstMag.Value)
>>>>>>> Stashed changes:Assets/Scenes/Scripts/Weapons/Rifle.cs
        {
            isFirstMag.Value = false;
            needsManualPull.Value = false;
            return;
        }
        needsManualPull.Value = true;
    }
}
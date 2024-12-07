using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PumpActionShotgun : Gun
{
    public XRGrabInteractable mainGrabInteractable;
    public Transform pumpTransform;
    public Transform pumpStartTransform;
    public Transform pumpEndTransform;
    public AudioClip pumpSound;

    public float pumpForce = 20f;
    public float pumpReturnSpeed = 5f;
    public float pumpThreshold = 0.5f;
    public int currentAmmo;
    public int maxAmmo = 8;
    public int palletsPerShot = 12;

    private bool isPumped = false;
    private bool isPumpGrabbed = false;
    private Vector3 lastHandPosition;
    private IXRSelectInteractor pumpGrabbingHand;

    protected override void Start()
    {
        mainGrabInteractable.activated.AddListener(TryFire);
        mainGrabInteractable.selectEntered.AddListener(OnGrabbed);
        mainGrabInteractable.selectExited.AddListener(OnReleased);

        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isPumpGrabbed)
        {
            MovePump();
        }
        else
        {
            ReturnPump();
        }

        CheckPumpStatus();
    }

    void TryFire(ActivateEventArgs args)
    {
        if (canFire)
        {
            Fire();
            canFire = false;
            isPumped = false;
        }
    }

    protected override void Fire()
    {
        if (currentAmmo > 0)
        {
            PalletSpawn();
            PlaySound("GunShot");
        }
        else
        {
            PlaySound("EmptyClip");
        }
    }

    private void PalletSpawn()
    {
        for (int i = 0; i < palletsPerShot; i++)
        {
            Transform randomRotation = barrel;
            randomRotation.Rotate(Vector3.up, Random.Range(-5, 5));
            Bullet.Create(barrel.position, randomRotation, 20, attackDamage);
        }
        currentAmmo--;
    }

    protected override void PlaySound(string soundName)
    {
        base.PlaySound(soundName);

        switch (soundName)
        {
            case "Pump":
                AudioManager.instance.PlayAudioClip(pumpSound);
                break;
        }

    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        // Check if this is a second grab (i.e., grabbing the pump)
        if (mainGrabInteractable.interactorsSelecting.Count > 1)
        {
            Debug.Log("Pump grabbed");
            pumpGrabbingHand = args.interactorObject;
            isPumpGrabbed = true;
            lastHandPosition = pumpGrabbingHand.transform.position;
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        // Check if the pump is being released
        if (args.interactorObject == pumpGrabbingHand)
        {
            Debug.Log("Pump released");
            isPumpGrabbed = false;
            pumpGrabbingHand = null;
        }
    }

    void MovePump()
    {
        if (pumpGrabbingHand != null)
        {
            Vector3 handDelta = lastHandPosition - pumpGrabbingHand.transform.position;
            float pumpDelta = Vector3.Dot(handDelta, pumpTransform.forward);;
            Vector3 newPosition = pumpTransform.localPosition - pumpTransform.forward * (pumpDelta * pumpForce);
            pumpTransform.localPosition = ClampPumpPosition(newPosition);
            lastHandPosition = pumpGrabbingHand.transform.position;
        }
    }

    void ReturnPump()
    {
            pumpTransform.localPosition = Vector3.MoveTowards(pumpTransform.localPosition,
            pumpStartTransform.localPosition, 4 * pumpReturnSpeed * Time.deltaTime);
    }

    Vector3 ClampPumpPosition(Vector3 position)
    {
        return new Vector3(
            pumpTransform.localPosition.x,
            pumpTransform.localPosition.y,
            Mathf.Clamp(position.z, pumpEndTransform.localPosition.z, pumpStartTransform.localPosition.z)
        );
    }

    void CheckPumpStatus()
    {
        if (Vector3.Distance(pumpTransform.localPosition, pumpEndTransform.localPosition) < pumpThreshold)
        {
            isPumped = true;
        }
        else if (isPumped && Vector3.Distance(pumpTransform.localPosition, pumpStartTransform.localPosition) < pumpThreshold)
        {
            Debug.Log("Pump action complete");
            canFire = true;
            isPumped = false;
        }
    }
}
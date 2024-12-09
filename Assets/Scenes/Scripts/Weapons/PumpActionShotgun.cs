using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Netcode;

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

    private NetworkVariable<bool> isPumped = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isPumpGrabbed = new NetworkVariable<bool>(false);
    private Vector3 lastHandPosition;
    private IXRSelectInteractor pumpGrabbingHand;

    protected override void Start()
    {
        base.Start();
        mainGrabInteractable.activated.AddListener(TryFire);
        mainGrabInteractable.selectEntered.AddListener(OnGrabbed);
        mainGrabInteractable.selectExited.AddListener(OnReleased);

        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isPumpGrabbed.Value)
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
        if (canFire.Value)
        {
            Fire();
            canFire.Value = false;
            isPumped.Value = false;
        }
    }

    protected override void Fire()
    {
        if (!IsServer) return;

        if (currentAmmo > 0)
        {
            PalletSpawn();
            PlaySoundClientRpc("GunShot");
        }
        else
        {
            PlaySoundClientRpc("EmptyClip");
        }
    }

    private void PalletSpawn()
    {
        for (int i = 0; i < palletsPerShot; i++)
        {
            Transform randomRotation = barrel;
            randomRotation.Rotate(Vector3.up, Random.Range(-5, 5));
            CreateBulletClientRpc(barrel.position, attackDamage);
        }
        currentAmmo--;
    }

    void OnGrabbed(SelectEnterEventArgs args)
    {
        if (mainGrabInteractable.interactorsSelecting.Count > 1)
        {
            Debug.Log("Pump grabbed");
            pumpGrabbingHand = args.interactorObject;
            isPumpGrabbed.Value = true;
            lastHandPosition = pumpGrabbingHand.transform.position;
        }
    }

    void OnReleased(SelectExitEventArgs args)
    {
        if (args.interactorObject == pumpGrabbingHand)
        {
            Debug.Log("Pump released");
            isPumpGrabbed.Value = false;
            pumpGrabbingHand = null;
        }
    }

    void MovePump()
    {
        if (pumpGrabbingHand != null)
        {
            Vector3 handDelta = lastHandPosition - pumpGrabbingHand.transform.position;
            float pumpDelta = Vector3.Dot(handDelta, pumpTransform.forward); 
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
            isPumped.Value = true;
        }
        else if (isPumped.Value && Vector3.Distance(pumpTransform.localPosition, pumpStartTransform.localPosition) < pumpThreshold)
        {
            Debug.Log("Pump action complete");
            canFire.Value = true;
            isPumped.Value = false;
        }
    }
}
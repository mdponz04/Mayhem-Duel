using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MeteorPistol : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    public LayerMask layerMask;
    public Transform shootSource;
    public float distance = 2f;

    private bool rayActivate = false;
    private void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(x => StartShoot());
        grabInteractable.deactivated.AddListener(x => StopShoot());
    }
    private void Update()
    {
        if (rayActivate) RayCastCheck();
    }
    private void StartShoot()
    {
        particles.Play();
        rayActivate = true;
    }
    private void StopShoot()
    {
        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        rayActivate = false;
    }

    private void RayCastCheck()
    {
        bool isHit = Physics.Raycast(shootSource.position, shootSource.forward, out RaycastHit hit, distance, layerMask);

        if (isHit)
        {
            hit.transform.gameObject.SendMessage("Break", SendMessageOptions.DontRequireReceiver);
        }
    }
}

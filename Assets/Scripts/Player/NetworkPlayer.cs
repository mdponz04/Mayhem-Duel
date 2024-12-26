using System.Collections;
using System.Collections.Generic;
using TheDamage;
using TheHealth;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform vrRoot;
    public Transform vrHead;
    public Transform vrLeftHand;
    public Transform vrLeftHandRootBone;
    public Transform vrRightHand;
    public Transform vrRightHandRootBone;

    public Renderer[] localMeshesToDisable;


    //Setup properties to handle damage
    private HealthSystem healthSystem;
    private float maxHealth = 100f;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.SetUp(maxHealth);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (Renderer mesh in localMeshesToDisable)
            {
                mesh.enabled = false;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            vrRoot.position = VRRipReference.Singleton.Root.position;
            vrRoot.rotation = VRRipReference.Singleton.Root.rotation;

            vrHead.position = VRRipReference.Singleton.Head.position;
            vrHead.rotation = VRRipReference.Singleton.Head.rotation;

            vrLeftHand.position = VRRipReference.Singleton.LeftHand.position;
            //vrLeftHand.rotation = VRRipReference.Singleton.LeftHand.rotation;
            vrLeftHandRootBone.rotation = VRRipReference.Singleton.LeftHandRootBone.rotation;

            vrRightHand.position = VRRipReference.Singleton.RightHand.position;
            //vrRightHand.rotation = VRRipReference.Singleton.RightHand.rotation;
            vrRightHandRootBone.rotation = VRRipReference.Singleton.RightHandRootBone.rotation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRipReference : MonoBehaviour
{
    public static VRRipReference Singleton;

    public Transform Root;
    public Transform Head;
    public Transform LeftHand;
    public Transform LeftHandRootBone;
    public Transform RightHand;
    public Transform RightHandRootBone;
    private void Awake()
    {
        Singleton = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnableTeleportRay : MonoBehaviour
{
    public GameObject leftTeleRay;
    public GameObject rightTeleRay;

    public InputActionProperty leftTeleActivate;
    public InputActionProperty rightTeleActivate;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (leftTeleRay != null)
        {
            leftTeleRay.SetActive(leftTeleActivate.action.ReadValue<float>() > 0.1f);
        }

        if (rightTeleRay != null)
        {
            rightTeleRay.SetActive(rightTeleActivate.action.ReadValue<float>() > 0.1f);
        }
    }
}

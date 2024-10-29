using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleMachineGunBulletCollision : MonoBehaviour
{
    private ModifyMachineGun machineGun;
    // Start is called before the first frame update
    void Start()
    {
        machineGun = GetComponentInParent<ModifyMachineGun>();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (machineGun != null)
        {
            machineGun.OnParticleCollision(other);
        }
    }
}
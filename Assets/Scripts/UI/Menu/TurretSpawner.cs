using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour
{
    #region Serialized Field
    [SerializeField] private Transform SpawnPosition;

    [SerializeField] private GameObject artilleryTurret;
    [SerializeField] private GameObject machineGunTurret;
    [SerializeField] private GameObject singleTargetTurret;
    #endregion
    public void SpawnArtilleryTurret()
    {
        if (EconomySystem.DeductPoint(1000))
        {
            Debug.Log("Spawn success");
            Instantiate(artilleryTurret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnMachineGunTurret()
    {
        if (EconomySystem.DeductPoint(1000))
        {
            Instantiate(machineGunTurret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnSingleTargetTurret()
    {
        if (EconomySystem.DeductPoint(1000))
        {
            Instantiate(artilleryTurret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
}

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
        Instantiate(artilleryTurret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    public void SpawnMachineGunTurret()
    {
        Instantiate(machineGunTurret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    public void SpawnSingleTargetTurret()
    {
        Instantiate(singleTargetTurret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
    }
}

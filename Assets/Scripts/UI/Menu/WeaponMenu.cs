using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMenu : MonoBehaviour
{
    #region Serialized Field

    [SerializeField] private GameObject AkPrefab;
    [SerializeField] private GameObject BarretPrefab;
    [SerializeField] private GameObject ColtPrefab;
    [SerializeField] private GameObject GatlingPrefab;
    [SerializeField] private GameObject HandgunPrefab;
    [SerializeField] private GameObject ShotgunPrefab;
    [SerializeField] private GameObject SMAWPrefab;
    [SerializeField] private GameObject UziPrefab;
    [SerializeField] private Transform SpawnPosition;

    #endregion

    public void SpawnAK()
    {
        Instantiate(GameAssets.i.pfAK, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfAKMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }

    }

    public void SpawnBarret()
    {
        Instantiate(GameAssets.i.pfBarret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfBarretMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnColt()
    {
        Instantiate(GameAssets.i.pfColt, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfColtMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnGatling()
    {
        Instantiate(GameAssets.i.pfGatling, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfGatlingMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnHandgun()
    {
        Instantiate(GameAssets.i.pfHandgun, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfHandgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnShotgun()
    {
        Instantiate(GameAssets.i.pfShotgun, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfShotgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnSMAW()
    {
        Instantiate(GameAssets.i.pfSMAW, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfSMAWMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnUzi()
    {
        Instantiate(GameAssets.i.pfUzi, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        for (int i = 0; i < 2; i++)
        {
            Instantiate(GameAssets.i.pfUziMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
}

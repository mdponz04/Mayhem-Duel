using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    #region Serialized Field
    [SerializeField] private Transform SpawnPosition;
    #endregion


    public void SpawnAK()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfAK, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfAKMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnBarret()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfBarret, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfBarretMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnColt()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfColt, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfColtMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnGatling()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfGatling, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfGatlingMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnHandgun()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfHandgun, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfHandgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnShotgun()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfShotgun, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfShotgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnSMAW()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfSMAW, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfSMAWMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnUzi()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            Instantiate(GameAssets.i.pfUzi, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            for (int i = 0; i < 2; i++)
            {
                Instantiate(GameAssets.i.pfUziMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }
}

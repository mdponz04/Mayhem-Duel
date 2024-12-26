using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagSpawner : MonoBehaviour
{
    #region Serialized Field
    [SerializeField] private Transform SpawnPosition;
    #endregion
    private int spawnNumber = 1;
    public void SpawnAKMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfAKMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnBarretMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfBarretMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnColtMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfColtMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnGatlingMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfGatlingMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnHandgunMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfHandgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnShotgunMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfShotgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnSMAWMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfSMAWMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }

    public void SpawnUziMag()
    {
        if (EconomySystem.DeductPoint(EconomySystem.TestPrice))
        {
            for (int i = 0; i < spawnNumber; i++)
            {
                Instantiate(GameAssets.i.pfUziMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
            }
        }
    }
}

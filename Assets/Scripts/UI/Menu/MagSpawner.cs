using UnityEngine;

public class MagSpawner : MonoBehaviour
{
    #region Serialized Field
    [SerializeField] private Transform SpawnPosition;
    #endregion
    private int spawnNumber = 1;
    public void SpawnAKMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfAKMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }

    }

    public void SpawnBarretMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfBarretMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnColtMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfColtMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnGatlingMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfGatlingMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnHandgunMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfHandgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnShotgunMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfShotgunMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnSMAWMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfSMAWMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }

    public void SpawnUziMag()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            Instantiate(GameAssets.i.pfUziMag, SpawnPosition.position + new Vector3(0, 1, 0), Quaternion.identity);
        }
    }
}

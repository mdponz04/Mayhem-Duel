using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }

    public Transform pfBullet;
    public Transform pfRocket;
    public Transform pfAK;
    public Transform pfBarret;
    public Transform pfColt;
    public Transform pfGatling;
    public Transform pfHandgun;
    public Transform pfShotgun;
    public Transform pfSMAW;
    public Transform pfUzi;
    public Transform pfAKMag;
    public Transform pfBarretMag;
    public Transform pfColtMag;
    public Transform pfGatlingMag;
    public Transform pfHandgunMag;
    public Transform pfShotgunMag;
    public Transform pfSMAWMag;
    public Transform pfUziMag;
}

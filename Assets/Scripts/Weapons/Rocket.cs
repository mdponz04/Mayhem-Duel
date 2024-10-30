using UnityEngine;

public class Rocket : Bullet
{
    public new static void Create(Vector3 position, Transform direction, float speed)
    {
        Transform bulleTransform = Instantiate(GameAssets.i.pfRocket, position, Quaternion.identity);
        Rocket rocket = bulleTransform.GetComponent<Rocket>();
        rocket.SetUp(direction, speed);
    }
}

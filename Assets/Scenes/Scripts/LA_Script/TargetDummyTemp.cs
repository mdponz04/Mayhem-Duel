using CodeMonkey.Utils;
using TheDamage;
using UnityEngine;

public class TargetDummyTemp : Vulnerable
{
    public float HP = 10000f;
    public float DestroyDuration = 2f;

    private bool isDead = false;
    public Vector3 GetWorlPosition()
    {
        return transform.position;
    }

    public override void TakeDamge(float damage)
    {
        if (isDead)
        {
            return;
        }

        HP -= damage;
        UtilsClass.CreateWorldTextPopup(damage.ToString(), gameObject.transform.position);

        if (HP <= 0)
        {
            Dead();
        }
    }

    void Dead()
    {
        isDead = true;

        Destroy(gameObject, DestroyDuration);
        //call remove gameObject event?
    }

    public GameObject GetEnemyGameObject()
    {
        return gameObject;
    }

}

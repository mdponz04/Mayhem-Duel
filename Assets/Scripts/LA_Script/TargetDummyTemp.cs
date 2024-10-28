using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummyTemp : MonoBehaviour, IEnemyTemp 
{
    public float HP = 10f;
    public float DestroyDuration = 2f;

    private bool isDead = false;
    public Vector3 GetWorlPosition()
    {
        return transform.position;
    }

    public virtual void TakeDamage(float damage)
    {
        //Debug.Log("I take damage");
        if (isDead)
        {
            return;
        }

        HP -= damage;
        UtilsClass.CreateWorldTextPopup(damage.ToString(), gameObject.transform.localPosition);

        if(HP <= 0)
        {
            Dead();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
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
    //private void OnParticleCollision(GameObject other)
    //{
    //    TakeDamage(1);
    //}
}

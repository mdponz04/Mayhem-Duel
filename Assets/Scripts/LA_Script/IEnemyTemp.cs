using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyTemp
{
    public void TakeDamage(float damage);
    public Vector3 GetWorlPosition();
    public GameObject GetEnemyGameObject();
}

using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpsDummyTemp : TargetDummyTemp
{
    public float TimeIntervalInSecond = 1f;
    private float currentTimeInterval;
    private float damageInTimeInterval;

    private float dps;
    // Start is called before the first frame update
    void Start()
    {
        UtilsClass.CreateWorldTextUpdater(GetDpsText, transform.position);
    }

    // Update is called once per frame
    public override void Update()
    {
    }
    private void FixedUpdate()
    {
        currentTimeInterval -= Time.deltaTime;
        if (currentTimeInterval <= 0)
        {
            dps = damageInTimeInterval;
            damageInTimeInterval = 0;
            currentTimeInterval = TimeIntervalInSecond;
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        damageInTimeInterval += damage;
    }

    private string GetDpsText()
    {
        return $"Dps: {dps}/{TimeIntervalInSecond}s";
    }
}
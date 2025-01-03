using CodeMonkey.Utils;
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

    private void FixedUpdate()
    {
        /*healthSystem.SetUp(HP);*/
        currentTimeInterval -= Time.deltaTime;
        if (currentTimeInterval <= 0)
        {
            dps = damageInTimeInterval;
            damageInTimeInterval = 0;
            currentTimeInterval = TimeIntervalInSecond;
        }
    }

    public override void TakeDamge(float damage)
    {
        base.TakeDamge(damage);
        damageInTimeInterval += damage;
    }

    private string GetDpsText()
    {
        return $"Dps: {dps}/{TimeIntervalInSecond}s";
    }
}

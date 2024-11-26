using Assets.Scripts.TheCastle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerTurretManager : MonoBehaviour
{
    public List<TurretType> turretTypes;
    public List<PlacedObjectTypeSO> turretPfs;
    private Dictionary<TurretType, PlacedObjectTypeSO> turretMap;

    private void Awake()
    {
        turretMap = new();

        for (int i = 0; i < turretTypes.Count; i++)
        {
            if (!turretMap.ContainsKey(turretTypes[i]))
            {
                turretMap.Add(turretTypes[i], turretPfs[i]);
            }
        }
    }

    public PlacedObjectTypeSO GetTurretPf(TurretType turretType)
    {
        return turretMap[turretType];
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TheCastle
{
    [CreateAssetMenu(fileName = "NewPlacedObjectType", menuName = "ScriptableObject/PlacedObject")]
    public class PlacedObjectTypeSO : ScriptableObject
    {
        public string nameString;
        public Transform prefab;
        public int width;
        public int height;
    }
}

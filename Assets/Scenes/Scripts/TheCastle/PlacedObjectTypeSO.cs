using UnityEngine;

namespace Assets.Scripts.TheCastle
{
    [CreateAssetMenu(fileName = "NewPlacedObjectType", menuName = "ScriptableObject/PlacedObject")]
    public class PlacedObjectTypeSO : ScriptableObject
    {
        public string nameString;
        public Transform prefab;
        public Transform visual;
        public int width;
        public int height;
    }
}
